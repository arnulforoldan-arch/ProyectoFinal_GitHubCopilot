using System;

namespace AdventureWorks.Enterprise.WebApp.Models
{
    /// <summary>
    /// Modelo que representa un producto en la aplicaci�n cliente.
    /// Incluye atributos generales de cat�logo y fabricaci�n.
    /// </summary>
    public class Product
    {
        /// <summary>Identificador �nico del producto.</summary>
        public int ProductId { get; set; }
        /// <summary>Nombre descriptivo del producto.</summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>C�digo o n�mero �nico de producto.</summary>
        public string ProductNumber { get; set; } = string.Empty;
        /// <summary>Indica si el producto se fabrica internamente.</summary>
        public bool MakeFlag { get; set; }
        /// <summary>Indica si el producto es un bien terminado.</summary>
        public bool FinishedGoodsFlag { get; set; }
        /// <summary>Color del producto.</summary>
        public string? Color { get; set; }
        /// <summary>Nivel m�nimo de stock de seguridad.</summary>
        public short SafetyStockLevel { get; set; }
        /// <summary>Punto de reorden del inventario.</summary>
        public short ReorderPoint { get; set; }
        /// <summary>Costo est�ndar.</summary>
        public decimal StandardCost { get; set; }
        /// <summary>Precio de lista.</summary>
        public decimal ListPrice { get; set; }
        /// <summary>Descripci�n de tama�o.</summary>
        public string? Size { get; set; }
        /// <summary>C�digo de unidad de medida del tama�o.</summary>
        public string? SizeUnitMeasureCode { get; set; }
        /// <summary>C�digo de unidad de medida del peso.</summary>
        public string? WeightUnitMeasureCode { get; set; }
        /// <summary>Peso del producto.</summary>
        public decimal? Weight { get; set; }
        /// <summary>D�as estimados de fabricaci�n.</summary>
        public int DaysToManufacture { get; set; }
        /// <summary>L�nea de producto.</summary>
        public string? ProductLine { get; set; }
        /// <summary>Clase del producto.</summary>
        public string? Class { get; set; }
        /// <summary>Estilo del producto.</summary>
        public string? Style { get; set; }
        /// <summary>Identificador de subcategor�a.</summary>
        public int? ProductSubcategoryId { get; set; }
        /// <summary>Identificador de modelo de producto.</summary>
        public int? ProductModelId { get; set; }
        /// <summary>Fecha de inicio de comercializaci�n.</summary>
        public DateTime SellStartDate { get; set; }
        /// <summary>Fecha de fin de comercializaci�n.</summary>
        public DateTime? SellEndDate { get; set; }
        /// <summary>Fecha de discontinuaci�n del producto.</summary>
        public DateTime? DiscontinuedDate { get; set; }
    }
}