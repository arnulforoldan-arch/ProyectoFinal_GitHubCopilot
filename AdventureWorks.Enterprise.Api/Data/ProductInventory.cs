using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdventureWorks.Enterprise.Api.Data
{
    /// <summary>
    /// Representa el inventario de un producto en una ubicación específica.
    /// Mapea la tabla Production.ProductInventory de la base de datos.
    /// </summary>
    [Table("ProductInventory", Schema = "Production")]
    public class ProductInventory
    {
        /// <summary>
        /// Identificador del producto.
        /// </summary>
        [Key]
        [Column("ProductID")]
        public int ProductId { get; set; }

        /// <summary>
        /// Identificador de la ubicación.
        /// </summary>
        [Key]
        [Column("LocationID")]
        public short LocationId { get; set; }

        /// <summary>
        /// Estante donde se almacena el producto.
        /// </summary>
        [Column("Shelf")]
        public string Shelf { get; set; } = string.Empty;

        /// <summary>
        /// Número de contenedor (bin) en la ubicación.
        /// </summary>
        [Column("Bin")]
        public byte Bin { get; set; }

        /// <summary>
        /// Cantidad disponible en inventario.
        /// </summary>
        [Column("Quantity")]
        public short Quantity { get; set; }
    }
}