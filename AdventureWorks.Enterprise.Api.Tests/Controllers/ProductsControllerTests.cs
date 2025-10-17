using AdventureWorks.Enterprise.Api.Controllers;
using AdventureWorks.Enterprise.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using AdventureWorks.Enterprise.Api.Services;
using Microsoft.Extensions.Configuration;

namespace AdventureWorks.Enterprise.Api.Tests.Controllers
{
    /// <summary>
    /// Pruebas unitarias para el controlador de productos.
    /// Incluye casos de éxito y fallo para todos los métodos públicos.
    /// </summary>
    public class ProductsControllerTests
    {
        private readonly DbContextOptions<AdventureWorksDbContext> dbOptions;
        private readonly IConfiguration config;

        /// <summary>
        /// Constructor de la clase de pruebas. Configura la base de datos en memoria y la configuración mínima.
        /// </summary>
        public ProductsControllerTests()
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
        /// Obtiene una instancia simulada del servicio de productos.
        /// </summary>
        private ProductoService GetProductoServiceMock()
        {
            return new ProductoService(config);
        }

        #region Pruebas de métodos públicos

        /// <summary>
        /// Prueba que retorna todos los productos correctamente.
        /// </summary>
        [Fact]
        public async Task GetProducts_ReturnsAllProducts()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            context.Products.AddRange(
                new Product { ProductId = 1, Name = "Product 1", ProductNumber = "P1" },
                new Product { ProductId = 2, Name = "Product 2", ProductNumber = "P2" }
            );
            await context.SaveChangesAsync();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);

