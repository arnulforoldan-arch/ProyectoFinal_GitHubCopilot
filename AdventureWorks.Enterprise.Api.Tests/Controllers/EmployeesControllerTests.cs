using AdventureWorks.Enterprise.Api.Controllers;
using AdventureWorks.Enterprise.Api.Data;
using AdventureWorks.Enterprise.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AdventureWorks.Enterprise.Api.Services;
using Microsoft.Extensions.Configuration;

namespace AdventureWorks.Enterprise.Api.Tests.Controllers
{
    /// <summary>
    /// Pruebas unitarias para el controlador de empleados.
    /// Incluye casos de éxito y fallo para todos los métodos públicos.
    /// </summary>
    public class EmployeesControllerTests
    {
        private readonly DbContextOptions<AdventureWorksDbContext> dbOptions;
        private readonly IConfiguration config;

        /// <summary>
        /// Constructor de la clase de pruebas. Configura la base de datos en memoria y la configuración mínima.
        /// </summary>
        public EmployeesControllerTests()
        {
            dbOptions = new DbContextOptionsBuilder<AdventureWorksDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var configBuilder = new ConfigurationBuilder();
            config = configBuilder.Build();
        }

        /// <summary>
        /// Obtiene una instancia del contexto de base de datos en memoria.
        /// </summary>
        private AdventureWorksDbContext GetDatabaseContext()
        {
            var context = new AdventureWorksDbContext(dbOptions);
            context.Database.EnsureCreated();
            return context;
        }

        /// <summary>
        /// Obtiene una instancia simulada del servicio de empleados.
        /// </summary>
        private EmpleadoService GetEmpleadoServiceMock()
        {
            return new EmpleadoService(config);
        }

        #region Pruebas de métodos públicos

        /// <summary>
        /// Prueba que retorna todos los empleados activos correctamente.
        /// </summary>
        [Fact]
        public async Task GetEmployees_ReturnsAllActiveEmployees()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            context.Employees.AddRange(
                new Employee { EmployeeId = 1, LoginId = "adventure-works\\ken0", CurrentFlag = true },
                new Employee { EmployeeId = 2, LoginId = "adventure-works\\terri0", CurrentFlag = true },
                new Employee { EmployeeId = 3, LoginId = "adventure-works\\david0", CurrentFlag = false }
            );
            await context.SaveChangesAsync();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);

