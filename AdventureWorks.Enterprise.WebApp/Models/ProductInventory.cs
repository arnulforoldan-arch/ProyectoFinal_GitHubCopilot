namespace AdventureWorks.Enterprise.WebApp.Models
{
    /// <summary>
    /// Modelo que representa un registro de inventario de producto en la aplicaci�n cliente.
    /// �til para reportes y visualizaci�n de existencias.
    /// </summary>
    public class ProductInventory
    {
        /// <summary>Estante donde se ubica el producto.</summary>
        public string Shelf { get; set; } = string.Empty;
        /// <summary>Identificador del contenedor (bin) en la ubicaci�n.</summary>
        public byte Bin { get; set; }
        /// <summary>Cantidad disponible.</summary>
        public short Quantity { get; set; }
        /// <summary>Identificador del producto.</summary>
        public int ProductId { get; set; }
        /// <summary>Identificador de la ubicaci�n.</summary>
        public short LocationId { get; set; }
    }
}