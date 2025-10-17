namespace AdventureWorks.Enterprise.Api.Models
{
    /// <summary>
    /// DTO para representar productos con bajo inventario.
    /// Incluye informaci�n relevante de stock y ubicaci�n.
    /// </summary>
    public class ProductoBajoInventarioDto
    {
        /// <summary>
        /// Identificador del producto.
        /// </summary>
        public int ProductID { get; set; }

        /// <summary>
        /// Nombre del producto.
        /// </summary>
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// N�mero de producto.
        /// </summary>
        public string ProductNumber { get; set; } = string.Empty;

        /// <summary>
        /// Nivel de stock de seguridad.
        /// </summary>
        public int SafetyStockLevel { get; set; }

        /// <summary>
        /// Inventario actual.
        /// </summary>
        public int CurrentInventory { get; set; }

        /// <summary>
        /// Punto de reorden.
        /// </summary>
        public int ReorderPoint { get; set; }

        /// <summary>
        /// Ubicaci�n del producto en almac�n.
        /// </summary>
        public string ProductLocation { get; set; } = string.Empty;

        /// <summary>
        /// Estado del inventario (por ejemplo: Bajo, Cr�tico, OK).
        /// </summary>
        public string InventoryStatus { get; set; } = string.Empty;
    }
}