            // Act
            var result = await controller.GetEmployees();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var employees = Assert.IsAssignableFrom<IEnumerable<Employee>>(okResult.Value);
            Assert.Equal(2, employees.Count());
        }

        /// <summary>
        /// Prueba que retorna un empleado válido por su ID.
        /// </summary>
        [Fact]
        public async Task GetEmployee_WithValidId_ReturnsEmployee()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var employeeId = 1;
            context.Employees.Add(new Employee { EmployeeId = employeeId, LoginId = "adventure-works\\ken0", CurrentFlag = true });
            await context.SaveChangesAsync();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);

            // Act
            var result = await controller.GetEmployee(employeeId);

            // Assert
            var okResult = Assert.IsType<ActionResult<Employee>>(result);
            var employee = Assert.IsType<Employee>(okResult.Value);
            Assert.Equal(employeeId, employee.EmployeeId);
        }

        /// <summary>
        /// Prueba que retorna NotFound cuando el ID no existe.
        /// </summary>
        [Fact]
        public async Task GetEmployee_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);

            // Act
            var result = await controller.GetEmployee(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Prueba que retorna NotFound cuando el empleado está inactivo.
        /// </summary>
        [Fact]
        public async Task GetEmployee_WithInactiveEmployeeId_ReturnsNotFound()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var employeeId = 1;
            context.Employees.Add(new Employee { EmployeeId = employeeId, LoginId = "adventure-works\\inactive0", CurrentFlag = false });
            await context.SaveChangesAsync();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);

            // Act
            var result = await controller.GetEmployee(employeeId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Prueba la paginación de empleados activos.
        /// </summary>
        [Fact]
        public async Task GetEmployeesPaged_ReturnsPagedResult()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            for (int i = 1; i <= 20; i++)
            {
                context.Employees.Add(new Employee { EmployeeId = i, LoginId = $"user{i}", CurrentFlag = true });
            }
            await context.SaveChangesAsync();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);
            var paginationParams = new PaginationParams { Page = 2, PageSize = 5 };

            // Act
            var result = await controller.GetEmployeesPaged(paginationParams.Page, paginationParams.PageSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagedResult = Assert.IsType<PagedResult<Employee>>(okResult.Value);
            Assert.Equal(5, pagedResult.Items.Count());
            Assert.Equal(20, pagedResult.TotalCount);
            Assert.Equal(2, pagedResult.CurrentPage);
        }

        /// <summary>
        /// Prueba que retorna el conteo de empleados activos.
        /// </summary>
        [Fact]
        public async Task GetEmployeesCount_ReturnsCount()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            context.Employees.Add(new Employee { EmployeeId = 1, LoginId = "user1", CurrentFlag = true });
            context.Employees.Add(new Employee { EmployeeId = 2, LoginId = "user2", CurrentFlag = false });
            await context.SaveChangesAsync();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);

            // Act
            var result = await controller.GetEmployeesCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var count = Assert.IsType<int>(okResult.Value);
            Assert.Equal(1, count);
        }

        /// <summary>
        /// Prueba que retorna un error de servidor al obtener el conteo de empleados.
        /// </summary>
        [Fact]
        public async Task GetEmployeesCount_ReturnsServerError()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);
            context.Dispose(); // Forzar error

            // Act
            var result = await controller.GetEmployeesCount();

            // Assert
            var statusResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, statusResult.StatusCode);
        }

        /// <summary>
        /// Prueba que crea un empleado correctamente.
        /// </summary>
        [Fact]
        public async Task CreateEmployee_ReturnsCreated()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);
            var employee = new Employee
            {
                NationalIdNumber = "123456789",
                LoginId = "user1",
                JobTitle = "Developer",
                BirthDate = DateTime.Today.AddYears(-30),
                MaritalStatus = "S",
                Gender = "M",
                HireDate = DateTime.Today.AddYears(-1),
                SalariedFlag = true,
                VacationHours = 10,
                SickLeaveHours = 5,
                CurrentFlag = true
            };

            // Act
            var result = await controller.CreateEmployee(employee);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdEmployee = Assert.IsType<Employee>(createdResult.Value);
            Assert.Equal("user1", createdEmployee.LoginId);
        }

        /// <summary>
        /// Prueba que retorna BadRequest por error de validación al crear un empleado.
        /// </summary>
        [Fact]
        public async Task CreateEmployee_ReturnsBadRequest_OnValidationError()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);
            var employee = new Employee
            {
                NationalIdNumber = "123456789",
                LoginId = "user1",
                JobTitle = "Developer",
                BirthDate = DateTime.Today,
                MaritalStatus = "X", // Estado civil inválido
                Gender = "Z", // Género inválido
                HireDate = DateTime.Today.AddYears(1), // Fecha futura
                SalariedFlag = true,
                VacationHours = 10,
                SickLeaveHours = 5,
                CurrentFlag = true
            };

            // Act
            var result = await controller.CreateEmployee(employee);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Prueba que retorna Conflict al intentar crear un empleado duplicado.
        /// </summary>
        [Fact]
        public async Task CreateEmployee_ReturnsConflict_OnDuplicate()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            context.Employees.Add(new Employee { NationalIdNumber = "123456789", LoginId = "user1", CurrentFlag = true });
            await context.SaveChangesAsync();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);
            var employee = new Employee
            {
                NationalIdNumber = "123456789",
                LoginId = "user1",
                JobTitle = "Developer",
                BirthDate = DateTime.Today.AddYears(-30),
                MaritalStatus = "S",
                Gender = "M",
                HireDate = DateTime.Today.AddYears(-1),
                SalariedFlag = true,
                VacationHours = 10,
                SickLeaveHours = 5,
                CurrentFlag = true
            };

            // Act
            var result = await controller.CreateEmployee(employee);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result.Result);
        }

        /// <summary>
        /// Prueba que actualiza un empleado correctamente.
        /// </summary>
        [Fact]
        public async Task UpdateEmployee_ReturnsNoContent()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);
            var employee = new Employee
            {
                EmployeeId = 1,
                NationalIdNumber = "123456789",
                LoginId = "user1",
                JobTitle = "Developer",
                BirthDate = DateTime.Today.AddYears(-30),
                MaritalStatus = "S",
                Gender = "M",
                HireDate = DateTime.Today.AddYears(-1),
                SalariedFlag = true,
                VacationHours = 10,
                SickLeaveHours = 5,
                CurrentFlag = true
            };
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            // Act
            employee.JobTitle = "Senior Developer";
            var result = await controller.UpdateEmployee(1, employee);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Prueba que retorna BadRequest por desacuerdo de ID en la actualización.
        /// </summary>
        [Fact]
        public async Task UpdateEmployee_ReturnsBadRequest_OnIdMismatch()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);
            var employee = new Employee { EmployeeId = 2 };

            // Act
            var result = await controller.UpdateEmployee(1, employee);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        }

        /// <summary>
        /// Prueba que retorna NotFound al actualizar un empleado que no existe.
        /// </summary>
        [Fact]
        public async Task UpdateEmployee_ReturnsNotFound_OnMissingEmployee()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);
            var employee = new Employee { EmployeeId = 1, CurrentFlag = true };

            // Act
            var result = await controller.UpdateEmployee(1, employee);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
        }

        /// <summary>
        /// Prueba que elimina un empleado correctamente.
        /// </summary>
        [Fact]
        public async Task DeleteEmployee_ReturnsNoContent()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);
            var employee = new Employee { EmployeeId = 1, LoginId = "user1", CurrentFlag = true };
            context.Employees.Add(employee);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.DeleteEmployee(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Prueba que retorna NotFound al intentar eliminar un empleado que no existe.
        /// </summary>
        [Fact]
        public async Task DeleteEmployee_ReturnsNotFound()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);

            // Act
            var result = await controller.DeleteEmployee(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Prueba que retorna los empleados con más tiempo en la empresa.
        /// </summary>
        [Fact]
        public async Task GetEmpleadosMasTiempo_ReturnsOk()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var empleadoService = GetEmpleadoServiceMock();
            var controller = new EmployeesController(context, empleadoService);

            // Act
            var result = await controller.GetEmpleadosMasTiempo();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
        }

        #endregion
    }
}