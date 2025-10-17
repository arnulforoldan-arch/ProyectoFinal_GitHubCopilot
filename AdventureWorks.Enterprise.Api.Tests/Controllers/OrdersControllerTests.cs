using AdventureWorks.Enterprise.Api.Controllers;
using AdventureWorks.Enterprise.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AdventureWorks.Enterprise.Api.Services;
using Microsoft.Extensions.Configuration;
using AdventureWorks.Enterprise.Api.Models; // agregar para PagedResult

namespace AdventureWorks.Enterprise.Api.Tests.Controllers
{
    /// <summary>
    /// Pruebas unitarias para el controlador de órdenes de venta.
    /// Incluye casos de éxito y fallo para todos los métodos públicos.
    /// </summary>
    public class OrdersControllerTests
    {
        private readonly DbContextOptions<AdventureWorksDbContext> dbOptions;
        private readonly IConfiguration config;

        /// <summary>
        /// Constructor de la clase de pruebas. Configura la base de datos en memoria y la configuración mínima.
        /// </summary>
        public OrdersControllerTests()
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
        /// Obtiene una instancia simulada del servicio de inventario de productos.
        /// </summary>
        private ProductoInventarioService GetProductoInventarioServiceMock()
        {
            return new ProductoInventarioService(config);
        }

        #region Pruebas de métodos públicos

        /// <summary>
        /// Prueba que retorna todas las órdenes correctamente.
        /// </summary>
        [Fact]
        public async Task GetOrders_ReturnsAllOrders()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            context.SalesOrders.AddRange(
                new SalesOrder { SalesOrderId = 1, CustomerId = 1, Status = 1 },
                new SalesOrder { SalesOrderId = 2, CustomerId = 2, Status = 2 }
            );
            await context.SaveChangesAsync();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);

