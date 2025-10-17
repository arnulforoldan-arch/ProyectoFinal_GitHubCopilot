namespace AdventureWorks.Enterprise.Api.Models
{
    /// <summary>
    /// DTO para representar los productos m�s vendidos.
    /// Incluye informaci�n de ventas y cantidad de �rdenes.
    /// </summary>
    public class TopProductoVendidoDto
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
        /// Cantidad total vendida.
        /// </summary>
        public int TotalQuantitySold { get; set; }

        /// <summary>
        /// Ingresos totales generados por el producto.
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// N�mero de �rdenes en las que aparece el producto.
        /// </summary>
        public int NumberOfOrders { get; set; }
    }
}