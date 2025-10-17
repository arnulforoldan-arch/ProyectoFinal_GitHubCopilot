namespace AdventureWorks.Enterprise.WebApp.Models
{
    /// <summary>
    /// DTO que representa productos con bajo inventario en la aplicación cliente.
    /// Incluye datos para indicadores visuales y exportación.
    /// </summary>
    public class ProductoBajoInventarioDto
    {
        /// <summary>Identificador del producto.</summary>
        public int ProductID { get; set; }
        /// <summary>Nombre del producto.</summary>
        public string ProductName { get; set; } = string.Empty;
        /// <summary>Número único del producto.</summary>
        public string ProductNumber { get; set; } = string.Empty;
        /// <summary>Nivel de stock de seguridad.</summary>
        public int SafetyStockLevel { get; set; }
        /// <summary>Inventario actual.</summary>
        public int CurrentInventory { get; set; }
        /// <summary>Punto de reorden.</summary>
        public int ReorderPoint { get; set; }
        /// <summary>Ubicación física del producto.</summary>
        public string ProductLocation { get; set; } = string.Empty;
        /// <summary>Estado del inventario (Ej: Bajo, Crítico, OK).</summary>
        public string InventoryStatus { get; set; } = string.Empty;
    }
}