            // Act
            var result = await controller.GetOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var orders = Assert.IsAssignableFrom<IEnumerable<SalesOrder>>(okResult.Value);
            Assert.Equal(2, orders.Count());
        }

        /// <summary>
        /// Prueba que retorna una orden específica por su ID.
        /// </summary>
        [Fact]
        public async Task GetOrder_WithValidId_ReturnsOrder()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var orderId = 1;
            context.SalesOrders.Add(new SalesOrder { SalesOrderId = orderId, CustomerId = 1, Status = 1 });
            await context.SaveChangesAsync();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);

            // Act
            var result = await controller.GetOrder(orderId);

            // Assert
            var okResult = Assert.IsType<ActionResult<SalesOrder>>(result);
            var order = Assert.IsType<SalesOrder>(okResult.Value);
            Assert.Equal(orderId, order.SalesOrderId);
        }

        /// <summary>
        /// Prueba que intenta obtener una orden con un ID inválido y recibe NotFound.
        /// </summary>
        [Fact]
        public async Task GetOrder_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);

            // Act
            var result = await controller.GetOrder(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Prueba que elimina una orden existente y retorna NoContent.
        /// </summary>
        [Fact]
        public async Task DeleteOrder_ReturnsNoContent()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);
            var order = new SalesOrder { SalesOrderId = 1, CustomerId = 1, Status = 1 };
            context.SalesOrders.Add(order);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.DeleteOrder(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Prueba que intenta eliminar una orden con un ID inválido y recibe NotFound.
        /// </summary>
        [Fact]
        public async Task DeleteOrder_ReturnsNotFound()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);

            // Act
            var result = await controller.DeleteOrder(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Prueba que intenta crear una orden con un objeto nulo y recibe BadRequest.
        /// </summary>
        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenOrderIsNull()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);

            // Act
            var result = await controller.CreateOrder(null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Prueba que intenta crear una orden con un CustomerId inválido y recibe BadRequest.
        /// </summary>
        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenCustomerIdIsInvalid()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);
            var order = new SalesOrder { CustomerId = 0 };

            // Act
            var result = await controller.CreateOrder(order);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Prueba que intenta crear una orden con una fecha de vencimiento anterior a la fecha de orden y recibe BadRequest.
        /// </summary>
        [Fact]
        public async Task CreateOrder_ReturnsBadRequest_WhenDueDateIsBeforeOrderDate()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);
            var order = new SalesOrder { CustomerId = 1, OrderDate = DateTime.Today, DueDate = DateTime.Today.AddDays(-1) };

            // Act
            var result = await controller.CreateOrder(order);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        /// <summary>
        /// Prueba que intenta actualizar una orden con un ID diferente en la URL y el objeto, y recibe BadRequest.
        /// </summary>
        [Fact]
        public async Task UpdateOrder_ReturnsBadRequest_OnIdMismatch()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);
            var order = new SalesOrder { SalesOrderId = 2 };

            // Act
            var result = await controller.UpdateOrder(1, order);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        }

        /// <summary>
        /// Prueba que intenta actualizar una orden que no existe en la base de datos y recibe NotFound.
        /// </summary>
        [Fact]
        public async Task UpdateOrder_ReturnsNotFound_OnMissingOrder()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);
            var order = new SalesOrder { SalesOrderId = 1 };

            // Act
            var result = await controller.UpdateOrder(1, order);

            // Assert
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
        }

        /// <summary>
        /// Prueba que retorna un resultado paginado de órdenes.
        /// </summary>
        [Fact]
        public async Task GetOrdersPaged_ReturnsPagedResult()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            for (int i = 1; i <= 20; i++)
            {
                context.SalesOrders.Add(new SalesOrder { SalesOrderId = i, CustomerId = 1, Status = 1 });
            }
            await context.SaveChangesAsync();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);

            // Act
            var result = await controller.GetOrdersPaged(2, 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var pagedResult = Assert.IsType<PagedResult<SalesOrder>>(okResult.Value);
            Assert.Equal(5, pagedResult.Items.Count());
            Assert.Equal(20, pagedResult.TotalCount);
            Assert.Equal(2, pagedResult.CurrentPage);
        }

        /// <summary>
        /// Prueba que retorna el conteo total de órdenes.
        /// </summary>
        [Fact]
        public async Task GetOrdersCount_ReturnsCount()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            context.SalesOrders.Add(new SalesOrder { SalesOrderId = 1, CustomerId = 1, Status = 1 });
            await context.SaveChangesAsync();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);

            // Act
            var result = await controller.GetOrdersCount();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var count = Assert.IsType<int>(okResult.Value);
            Assert.Equal(1, count);
        }

        /// <summary>
        /// Prueba que retorna las órdenes de un cliente específico.
        /// </summary>
        [Fact]
        public async Task GetOrdersByCustomer_ReturnsOrders()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            context.SalesOrders.Add(new SalesOrder { SalesOrderId = 1, CustomerId = 99, Status = 1 });
            await context.SaveChangesAsync();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);

            // Act
            var result = await controller.GetOrdersByCustomer(99);

            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<SalesOrder>>>(result);
            var orders = Assert.IsAssignableFrom<IEnumerable<SalesOrder>>(okResult.Value);
            Assert.Single(orders);
        }

        /// <summary>
        /// Prueba que intenta obtener órdenes de un cliente inexistente y recibe NotFound.
        /// </summary>
        [Fact]
        public async Task GetOrdersByCustomer_ReturnsNotFound()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);

            // Act
            var result = await controller.GetOrdersByCustomer(999);

            // Assert
            var notFound = Assert.IsType<ActionResult<IEnumerable<SalesOrder>>>(result);
            Assert.Null(notFound.Value);
        }

        /// <summary>
        /// Prueba que retorna las órdenes con un estado específico.
        /// </summary>
        [Fact]
        public async Task GetOrdersByStatus_ReturnsOrders()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            context.SalesOrders.Add(new SalesOrder { SalesOrderId = 1, CustomerId = 1, Status = 5 });
            await context.SaveChangesAsync();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);

            // Act
            var result = await controller.GetOrdersByStatus(5);

            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<SalesOrder>>>(result);
            var orders = Assert.IsAssignableFrom<IEnumerable<SalesOrder>>(okResult.Value);
            Assert.Single(orders);
        }

        /// <summary>
        /// Prueba que intenta obtener órdenes con un estado inexistente y recibe NotFound.
        /// </summary>
        [Fact]
        public async Task GetOrdersByStatus_ReturnsNotFound()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);

            // Act
            var result = await controller.GetOrdersByStatus(99);

            // Assert
            var notFound = Assert.IsType<ActionResult<IEnumerable<SalesOrder>>>(result);
            Assert.Null(notFound.Value);
        }

        /// <summary>
        /// Prueba que retorna productos con bajo inventario.
        /// </summary>
        [Fact]
        public async Task GetProductosBajoInventario_ReturnsOk()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoInventarioService = GetProductoInventarioServiceMock();
            var controller = new OrdersController(context, productoInventarioService);

            // Act
            var result = await controller.GetProductosBajoInventario();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
        }

        #endregion
    }
}