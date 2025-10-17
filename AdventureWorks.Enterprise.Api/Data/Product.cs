using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdventureWorks.Enterprise.Api.Data
{
    /// <summary>
    /// Representa un producto.
    /// Mapea la tabla Production.Product de la base de datos.
    /// </summary>
    [Table("Product", Schema = "Production")]
    public class Product
    {
        /// <summary>
        /// Identificador único del producto (ProductID).
        /// </summary>
        [Column("ProductID")]
        public int ProductId { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Número de producto.
        /// </summary>
        public string ProductNumber { get; set; } = string.Empty;

        /// <summary>
        /// Indica si el producto se fabrica internamente.
        /// </summary>
        public bool MakeFlag { get; set; }

        /// <summary>
        /// Indica si el producto es un bien terminado.
        /// </summary>
        public bool FinishedGoodsFlag { get; set; }

        /// <summary>
        /// Color del producto.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Nivel mínimo de stock de seguridad.
        /// </summary>
        public short SafetyStockLevel { get; set; }

        /// <summary>
        /// Punto de reorden.
        /// </summary>
        public short ReorderPoint { get; set; }

        /// <summary>
        /// Costo estándar.
        /// </summary>
        public decimal StandardCost { get; set; }

        /// <summary>
        /// Precio de lista.
        /// </summary>
        public decimal ListPrice { get; set; }

        /// <summary>
        /// Tamaño del producto.
        /// </summary>
        public string? Size { get; set; }

        /// <summary>
        /// Código de unidad de medida de tamaño.
        /// </summary>
        public string? SizeUnitMeasureCode { get; set; }

        /// <summary>
        /// Código de unidad de medida de peso.
        /// </summary>
        public string? WeightUnitMeasureCode { get; set; }

        /// <summary>
        /// Peso del producto.
        /// </summary>
        public decimal? Weight { get; set; }

        /// <summary>
        /// Días para fabricar el producto.
        /// </summary>
        public int DaysToManufacture { get; set; }

        /// <summary>
        /// Línea de producto.
        /// </summary>
        public string? ProductLine { get; set; }

        /// <summary>
        /// Clase del producto.
        /// </summary>
        public string? Class { get; set; }

        /// <summary>
        /// Estilo del producto.
        /// </summary>
        public string? Style { get; set; }

        /// <summary>
        /// Identificador de subcategoría de producto.
        /// </summary>
        public int? ProductSubcategoryId { get; set; }

        /// <summary>
        /// Identificador de modelo de producto.
        /// </summary>
        public int? ProductModelId { get; set; }

        /// <summary>
        /// Fecha de inicio de venta.
        /// </summary>
        public DateTime SellStartDate { get; set; }

        /// <summary>
        /// Fecha de fin de venta.
        /// </summary>
        public DateTime? SellEndDate { get; set; }

        /// <summary>
        /// Fecha de discontinuación.
        /// </summary>
        public DateTime? DiscontinuedDate { get; set; }
    }
}