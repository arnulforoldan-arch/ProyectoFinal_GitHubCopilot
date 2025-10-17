using AdventureWorks.Enterprise.Api.Data;
using AdventureWorks.Enterprise.Api.Models;
using AdventureWorks.Enterprise.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdventureWorks.Enterprise.Api.Controllers
{
    /// <summary>
    /// Controlador para la gestión de productos e inventario.
    /// Proporciona operaciones CRUD sobre la entidad Product.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AdventureWorksDbContext _context;
        private readonly ProductoService _productoService;

        /// <summary>
        /// Inicializa una nueva instancia del controlador ProductsController.
        /// </summary>
        /// <param name="context">Contexto de base de datos inyectado.</param>
        /// <param name="productoService">Servicio de productos inyectado.</param>
        public ProductsController(AdventureWorksDbContext context, ProductoService productoService)
        {
            _context = context;
            _productoService = productoService;
        }

        /// <summary>
        /// Obtiene la lista de todos los productos.
        /// </summary>
        /// <returns>Lista de productos.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            return await _context.Products.ToListAsync();
        }

        /// <summary>
        /// Obtiene un producto por su identificador.
        /// </summary>
        /// <param name="id">Identificador del producto.</param>
        /// <returns>Producto encontrado o NotFound si no existe.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();
            return product;
        }

        /// <summary>
        /// Crea un nuevo producto.
        /// </summary>
        /// <param name="product">Datos del producto a crear.</param>
        /// <returns>Producto creado.</returns>
        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetProduct), new { id = product.ProductId }, product);
        }

        /// <summary>
        /// Actualiza los datos de un producto existente.
        /// </summary>
        /// <param name="id">Identificador del producto.</param>
        /// <param name="product">Datos actualizados del producto.</param>
        /// <returns>NoContent si la actualización fue exitosa, NotFound si no existe.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.ProductId)
                return BadRequest();

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        /// <summary>
        /// Elimina un producto si no tiene dependencias en BillOfMaterials.
        /// </summary>
        /// <param name="id">Identificador del producto.</param>
        /// <returns>NoContent si la operación fue exitosa, Conflict si tiene dependencias, NotFound si no existe.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Verifica dependencias en BillOfMaterials
            var tieneDependencias = await _context.BillOfMaterials
                .AnyAsync(b => b.ProductAssemblyId == id);

            if (tieneDependencias)
                return Conflict("No se puede eliminar el producto porque está referenciado en BillOfMaterials.");

            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound();

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Obtiene el inventario de todos los productos.
        /// </summary>
        /// <returns>Lista de inventario por producto.</returns>
        [HttpGet("inventory")]
        public async Task<ActionResult<IEnumerable<ProductInventory>>> GetAllInventory()
        {
            return await _context.ProductInventories.ToListAsync();
        }

        /// <summary>
        /// Obtiene el inventario de un producto específico.
        /// </summary>
        /// <param name="id">Identificador del producto.</param>
        /// <returns>Inventario del producto o NotFound si no existe.</returns>
        [HttpGet("{id}/inventory")]
        public async Task<ActionResult<IEnumerable<ProductInventory>>> GetProductInventory(int id)
        {
            var inventory = await _context.ProductInventories
                .Where(pi => pi.ProductId == id)
                .ToListAsync();

            if (inventory == null || inventory.Count == 0)
                return NotFound();

            return inventory;
        }

        /// <summary>
        /// Actualiza la cantidad en inventario de un producto en una ubicación.
        /// </summary>
        /// <param name="id">Identificador del producto.</param>
        /// <param name="locationId">Identificador de la ubicación.</param>
        /// <param name="quantity">Nueva cantidad.</param>
        /// <returns>NoContent si la actualización fue exitosa, NotFound si no existe.</returns>
        [HttpPut("{id}/inventory/{locationId}")]
        public async Task<IActionResult> UpdateProductInventory(int id, short locationId, [FromBody] short quantity)
        {
            var inventory = await _context.ProductInventories
                .FirstOrDefaultAsync(pi => pi.ProductId == id && pi.LocationId == locationId);

            if (inventory == null)
                return NotFound();

            inventory.Quantity = quantity;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Obtiene los 10 productos más vendidos.
        /// </summary>
        /// <returns>Lista de los 10 productos más vendidos.</returns>
        [HttpGet("top-vendidos")]
        public async Task<ActionResult<IEnumerable<TopProductoVendidoDto>>> GetTopVendidos()
        {
            var productos = await _productoService.ObtenerTop10ProductosVendidosAsync();
            return Ok(productos);
        }

        /// <summary>
        /// Verifica si existe un producto por su identificador.
        /// </summary>
        /// <param name="id">Identificador del producto.</param>
        /// <returns>True si existe, false en caso contrario.</returns>
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}