using AdventureWorks.Enterprise.WebApp.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Enterprise.WebApp.Services
{
    /// <summary>
    /// Servicio central para acceder a la API REST del backend AdventureWorks.
    /// Incluye métodos para empleados, productos e órdenes, con soporte de paginación y operaciones CRUD.
    /// Implementa estrategias de fallback cuando el servidor no responde o ciertas capacidades no están disponibles.
    /// </summary>
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Inicializa el servicio con un cliente HTTP configurado mediante DI.
        /// </summary>
        /// <param name="httpClient">Instancia de <see cref="HttpClient"/> configurada con BaseAddress.</param>
        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        #region Empleados

        /// <summary>Obtiene la lista completa de empleados activos desde la API.</summary>
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<Employee>>("api/employees");
                return result ?? new List<Employee>();
            }
            catch
            {
                return new List<Employee>();
            }
        }

        /// <summary>Obtiene empleados paginados desde el servidor.</summary>
        /// <param name="paginationParams">Parámetros de paginación y búsqueda.</param>
        /// <returns>Resultado paginado de empleados.</returns>
        public async Task<PagedResult<Employee>> GetEmployeesPagedAsync(PaginationParams paginationParams)
        {
            try
            {
                var queryString = paginationParams.ToQueryString();
                var url = $"api/employees/paged?{queryString}";
                var result = await _httpClient.GetFromJsonAsync<PagedResult<Employee>>(url);
                return result ?? new PagedResult<Employee>();
            }
            catch
            {
                // Fallback a paginación del lado cliente si el servidor no soporta paginación
                var allEmployees = await GetEmployeesAsync();
                return SimulateServerPagination(allEmployees, paginationParams);
            }
        }

        /// <summary>Obtiene el conteo total de empleados activos.</summary>
        public async Task<int> GetEmployeesCountAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<int>("api/employees/count");
                return result;
            }
            catch
            {
                // Fallback: obtener todos los empleados y contar
                var allEmployees = await GetEmployeesAsync();
                return allEmployees.Count;
            }
        }

        /// <summary>Simula paginación en cliente cuando el servidor no soporta paginación.</summary>
        /// <param name="allEmployees">Lista completa de empleados.</param>
        /// <param name="paginationParams">Parámetros de paginación.</param>
        /// <returns>Resultado paginado simulado.</returns>
        private PagedResult<Employee> SimulateServerPagination(List<Employee> allEmployees, PaginationParams paginationParams)
        {
            var filteredEmployees = allEmployees.AsEnumerable();

            // Aplicar filtro de búsqueda
            if (!string.IsNullOrWhiteSpace(paginationParams.Search))
            {
                var search = paginationParams.Search.ToLower();
                filteredEmployees = filteredEmployees.Where(e =>
                    e.LoginId.ToLower().Contains(search) ||
                    e.JobTitle.ToLower().Contains(search) ||
                    (!string.IsNullOrEmpty(e.NationalIdNumber) && e.NationalIdNumber.ToLower().Contains(search)) ||
                    e.EmployeeId.ToString().Contains(search));
            }

            // Aplicar ordenamiento
            if (!string.IsNullOrWhiteSpace(paginationParams.SortBy))
            {
                var isDescending = paginationParams.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
                filteredEmployees = paginationParams.SortBy.ToLower() switch
                {
                    "employeeid" => isDescending ? filteredEmployees.OrderByDescending(e => e.EmployeeId) : filteredEmployees.OrderBy(e => e.EmployeeId),
                    "loginid" => isDescending ? filteredEmployees.OrderByDescending(e => e.LoginId) : filteredEmployees.OrderBy(e => e.LoginId),
                    "jobtitle" => isDescending ? filteredEmployees.OrderByDescending(e => e.JobTitle) : filteredEmployees.OrderBy(e => e.JobTitle),
                    "hiredate" => isDescending ? filteredEmployees.OrderByDescending(e => e.HireDate) : filteredEmployees.OrderBy(e => e.HireDate),
                    "nationalidnumber" => isDescending ? filteredEmployees.OrderByDescending(e => e.NationalIdNumber ?? string.Empty) : filteredEmployees.OrderBy(e => e.NationalIdNumber ?? string.Empty),
                    _ => isDescending ? filteredEmployees.OrderByDescending(e => e.EmployeeId) : filteredEmployees.OrderBy(e => e.EmployeeId)
                };
            }

            var totalCount = filteredEmployees.Count();
            var employees = filteredEmployees.Skip((paginationParams.Page - 1) * paginationParams.PageSize).Take(paginationParams.PageSize).ToList();

            return new PagedResult<Employee>
            {
                Items = employees,
                TotalCount = totalCount,
                CurrentPage = paginationParams.Page,
                PageSize = paginationParams.PageSize
            };
        }

        /// <summary>Obtiene un empleado por su identificador.</summary>
        public async Task<Employee?> GetEmployeeAsync(int id)
        {
            try { return await _httpClient.GetFromJsonAsync<Employee>($"api/employees/{id}"); }
            catch { return null; }
        }

        /// <summary>Crea un nuevo empleado.</summary>
        /// <remarks>
        /// Mejora: manejar respuestas 201 Created incluso si el cuerpo viene vacío y hacer fallback usando Location.
        /// </remarks>
        public async Task<Employee?> CreateEmployeeAsync(Employee employee)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/employees", employee);
                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[CreateEmployeeAsync] StatusCode={response.StatusCode} ErrorBody={error}");
                    return null;
                }

                // Leer cuerpo (algunas implementaciones de CreatedAtAction pueden devolver vacío).
                var raw = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(raw))
                {
                    // Intentar obtener desde Location header
                    if (response.Headers.Location != null)
                    {
                        var location = response.Headers.Location.ToString();
                        // Si es relativo, construir URL absoluta
                        if (!response.Headers.Location.IsAbsoluteUri && _httpClient.BaseAddress != null)
                            location = new System.Uri(_httpClient.BaseAddress, location).ToString();
                        try
                        {
                            var fetched = await _httpClient.GetFromJsonAsync<Employee>(location);
                            if (fetched != null) return fetched;
                        }
                        catch (System.Exception exLoc)
                        {
                            System.Diagnostics.Debug.WriteLine($"[CreateEmployeeAsync] Fallback GET failed: {exLoc.Message}");
                        }
                    }
                    // Último fallback: devolver el objeto enviado con EmployeeId si fue asignado por el servidor
                    return employee.EmployeeId > 0 ? employee : employee;
                }

                try
                {
                    var created = JsonSerializer.Deserialize<Employee>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (created == null && response.Headers.Location != null)
                    {
                        var location = response.Headers.Location.ToString();
                        if (!response.Headers.Location.IsAbsoluteUri && _httpClient.BaseAddress != null)
                            location = new System.Uri(_httpClient.BaseAddress, location).ToString();
                        created = await _httpClient.GetFromJsonAsync<Employee>(location);
                    }
                    return created;
                }
                catch (System.Exception exDeser)
                {
                    System.Diagnostics.Debug.WriteLine($"[CreateEmployeeAsync] Deserialize failed: {exDeser.Message} Raw={raw}");
                    if (response.Headers.Location != null)
                    {
                        try
                        {
                            var location = response.Headers.Location.ToString();
                            if (!response.Headers.Location.IsAbsoluteUri && _httpClient.BaseAddress != null)
                                location = new System.Uri(_httpClient.BaseAddress, location).ToString();
                            var fetched = await _httpClient.GetFromJsonAsync<Employee>(location);
                            if (fetched != null) return fetched;
                        }
                        catch { }
                    }
                    return null;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CreateEmployeeAsync] Exception: {ex.Message}");
                return null;
            }
        }

        /// <summary>Actualiza los datos de un empleado existente.</summary>
        public async Task<bool> UpdateEmployeeAsync(int id, Employee employee)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/employees/{id}", employee);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        /// <summary>Elimina (desactiva) un empleado.</summary>
        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/employees/{id}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        /// <summary>Obtiene empleados con más antigüedad usando el endpoint especializado.</summary>
        public async Task<List<EmpleadoMasTiempoDto>> GetEmpleadosMasTiempoAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<EmpleadoMasTiempoDto>>("api/employees/mas-tiempo");
                return result ?? new List<EmpleadoMasTiempoDto>();
            }
            catch { return new List<EmpleadoMasTiempoDto>(); }
        }

        /// <summary>Verifica si existen duplicados de ID nacional o Login.</summary>
        public async Task<bool> CheckEmployeeDuplicatesAsync(string nationalId, string loginId, int? excludeEmployeeId = null)
        {
            try
            {
                var allEmployees = await GetEmployeesAsync();
                return allEmployees.Any(e => (e.NationalIdNumber == nationalId || e.LoginId == loginId) && (excludeEmployeeId == null || e.EmployeeId != excludeEmployeeId));
            }
            catch { return false; }
        }

        #endregion

        #region Productos

        /// <summary>Obtiene todos los productos.</summary>
        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<Product>>("api/products");
                return result ?? new List<Product>();
            }
            catch { return new List<Product>(); }
        }

        /// <summary>Obtiene un producto por su ID.</summary>
        public async Task<Product?> GetProductAsync(int id)
        {
            try { return await _httpClient.GetFromJsonAsync<Product>($"api/products/{id}"); }
            catch { return null; }
        }

        /// <summary>Crea un nuevo producto.</summary>
        public async Task<Product?> CreateProductAsync(Product product)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/products", product);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Product>();
            }
            catch { return null; }
        }

        /// <summary>Actualiza un producto existente.</summary>
        public async Task<bool> UpdateProductAsync(int id, Product product)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/products/{id}", product);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        /// <summary>Elimina un producto por su ID.</summary>
        public async Task<bool> DeleteProductAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/products/{id}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        /// <summary>Obtiene todo el inventario de productos.</summary>
        public async Task<List<ProductInventory>> GetAllInventoryAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<ProductInventory>>("api/products/inventory");
                return result ?? new List<ProductInventory>();
            }
            catch { return new List<ProductInventory>(); }
        }

        /// <summary>Obtiene el inventario de un producto específico.</summary>
        public async Task<List<ProductInventory>> GetProductInventoryAsync(int id)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<ProductInventory>>($"api/products/{id}/inventory");
                return result ?? new List<ProductInventory>();
            }
            catch { return new List<ProductInventory>(); }
        }

        /// <summary>Actualiza la cantidad de inventario de un producto en una ubicación.</summary>
        public async Task<bool> UpdateProductInventoryAsync(int id, short locationId, short quantity)
        {
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(quantity), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/products/{id}/inventory/{locationId}", content);
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        /// <summary>Obtiene los productos más vendidos.</summary>
        public async Task<List<TopProductoVendidoDto>> GetTopVendidosAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<TopProductoVendidoDto>>("api/products/top-vendidos");
                return result ?? new List<TopProductoVendidoDto>();
            }
            catch { return new List<TopProductoVendidoDto>(); }
        }

        #endregion

        #region Órdenes

        /// <summary>Obtiene todas las órdenes.</summary>
        public async Task<List<SalesOrder>> GetOrdersAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<SalesOrder>>("api/orders");
                return result ?? new List<SalesOrder>();
            }
            catch { return new List<SalesOrder>(); }
        }

        /// <summary>Obtiene órdenes paginadas desde el servidor.</summary>
        public async Task<PagedResult<SalesOrder>> GetOrdersPagedAsync(PaginationParams paginationParams)
        {
            try
            {
                var queryString = paginationParams.ToQueryString();
                var url = $"api/orders/paged?{queryString}";
                var result = await _httpClient.GetFromJsonAsync<PagedResult<SalesOrder>>(url);
                return result ?? new PagedResult<SalesOrder>();
            }
            catch
            {
                // Fallback a paginación del lado cliente si el servidor no soporta paginación
                var allOrders = await GetOrdersAsync();
                return SimulateServerPaginationOrders(allOrders, paginationParams);
            }
        }

        /// <summary>Obtiene el conteo total de órdenes.</summary>
        public async Task<int> GetOrdersCountAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<int>("api/orders/count");
                return result;
            }
            catch
            {
                // Fallback: obtener todas las órdenes y contar
                var allOrders = await GetOrdersAsync();
                return allOrders.Count;
            }
        }

        /// <summary>Simula paginación de órdenes cuando el servidor no la soporta.</summary>
        private PagedResult<SalesOrder> SimulateServerPaginationOrders(List<SalesOrder> allOrders, PaginationParams paginationParams)
        {
            var filteredOrders = allOrders.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(paginationParams.Search))
            {
                var search = paginationParams.Search.ToLower();
                filteredOrders = filteredOrders.Where(o =>
                    (o.SalesOrderNumber?.ToLower().Contains(search) ?? false) ||
                    (o.PurchaseOrderNumber?.ToLower().Contains(search) ?? false) ||
                    (o.AccountNumber?.ToLower().Contains(search) ?? false) ||
                    o.SalesOrderId.ToString().Contains(search) ||
                    o.CustomerId.ToString().Contains(search));
            }
            if (!string.IsNullOrWhiteSpace(paginationParams.SortBy))
            {
                var isDescending = paginationParams.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);
                filteredOrders = paginationParams.SortBy.ToLower() switch
                {
                    "salesorderid" => isDescending ? filteredOrders.OrderByDescending(o => o.SalesOrderId) : filteredOrders.OrderBy(o => o.SalesOrderId),
                    "salesordernumber" => isDescending ? filteredOrders.OrderByDescending(o => o.SalesOrderNumber ?? string.Empty) : filteredOrders.OrderBy(o => o.SalesOrderNumber ?? string.Empty),
                    "orderdate" => isDescending ? filteredOrders.OrderByDescending(o => o.OrderDate) : filteredOrders.OrderBy(o => o.OrderDate),
                    "duedate" => isDescending ? filteredOrders.OrderByDescending(o => o.DueDate) : filteredOrders.OrderBy(o => o.DueDate),
                    "customerid" => isDescending ? filteredOrders.OrderByDescending(o => o.CustomerId) : filteredOrders.OrderBy(o => o.CustomerId),
                    "totaldue" => isDescending ? filteredOrders.OrderByDescending(o => o.TotalDue) : filteredOrders.OrderBy(o => o.TotalDue),
                    "status" => isDescending ? filteredOrders.OrderByDescending(o => o.Status) : filteredOrders.OrderBy(o => o.Status),
                    _ => isDescending ? filteredOrders.OrderByDescending(o => o.OrderDate) : filteredOrders.OrderBy(o => o.OrderDate)
                };
            }
            var totalCount = filteredOrders.Count();
            var orders = filteredOrders.Skip((paginationParams.Page - 1) * paginationParams.PageSize).Take(paginationParams.PageSize).ToList();
            return new PagedResult<SalesOrder>
            {
                Items = orders,
                TotalCount = totalCount,
                CurrentPage = paginationParams.Page,
                PageSize = paginationParams.PageSize
            };
        }

        /// <summary>Obtiene una orden por su ID.</summary>
        public async Task<SalesOrder?> GetOrderAsync(int id)
        {
            try { return await _httpClient.GetFromJsonAsync<SalesOrder>($"api/orders/{id}"); }
            catch { return null; }
        }

        /// <summary>Crea una nueva orden.</summary>
        public async Task<SalesOrder?> CreateOrderAsync(SalesOrder order)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/orders", order);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<SalesOrder>();
            }
            catch { return null; }
        }

        /// <summary>Actualiza una orden existente.</summary>
        public async Task<bool> UpdateOrderAsync(int id, SalesOrder order)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/orders/{id}", order);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"API Error Response: {errorContent}");
                }
                return response.IsSuccessStatusCode;
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception in UpdateOrderAsync: {ex.Message}");
                return false;
            }
        }

        /// <summary>Elimina una orden por su ID.</summary>
        public async Task<bool> DeleteOrderAsync(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/orders/{id}");
                return response.IsSuccessStatusCode;
            }
            catch { return false; }
        }

        /// <summary>Obtiene órdenes filtradas por cliente.</summary>
        public async Task<List<SalesOrder>> GetOrdersByCustomerAsync(int customerId)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<SalesOrder>>($"api/orders/customer/{customerId}");
                return result ?? new List<SalesOrder>();
            }
            catch { return new List<SalesOrder>(); }
        }

        /// <summary>Obtiene órdenes filtradas por estado.</summary>
        public async Task<List<SalesOrder>> GetOrdersByStatusAsync(byte status)
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<SalesOrder>>($"api/orders/status/{status}");
                return result ?? new List<SalesOrder>();
            }
            catch { return new List<SalesOrder>(); }
        }

        /// <summary>Obtiene productos con bajo inventario (expuesto vía OrdersController).</summary>
        public async Task<List<ProductoBajoInventarioDto>> GetProductosBajoInventarioAsync()
        {
            try
            {
                var result = await _httpClient.GetFromJsonAsync<List<ProductoBajoInventarioDto>>("api/orders/productos-bajo-inventario");
                return result ?? new List<ProductoBajoInventarioDto>();
            }
            catch { return new List<ProductoBajoInventarioDto>(); }
        }

        /// <summary>Alias para compatibilidad con componentes existentes.</summary>
        public Task<List<ProductoBajoInventarioDto>> GetProductLowInventoryAsync() => GetProductosBajoInventarioAsync();

        #endregion
    }
}