            // Act
            var result = await controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<Product>>(okResult.Value);
            Assert.Equal(2, products.Count());
        }

        /// <summary>
        /// Prueba que obtiene un producto por Id correctamente.
        /// </summary>
        [Fact]
        public async Task GetProduct_WithValidId_ReturnsProduct()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productId = 1;
            context.Products.Add(new Product { ProductId = productId, Name = "Product 1", ProductNumber = "P1" });
            await context.SaveChangesAsync();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);

            // Act
            var result = await controller.GetProduct(productId);

            // Assert
            var okResult = Assert.IsType<ActionResult<Product>>(result);
            var product = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(productId, product.ProductId);
        }

        /// <summary>
        /// Prueba que retorna NotFound cuando se busca un producto con Id inválido.
        /// </summary>
        [Fact]
        public async Task GetProduct_WithInvalidId_ReturnsNotFound()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);

            // Act
            var result = await controller.GetProduct(999);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        /// <summary>
        /// Prueba que crea un producto y retorna el resultado correcto.
        /// </summary>
        [Fact]
        public async Task CreateProduct_ReturnsCreated()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);
            var product = new Product { Name = "Nuevo", ProductNumber = "P100" };

            // Act
            var result = await controller.CreateProduct(product);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var createdProduct = Assert.IsType<Product>(createdResult.Value);
            Assert.Equal("Nuevo", createdProduct.Name);
        }

        /// <summary>
        /// Prueba que actualiza un producto y retorna NoContent.
        /// </summary>
        [Fact]
        public async Task UpdateProduct_ReturnsNoContent()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);
            var product = new Product { ProductId = 1, Name = "Prod", ProductNumber = "P1" };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            product.Name = "ProdEditado";
            var result = await controller.UpdateProduct(1, product);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Prueba que retorna BadRequest cuando hay un desajuste de Id en la actualización del producto.
        /// </summary>
        [Fact]
        public async Task UpdateProduct_ReturnsBadRequest_OnIdMismatch()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);
            var product = new Product { ProductId = 2 };

            // Act
            var result = await controller.UpdateProduct(1, product);

            // Assert
            var badRequest = Assert.IsType<BadRequestResult>(result);
        }

        /// <summary>
        /// Prueba que retorna NotFound cuando se intenta actualizar un producto que no existe.
        /// </summary>
        [Fact]
        public async Task UpdateProduct_ReturnsNotFound_OnMissingProduct()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);
            var product = new Product { ProductId = 1 };

            // Act
            var result = await controller.UpdateProduct(1, product);

            // Assert
            var notFound = Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Prueba que elimina un producto y retorna NoContent.
        /// </summary>
        [Fact]
        public async Task DeleteProduct_ReturnsNoContent()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);
            var product = new Product { ProductId = 1, Name = "Prod", ProductNumber = "P1" };
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var result = await controller.DeleteProduct(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Prueba que retorna Conflict cuando se intenta eliminar un producto que tiene dependencias.
        /// </summary>
        [Fact]
        public async Task DeleteProduct_ReturnsConflict_WhenHasDependencies()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);
            context.Products.Add(new Product { ProductId = 1, Name = "Prod", ProductNumber = "P1" });
            context.BillOfMaterials.Add(new BillOfMaterials { ProductAssemblyId = 1 });
            await context.SaveChangesAsync();

            // Act
            var result = await controller.DeleteProduct(1);

            // Assert
            var conflict = Assert.IsType<ConflictObjectResult>(result);
        }

        /// <summary>
        /// Prueba que retorna NotFound cuando se intenta eliminar un producto que no existe.
        /// </summary>
        [Fact]
        public async Task DeleteProduct_ReturnsNotFound()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);

            // Act
            var result = await controller.DeleteProduct(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Prueba que obtiene todo el inventario de productos.
        /// </summary>
        [Fact]
        public async Task GetAllInventory_ReturnsInventory()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);
            context.ProductInventories.Add(new ProductInventory { ProductId = 1, LocationId = 1, Quantity = 10 });
            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetAllInventory();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var inventory = Assert.IsAssignableFrom<IEnumerable<ProductInventory>>(okResult.Value);
            Assert.Single(inventory);
        }

        /// <summary>
        /// Prueba que obtiene el inventario de un producto específico.
        /// </summary>
        [Fact]
        public async Task GetProductInventory_ReturnsInventory()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);
            context.ProductInventories.Add(new ProductInventory { ProductId = 1, LocationId = 1, Quantity = 10 });
            await context.SaveChangesAsync();

            // Act
            var result = await controller.GetProductInventory(1);

            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<ProductInventory>>>(result);
            var inventory = Assert.IsAssignableFrom<IEnumerable<ProductInventory>>(okResult.Value);
            Assert.Single(inventory);
        }

        /// <summary>
        /// Prueba que retorna NotFound cuando se busca el inventario de un producto inexistente.
        /// </summary>
        [Fact]
        public async Task GetProductInventory_ReturnsNotFound()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);

            // Act
            var result = await controller.GetProductInventory(999);

            // Assert
            var notFound = Assert.IsType<ActionResult<IEnumerable<ProductInventory>>>(result);
            Assert.Null(notFound.Value);
        }

        /// <summary>
        /// Prueba que actualiza el inventario de un producto y retorna NoContent.
        /// </summary>
        [Fact]
        public async Task UpdateProductInventory_ReturnsNoContent()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);
            context.ProductInventories.Add(new ProductInventory { ProductId = 1, LocationId = 1, Quantity = 10 });
            await context.SaveChangesAsync();

            // Act
            var result = await controller.UpdateProductInventory(1, 1, 20);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        /// <summary>
        /// Prueba que retorna NotFound cuando se intenta actualizar el inventario de un producto inexistente.
        /// </summary>
        [Fact]
        public async Task UpdateProductInventory_ReturnsNotFound()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);

            // Act
            var result = await controller.UpdateProductInventory(1, 1, 20);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Prueba que obtiene los productos más vendidos.
        /// </summary>
        [Fact]
        public async Task GetTopVendidos_ReturnsOk()
        {
            // Arrange
            await using var context = GetDatabaseContext();
            var productoService = GetProductoServiceMock();
            var controller = new ProductsController(context, productoService);

            // Act
            var result = await controller.GetTopVendidos();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
        }

        #endregion
    }
}