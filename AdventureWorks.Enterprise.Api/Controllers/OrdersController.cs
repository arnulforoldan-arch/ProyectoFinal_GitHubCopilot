using AdventureWorks.Enterprise.Api.Data;
using AdventureWorks.Enterprise.Api.Models;
using AdventureWorks.Enterprise.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace AdventureWorks.Enterprise.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestin de rdenes de venta.
    /// Proporciona operaciones CRUD y de consulta sobre la entidad SalesOrder.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly AdventureWorksDbContext _context;
        private readonly ProductoInventarioService _productoInventarioService;

        /// <summary>
        /// Inicializa una nueva instancia del controlador OrdersController.
        /// </summary>
        /// <param name="context">Contexto de base de datos inyectado.</param>
        /// <param name="productoInventarioService">Servicio para la gesti 1n de inventario de productos.</param>
        public OrdersController(AdventureWorksDbContext context, ProductoInventarioService productoInventarioService)
        {
            _context = context;
            _productoInventarioService = productoInventarioService;
        }

        /// <summary>
        /// Obtiene la lista de todas las rdenes de venta.
        /// </summary>
        /// <returns>Lista de rdenes de venta.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SalesOrder>>> GetOrders()
        {
            return await _context.SalesOrders.ToListAsync();
        }

        /// <summary>
        /// Obtiene una orden de venta por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la orden de venta.</param>
        /// <returns>Orden encontrada o NotFound si no existe.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<SalesOrder>> GetOrder(int id)
        {
            var order = await _context.SalesOrders.FindAsync(id);
            if (order == null)
                return NotFound();
            return order;
        }

        /// <summary>
        /// Crea una nueva orden de venta.
        /// Evita columnas calculadas (SalesOrderNumber, TotalDue) y usa SQL parametrizado.
        /// </summary>
        /// <param name="order">Datos de la orden a crear.</param>
        /// <returns>Orden creada.</returns>
        [HttpPost]
        public async Task<ActionResult<SalesOrder>> CreateOrder(SalesOrder order)
        {
            try
            {
                // Normalizar/validar datos de entrada bsicos
                if (order == null)
                    return BadRequest("Datos de orden no proporcionados.");

                if (order.CustomerId <= 0)
                    return BadRequest("El CustomerId debe ser mayor que 0.");

                if (order.OrderDate == default)
                    order.OrderDate = DateTime.Today;

                if (order.DueDate == default)
                    order.DueDate = order.OrderDate;

                if (order.DueDate < order.OrderDate)
                    return BadRequest("La fecha de vencimiento debe ser posterior o igual a la fecha de orden.");

                // En AdventureWorks el rango v 1alido suele ser 1..5
                if (order.Status < 1 || order.Status > 5)
                    order.Status = 1;

                // Evitar forzar columnas calculadas desde el cliente
                order.SalesOrderNumber = null;
                // TotalDue es calculada en DB (SubTotal + TaxAmt + Freight), se ignora el valor de entrada

                // Valorar OnlineOrderFlag (por defecto false si no llega del cliente)
                // Mantener el valor proporcionado si llega; no es columna calculada

                await using var connection = _context.Database.GetDbConnection();
                await connection.OpenAsync();

                // Resolver valores requeridos por defecto
                async Task<int?> GetScalarIntAsync(string sql)
                {
                    await using var tmp = connection.CreateCommand();
                    tmp.CommandText = sql;
                    var obj = await tmp.ExecuteScalarAsync();
                    if (obj == null || obj == DBNull.Value) return null;
                    return Convert.ToInt32(obj);
                }

                var billToId = await GetScalarIntAsync("SELECT TOP(1) [AddressID] FROM [Person].[Address] ORDER BY [AddressID]");
                var shipToId = billToId; // usar el mismo si no se especifica
                var shipMethodId = await GetScalarIntAsync("SELECT TOP(1) [ShipMethodID] FROM [Purchasing].[ShipMethod] ORDER BY [ShipMethodID]");

                if (billToId == null || shipToId == null || shipMethodId == null)
                {
                    return StatusCode(500, new { message = "No hay datos de Address o ShipMethod disponibles para crear la orden." });
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();

                var cmd = connection.CreateCommand();
                cmd.Transaction = (SqlTransaction?)transaction.GetDbTransaction();

                cmd.CommandText = @"
INSERT INTO [Sales].[SalesOrderHeader]
    ([OrderDate], [DueDate], [ShipDate], [Status], [OnlineOrderFlag],
     [PurchaseOrderNumber], [AccountNumber], [CustomerID], [SalesPersonID], [TerritoryID],
     [BillToAddressID], [ShipToAddressID], [ShipMethodID],
     [SubTotal], [TaxAmt], [Freight], [Comment], [CreditCardApprovalCode])
OUTPUT INSERTED.[SalesOrderID]
VALUES
    (@OrderDate, @DueDate, @ShipDate, @Status, @OnlineOrderFlag,
     @PurchaseOrderNumber, @AccountNumber, @CustomerID, @SalesPersonID, @TerritoryID,
     @BillToAddressID, @ShipToAddressID, @ShipMethodID,
     @SubTotal, @TaxAmt, @Freight, @Comment, @CreditCardApprovalCode);";

                // Agregar par 1ametros seguros
                cmd.Parameters.Add(new SqlParameter("@OrderDate", SqlDbType.DateTime2) { Value = order.OrderDate });
                cmd.Parameters.Add(new SqlParameter("@DueDate", SqlDbType.DateTime2) { Value = order.DueDate });
                cmd.Parameters.Add(new SqlParameter("@ShipDate", SqlDbType.DateTime2) { Value = (object?)order.ShipDate ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.TinyInt) { Value = order.Status });
                cmd.Parameters.Add(new SqlParameter("@OnlineOrderFlag", SqlDbType.Bit) { Value = order.OnlineOrderFlag });
                cmd.Parameters.Add(new SqlParameter("@PurchaseOrderNumber", SqlDbType.NVarChar, 25) { Value = (object?)order.PurchaseOrderNumber ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@AccountNumber", SqlDbType.NVarChar, 15) { Value = (object?)order.AccountNumber ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@CustomerID", SqlDbType.Int) { Value = order.CustomerId });
                cmd.Parameters.Add(new SqlParameter("@SalesPersonID", SqlDbType.Int)
                {
                    Value = (order.SalesPersonId.HasValue && order.SalesPersonId.Value > 0)
                        ? order.SalesPersonId.Value
                        : (object)DBNull.Value
                });
                cmd.Parameters.Add(new SqlParameter("@TerritoryID", SqlDbType.Int)
                {
                    Value = (order.TerritoryId.HasValue && order.TerritoryId.Value > 0)
                        ? order.TerritoryId.Value
                        : (object)DBNull.Value
                });
                cmd.Parameters.Add(new SqlParameter("@BillToAddressID", SqlDbType.Int) { Value = billToId.Value });
                cmd.Parameters.Add(new SqlParameter("@ShipToAddressID", SqlDbType.Int) { Value = shipToId.Value });
                cmd.Parameters.Add(new SqlParameter("@ShipMethodID", SqlDbType.Int) { Value = shipMethodId.Value });
                cmd.Parameters.Add(new SqlParameter("@SubTotal", SqlDbType.Money) { Value = order.SubTotal });
                cmd.Parameters.Add(new SqlParameter("@TaxAmt", SqlDbType.Money) { Value = order.TaxAmt });
                cmd.Parameters.Add(new SqlParameter("@Freight", SqlDbType.Money) { Value = order.Freight });
                cmd.Parameters.Add(new SqlParameter("@Comment", SqlDbType.NVarChar, 128) { Value = (object?)order.Comment ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@CreditCardApprovalCode", SqlDbType.NVarChar, 15) { Value = (object?)order.CreditCardApprovalCode ?? DBNull.Value });

                // Ejecutar y obtener el ID generado
                var newIdObj = await cmd.ExecuteScalarAsync();
                var newId = Convert.ToInt32(newIdObj);

                await transaction.CommitAsync();

                // Recuperar la entidad creada (incluye columnas calculadas)
                var created = await _context.SalesOrders.FindAsync(newId);
                if (created == null)
                {
                    // Como fallback, devolver un objeto con el ID
                    created = new SalesOrder
                    {
                        SalesOrderId = newId,
                        OrderDate = order.OrderDate,
                        DueDate = order.DueDate,
                        ShipDate = order.ShipDate,
                        Status = order.Status,
                        OnlineOrderFlag = order.OnlineOrderFlag,
                        PurchaseOrderNumber = order.PurchaseOrderNumber,
                        AccountNumber = order.AccountNumber,
                        CustomerId = order.CustomerId,
                        SalesPersonId = (order.SalesPersonId.HasValue && order.SalesPersonId.Value > 0) ? order.SalesPersonId : null,
                        TerritoryId = (order.TerritoryId.HasValue && order.TerritoryId.Value > 0) ? order.TerritoryId : null,
                        SubTotal = order.SubTotal,
                        TaxAmt = order.TaxAmt,
                        Freight = order.Freight,
                        Comment = order.Comment,
                        CreditCardApprovalCode = order.CreditCardApprovalCode
                    };
                }

                return CreatedAtAction(nameof(GetOrder), new { id = created.SalesOrderId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al crear la orden.", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza los datos de una orden de venta existente.
        /// </summary>
        /// <param name="id">Identificador de la orden de venta.</param>
        /// <param name="order">Datos actualizados de la orden.</param>
        /// <returns>NoContent si la actualizaci 1n fue exitosa, NotFound si no existe.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, SalesOrder order)
        {
            try
            {
                // Logging detallado para depuraci n
                System.Diagnostics.Debug.WriteLine($"UpdateOrder API called - ID: {id}");
                System.Diagnostics.Debug.WriteLine($"Order received - SalesOrderId: {order.SalesOrderId}");

                if (id != order.SalesOrderId)
                {
                    System.Diagnostics.Debug.WriteLine($"ID mismatch - Route: {id}, Order: {order.SalesOrderId}");
                    return BadRequest("El ID de la orden no coincide con el ID de la ruta.");
                }

                var existingOrder = await _context.SalesOrders.FindAsync(id);
                if (existingOrder == null)
                {
                    return NotFound("Orden de venta no encontrada.");
                }

                // Usar SQL crudo para evitar conflictos con triggers
                // Solo actualizar campos seguros para modificar
                var sql = @"
                    UPDATE [Sales].[SalesOrderHeader] 
                    SET [OrderDate] = {0}, 
                        [DueDate] = {1}, 
                        [ShipDate] = {2}, 
                        [Status] = {3}, 
                        [Comment] = {4}
                    WHERE [SalesOrderID] = {5}";

                var rowsAffected = await _context.Database.ExecuteSqlRawAsync(sql,
                    order.OrderDate,
                    order.DueDate,
                    order.ShipDate,
                    order.Status,
                    order.Comment ?? (object)DBNull.Value,
                    id);

                System.Diagnostics.Debug.WriteLine($"ExecuteSqlRaw completed - {rowsAffected} rows affected");

                if (rowsAffected > 0)
                {
                    return NoContent();
                }
                else
                {
                    return BadRequest("No se pudo actualizar la orden. Verifique que la orden existe.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General exception in UpdateOrder: {ex.Message}");
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Elimina una orden de venta por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la orden de venta.</param>
        /// <returns>NoContent si la operaci 1n fue exitosa, NotFound si no existe.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.SalesOrders.FindAsync(id);
            if (order == null)
                return NotFound();

            _context.SalesOrders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Obtiene todas las rdenes de venta de un cliente especfico.
        /// </summary>
        /// <param name="customerId">Identificador del cliente.</param>
        /// <returns>Lista de rdenes de venta del cliente.</returns>
        [HttpGet("customer/{customerId}")]
        public async Task<ActionResult<IEnumerable<SalesOrder>>> GetOrdersByCustomer(int customerId)
        {
            var orders = await _context.SalesOrders
                .Where(o => o.CustomerId == customerId)
                .ToListAsync();

            if (orders == null || orders.Count == 0)
                return NotFound();

            return orders;
        }

        /// <summary>
        /// Obtiene todas las rdenes de venta por estado.
        /// </summary>
        /// <param name="status">Estado de la orden.</param>
        /// <returns>Lista de rdenes de venta con el estado especificado.</returns>
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<SalesOrder>>> GetOrdersByStatus(byte status)
        {
            var orders = await _context.SalesOrders
                .Where(o => o.Status == status)
                .ToListAsync();

            if (orders == null || orders.Count == 0)
                return NotFound();

            return orders;
        }

        /// <summary>
        /// Verifica si existe una orden de venta por su identificador.
        /// </summary>
        /// <param name="id">Identificador de la orden de venta.</param>
        /// <returns>True si existe, false en caso contrario.</returns>
        private bool OrderExists(int id)
        {
            return _context.SalesOrders.Any(e => e.SalesOrderId == id);
        }

        /// <summary>
        /// Obtiene los productos con bajo inventario.
        /// </summary>
        /// <returns>Lista de productos con inventario por debajo del nivel de seguridad.</returns>
        [HttpGet("productos-bajo-inventario")]
        public async Task<ActionResult<IEnumerable<ProductoBajoInventarioDto>>> GetProductosBajoInventario()
        {
            var productos = await _productoInventarioService.ObtenerProductosBajoInventarioAsync();
            return Ok(productos);
        }

        /// <summary>
        /// Obtiene rdenes paginadas con opciones de b6squeda y ordenamiento
        /// </summary>
        /// <param name="page">N6mero de p61gina (basado en 1)</param>
        /// <param name="pageSize">Elementos por p61gina (m61ximo 100)</param>
        /// <param name="search">T69rmino de b6squeda opcional</param>
        /// <param name="sortBy">Campo de ordenamiento</param>
        /// <param name="sortDirection">Direcci63n del ordenamiento (asc/desc)</param>
        /// <returns>Resultado paginado con rdenes</returns>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<SalesOrder>>> GetOrdersPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "OrderDate",
            [FromQuery] string sortDirection = "desc")
        {
            try
            {
                // Validar par61metros
                var paginationParams = new PaginationParams
                {
                    Page = Math.Max(1, page),
                    PageSize = Math.Min(Math.Max(1, pageSize), 100),
                    Search = search,
                    SortBy = sortBy,
                    SortDirection = sortDirection
                };

                // Construir consulta base
                var query = _context.SalesOrders.AsQueryable();

                // Aplicar filtros de b6squeda
                if (!string.IsNullOrWhiteSpace(paginationParams.Search))
                {
                    var searchTerm = paginationParams.Search.ToLower();
                    query = query.Where(o =>
                        (o.SalesOrderNumber != null && o.SalesOrderNumber.ToLower().Contains(searchTerm)) ||
                        (o.PurchaseOrderNumber != null && o.PurchaseOrderNumber.ToLower().Contains(searchTerm)) ||
                        (o.AccountNumber != null && o.AccountNumber.ToLower().Contains(searchTerm)) ||
                        o.SalesOrderId.ToString().Contains(searchTerm) ||
                        o.CustomerId.ToString().Contains(searchTerm));
                }

                // Aplicar ordenamiento
                if (!string.IsNullOrWhiteSpace(paginationParams.SortBy))
                {
                    var isDescending = paginationParams.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

                    query = paginationParams.SortBy.ToLower() switch
                    {
                        "salesorderid" => isDescending
                            ? query.OrderByDescending(o => o.SalesOrderId)
                            : query.OrderBy(o => o.SalesOrderId),
                        "salesordernumber" => isDescending
                            ? query.OrderByDescending(o => o.SalesOrderNumber)
                            : query.OrderBy(o => o.SalesOrderNumber),
                        "orderdate" => isDescending
                            ? query.OrderByDescending(o => o.OrderDate)
                            : query.OrderBy(o => o.OrderDate),
                        "duedate" => isDescending
                            ? query.OrderByDescending(o => o.DueDate)
                            : query.OrderBy(o => o.DueDate),
                        "customerid" => isDescending
                            ? query.OrderByDescending(o => o.CustomerId)
                            : query.OrderBy(o => o.CustomerId),
                        "totaldue" => isDescending
                            ? query.OrderByDescending(o => o.TotalDue)
                            : query.OrderBy(o => o.TotalDue),
                        "status" => isDescending
                            ? query.OrderByDescending(o => o.Status)
                            : query.OrderBy(o => o.Status),
                        _ => isDescending
                            ? query.OrderByDescending(o => o.OrderDate)
                            : query.OrderBy(o => o.OrderDate)
                    };
                }
                else
                {
                    query = query.OrderByDescending(o => o.OrderDate);
                }

                // Obtener conteo total
                var totalCount = await query.CountAsync();

                // Aplicar paginaci63n
                var orders = await query
                    .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
                    .Take(paginationParams.PageSize)
                    .ToListAsync();

                var result = new PagedResult<SalesOrder>
                {
                    Items = orders,
                    TotalCount = totalCount,
                    CurrentPage = paginationParams.Page,
                    PageSize = paginationParams.PageSize
                };

                // Agregar headers de paginaci63n para APIs RESTful
                Response.Headers.Add("X-Pagination-TotalCount", result.TotalCount.ToString());
                Response.Headers.Add("X-Pagination-TotalPages", result.TotalPages.ToString());
                Response.Headers.Add("X-Pagination-CurrentPage", result.CurrentPage.ToString());
                Response.Headers.Add("X-Pagination-PageSize", result.PageSize.ToString());
                Response.Headers.Add("X-Pagination-HasNext", result.HasNextPage.ToString());
                Response.Headers.Add("X-Pagination-HasPrevious", result.HasPreviousPage.ToString());

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log del error (aqu6d podr6as usar ILogger)
                return StatusCode(500, new
                {
                    message = "Error interno del servidor al obtener rdenes paginadas",
                    error = ex.Message
                });
            }
        }

        /// <summary>
        /// Obtiene el conteo total de rdenes
        /// </summary>
        /// <returns>N6mero total de rdenes</returns>
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetOrdersCount()
        {
            try
            {
                var count = await _context.SalesOrders.CountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error al obtener el conteo de rdenes",
                    error = ex.Message
                });
            }
        }
    }
}