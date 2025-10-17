using System;

namespace AdventureWorks.Enterprise.WebApp.Models
{
    /// <summary>
    /// Modelo que representa un producto en la aplicación cliente.
    /// Incluye atributos generales de catálogo y fabricación.
    /// </summary>
    public class Product
    {
        /// <summary>Identificador único del producto.</summary>
        public int ProductId { get; set; }
        /// <summary>Nombre descriptivo del producto.</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>Código o número único de producto.</summary>
        public string ProductNumber { get; set; } = string.Empty;
        /// <summary>Indica si el producto se fabrica internamente.</summary>
        public bool MakeFlag { get; set; }
        /// <summary>Indica si el producto es un bien terminado.</summary>
        public bool FinishedGoodsFlag { get; set; }
        /// <summary>Color del producto.</summary>
        public string? Color { get; set; }
        /// <summary>Nivel mínimo de stock de seguridad.</summary>
        public short SafetyStockLevel { get; set; }
        /// <summary>Punto de reorden del inventario.</summary>
        public short ReorderPoint { get; set; }
        /// <summary>Costo estándar.</summary>
        public decimal StandardCost { get; set; }
        /// <summary>Precio de lista.</summary>
        public decimal ListPrice { get; set; }
        /// <summary>Descripción de tamaño.</summary>
        public string? Size { get; set; }
        /// <summary>Código de unidad de medida del tamaño.</summary>
        public string? SizeUnitMeasureCode { get; set; }
        /// <summary>Código de unidad de medida del peso.</summary>
        public string? WeightUnitMeasureCode { get; set; }
        /// <summary>Peso del producto.</summary>
        public decimal? Weight { get; set; }
        /// <summary>Días estimados de fabricación.</summary>
        public int DaysToManufacture { get; set; }
        /// <summary>Línea de producto.</summary>
        public string? ProductLine { get; set; }
        /// <summary>Clase del producto.</summary>
        public string? Class { get; set; }
        /// <summary>Estilo del producto.</summary>
        public string? Style { get; set; }
        /// <summary>Identificador de subcategoría.</summary>
        public int? ProductSubcategoryId { get; set; }
        /// <summary>Identificador de modelo de producto.</summary>
        public int? ProductModelId { get; set; }
        /// <summary>Fecha de inicio de comercialización.</summary>
        public DateTime SellStartDate { get; set; }
        /// <summary>Fecha de fin de comercialización.</summary>
        public DateTime? SellEndDate { get; set; }
        /// <summary>Fecha de discontinuación del producto.</summary>
        public DateTime? DiscontinuedDate { get; set; }
    }
}