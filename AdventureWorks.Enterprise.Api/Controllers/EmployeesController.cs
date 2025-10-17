using AdventureWorks.Enterprise.Api.Data;
using AdventureWorks.Enterprise.Api.Models;
using AdventureWorks.Enterprise.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdventureWorks.Enterprise.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly AdventureWorksDbContext _context;
        private readonly EmpleadoService _empleadoService;

        public EmployeesController(AdventureWorksDbContext context, EmpleadoService empleadoService)
        {
            _context = context;
            _empleadoService = empleadoService;
        }

        /// <summary>
        /// Obtiene la lista de todos los empleados.
        /// </summary>
        /// <returns>Lista de empleados.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
        {
            return await _context.Employees.Where(e => e.CurrentFlag).ToListAsync();
        }

        /// <summary>
        /// Obtiene empleados paginados con opciones de búsqueda y ordenamiento
        /// </summary>
        /// <param name="page">Número de página (basado en 1)</param>
        /// <param name="pageSize">Elementos por página (máximo 100)</param>
        /// <param name="search">Término de búsqueda opcional</param>
        /// <param name="sortBy">Campo de ordenamiento</param>
        /// <param name="sortDirection">Dirección del ordenamiento (asc/desc)</param>
        /// <returns>Resultado paginado con empleados</returns>
        [HttpGet("paged")]
        public async Task<ActionResult<PagedResult<Employee>>> GetEmployeesPaged(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = "EmployeeId",
            [FromQuery] string sortDirection = "asc")
        {
            try
            {
                // Validar parámetros
                var paginationParams = new PaginationParams
                {
                    Page = Math.Max(1, page),
                    PageSize = Math.Min(Math.Max(1, pageSize), 100),
                    Search = search,
                    SortBy = sortBy,
                    SortDirection = sortDirection
                };

                // Construir consulta base - solo empleados activos
                var query = _context.Employees.Where(e => e.CurrentFlag);

                // Aplicar filtros de búsqueda
                if (!string.IsNullOrWhiteSpace(paginationParams.Search))
                {
                    var searchTerm = paginationParams.Search.ToLower();
                    query = query.Where(e => 
                        e.LoginId.ToLower().Contains(searchTerm) ||
                        e.JobTitle.ToLower().Contains(searchTerm) ||
                        e.NationalIdNumber.Contains(searchTerm) ||
                        e.EmployeeId.ToString().Contains(searchTerm));
                }

                // Aplicar ordenamiento
                if (!string.IsNullOrWhiteSpace(paginationParams.SortBy))
                {
                    var isDescending = paginationParams.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase);

                    query = paginationParams.SortBy.ToLower() switch
                    {
                        "employeeid" => isDescending 
                            ? query.OrderByDescending(e => e.EmployeeId)
                            : query.OrderBy(e => e.EmployeeId),
                        "loginid" => isDescending
                            ? query.OrderByDescending(e => e.LoginId)
                            : query.OrderBy(e => e.LoginId),
                        "jobtitle" => isDescending
                            ? query.OrderByDescending(e => e.JobTitle)
                            : query.OrderBy(e => e.JobTitle),
                        "hiredate" => isDescending
                            ? query.OrderByDescending(e => e.HireDate)
                            : query.OrderBy(e => e.HireDate),
                        "birthdate" => isDescending
                            ? query.OrderByDescending(e => e.BirthDate)
                            : query.OrderBy(e => e.BirthDate),
                        "nationalidnumber" => isDescending
                            ? query.OrderByDescending(e => e.NationalIdNumber)
                            : query.OrderBy(e => e.NationalIdNumber),
                        _ => isDescending
                            ? query.OrderByDescending(e => e.EmployeeId)
                            : query.OrderBy(e => e.EmployeeId)
                    };
                }
                else
                {
                    query = query.OrderBy(e => e.EmployeeId);
                }

                // Obtener conteo total
                var totalCount = await query.CountAsync();

                // Aplicar paginación
                var employees = await query
                    .Skip((paginationParams.Page - 1) * paginationParams.PageSize)
                    .Take(paginationParams.PageSize)
                    .ToListAsync();

                var result = new PagedResult<Employee>
                {
                    Items = employees,
                    TotalCount = totalCount,
                    CurrentPage = paginationParams.Page,
                    PageSize = paginationParams.PageSize
                };

                // Agregar headers de paginación para APIs RESTful
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
                // Log del error (aquí podrías usar ILogger)
                return StatusCode(500, new { 
                    message = "Error interno del servidor al obtener empleados paginados", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Obtiene el conteo total de empleados activos
        /// </summary>
        /// <returns>Número total de empleados</returns>
        [HttpGet("count")]
        public async Task<ActionResult<int>> GetEmployeesCount()
        {
            try
            {
                var count = await _context.Employees.CountAsync(e => e.CurrentFlag);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    message = "Error al obtener el conteo de empleados", 
                    error = ex.Message 
                });
            }
        }

        /// <summary>
        /// Obtiene un empleado por su identificador.
        /// </summary>
        /// <param name="id">Identificador del empleado.</param>
        /// <returns>Empleado encontrado o NotFound si no existe.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null || !employee.CurrentFlag)
                return NotFound();
            return employee;
        }

        /// <summary>
        /// Crea un nuevo empleado siguiendo el modelo AdventureWorks:
        /// 1) Inserta BusinessEntity
        /// 2) Inserta Person.Person (mínimo requerido)
        /// 3) Inserta HumanResources.Employee
        /// </summary>
        /// <param name="employee">Datos del empleado a crear.</param>
        /// <returns>Empleado creado.</returns>
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
        {
            // Validaciones de dominio AdventureWorks
            var errors = new List<string>();
            if (string.IsNullOrWhiteSpace(employee.MaritalStatus) || employee.MaritalStatus.Length != 1 ||
                !(new[] { "S", "M", "D", "W" }.Contains(employee.MaritalStatus)))
                errors.Add("Estado Civil inválido. Use S/M/D/W.");
            if (string.IsNullOrWhiteSpace(employee.Gender) || employee.Gender.Length != 1 ||
                !(new[] { "M", "F" }.Contains(employee.Gender)))
                errors.Add("Género inválido. Use M/F.");
            if (employee.NationalIdNumber?.Length > 15) errors.Add("ID Nacional no puede exceder 15 caracteres.");
            if (employee.LoginId?.Length > 256) errors.Add("Login ID no puede exceder 256 caracteres.");
            if (employee.JobTitle?.Length > 50) errors.Add("Cargo no puede exceder 50 caracteres.");
            if (employee.HireDate.Date > DateTime.Today) errors.Add("La Fecha de Contratación no puede ser futura.");
            if (employee.BirthDate.Date > DateTime.Today.AddYears(-18)) errors.Add("El empleado debe ser mayor de 18 años.");
            if (errors.Count > 0) return BadRequest(new { message = string.Join(" ", errors) });

            // Duplicados (evita violar claves únicas)
            var duplicate = await _context.Employees.AnyAsync(e =>
                e.NationalIdNumber == employee.NationalIdNumber || e.LoginId == employee.LoginId);
            if (duplicate)
                return Conflict(new { message = "Ya existe un empleado con el mismo ID Nacional o Login ID." });

            employee.CurrentFlag = true;

            // Abrir conexión y transacción ADO.NET sobre el mismo connection del DbContext
            var connection = (SqlConnection)_context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            await using var dbTransaction = await connection.BeginTransactionAsync();
            try
            {
                // 1) BusinessEntity
                var insertBeCmd = new SqlCommand(@"
                    INSERT INTO Person.BusinessEntity (rowguid, ModifiedDate)
                    VALUES (NEWID(), GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() AS int);
                ", connection, (SqlTransaction)dbTransaction);

                var newBusinessEntityIdObj = await insertBeCmd.ExecuteScalarAsync();
                var newBusinessEntityId = Convert.ToInt32(newBusinessEntityIdObj);

                // 2) Person.Person (mínimo requerido)
                var firstName = string.IsNullOrWhiteSpace(employee.LoginId) ? "Usuario" : employee.LoginId.Trim();
                if (firstName.Length > 50) firstName = firstName[..50];
                var lastName = "Empleado";

                var insertPersonCmd = new SqlCommand(@"
                    INSERT INTO Person.Person (
                        BusinessEntityID, PersonType, NameStyle, Title, FirstName, MiddleName, LastName,
                        Suffix, EmailPromotion, rowguid, ModifiedDate
                    ) VALUES (
                        @BusinessEntityID, 'EM', 0, NULL, @FirstName, NULL, @LastName,
                        NULL, 0, NEWID(), GETDATE()
                    );
                ", connection, (SqlTransaction)dbTransaction);
                insertPersonCmd.Parameters.AddWithValue("@BusinessEntityID", newBusinessEntityId);
                insertPersonCmd.Parameters.AddWithValue("@FirstName", firstName);
                insertPersonCmd.Parameters.AddWithValue("@LastName", lastName);
                await insertPersonCmd.ExecuteNonQueryAsync();

                // 3) HumanResources.Employee
                var insertEmpCmd = new SqlCommand(@"
                    INSERT INTO HumanResources.Employee (
                        BusinessEntityID, NationalIDNumber, LoginID, JobTitle, BirthDate,
                        MaritalStatus, Gender, HireDate, SalariedFlag, VacationHours,
                        SickLeaveHours, CurrentFlag, rowguid, ModifiedDate
                    ) VALUES (
                        @BusinessEntityID, @NationalIDNumber, @LoginID, @JobTitle, @BirthDate,
                        @MaritalStatus, @Gender, @HireDate, @SalariedFlag, @VacationHours,
                        @SickLeaveHours, @CurrentFlag, NEWID(), GETDATE()
                    );
                ", connection, (SqlTransaction)dbTransaction);

                insertEmpCmd.Parameters.AddWithValue("@BusinessEntityID", newBusinessEntityId);
                insertEmpCmd.Parameters.AddWithValue("@NationalIDNumber", employee.NationalIdNumber);
                insertEmpCmd.Parameters.AddWithValue("@LoginID", employee.LoginId);
                insertEmpCmd.Parameters.AddWithValue("@JobTitle", employee.JobTitle);
                insertEmpCmd.Parameters.AddWithValue("@BirthDate", employee.BirthDate.Date);
                insertEmpCmd.Parameters.AddWithValue("@MaritalStatus", employee.MaritalStatus);
                insertEmpCmd.Parameters.AddWithValue("@Gender", employee.Gender);
                insertEmpCmd.Parameters.AddWithValue("@HireDate", employee.HireDate.Date);
                insertEmpCmd.Parameters.AddWithValue("@SalariedFlag", employee.SalariedFlag);
                insertEmpCmd.Parameters.AddWithValue("@VacationHours", employee.VacationHours);
                insertEmpCmd.Parameters.AddWithValue("@SickLeaveHours", employee.SickLeaveHours);
                insertEmpCmd.Parameters.AddWithValue("@CurrentFlag", employee.CurrentFlag);

                await insertEmpCmd.ExecuteNonQueryAsync();

                await dbTransaction.CommitAsync();

                employee.EmployeeId = newBusinessEntityId;
                return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeId }, employee);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return StatusCode(500, new { message = "Error al crear el empleado.", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza los datos de un empleado existente.
        /// </summary>
        /// <param name="id">Identificador del empleado.</param>
        /// <param name="employee">Datos actualizados del empleado.</param>
        /// <returns>NoContent si la actualización fue exitosa, NotFound si no existe.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
                return BadRequest("El ID del empleado no coincide con el ID de la ruta.");

            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null || !existingEmployee.CurrentFlag)
                return NotFound("Empleado no encontrado o no activo.");

            // Actualizar solo los campos que pueden ser modificados
            existingEmployee.NationalIdNumber = employee.NationalIdNumber;
            existingEmployee.LoginId = employee.LoginId;
            existingEmployee.JobTitle = employee.JobTitle;
            existingEmployee.BirthDate = employee.BirthDate;
            existingEmployee.MaritalStatus = employee.MaritalStatus;
            existingEmployee.Gender = employee.Gender;
            existingEmployee.HireDate = employee.HireDate;
            existingEmployee.SalariedFlag = employee.SalariedFlag;
            existingEmployee.VacationHours = employee.VacationHours;
            existingEmployee.SickLeaveHours = employee.SickLeaveHours;
            existingEmployee.CurrentFlag = employee.CurrentFlag;

            try
            {
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!EmployeeExists(id))
                    return NotFound("El empleado ya no existe.");
                else
                    return Conflict($"Error de concurrencia: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Marca un empleado como no actual (no lo elimina físicamente).
        /// </summary>
        /// <param name="id">Identificador del empleado.</param>
        /// <returns>NoContent si la operación fue exitosa, NotFound si no existe.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound();

            employee.CurrentFlag = false;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Obtiene los empleados con más tiempo en la empresa.
        /// </summary>
        /// <returns>Lista de empleados con más antigüedad.</returns>
        [HttpGet("mas-tiempo")]
        public async Task<ActionResult<IEnumerable<EmpleadoMasTiempoDto>>> GetEmpleadosMasTiempo()
        {
            var empleados = await _empleadoService.ObtenerEmpleadosMasTiempoAsync();
            return Ok(empleados);
        }

        /// <summary>
        /// Verifica si existe un empleado por su identificador.
        /// </summary>
        /// <param name="id">Identificador del empleado.</param>
        /// <returns>True si existe, false en caso contrario.</returns>
        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeId == id && e.CurrentFlag);
        }
    }
}