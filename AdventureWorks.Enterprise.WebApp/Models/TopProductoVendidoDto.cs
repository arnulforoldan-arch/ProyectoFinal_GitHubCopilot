namespace AdventureWorks.Enterprise.WebApp.Models
{
    /// <summary>
    /// DTO que representa información de un producto más vendido.
    /// Incluye cantidad total vendida, ingresos y número de órdenes asociadas.
    /// </summary>
    public class TopProductoVendidoDto
    {
        /// <summary>Identificador del producto.</summary>
        public int ProductID { get; set; }
        /// <summary>Nombre del producto.</summary>
        public string ProductName { get; set; } = string.Empty;
        /// <summary>Número único del producto.</summary>
        public string ProductNumber { get; set; } = string.Empty;
        /// <summary>Cantidad total vendida del producto.</summary>
        public int TotalQuantitySold { get; set; }
        /// <summary>Ingresos totales generados por las ventas del producto.</summary>
        public decimal TotalRevenue { get; set; }
        /// <summary>Número de órdenes en las que el producto aparece.</summary>
        public int NumberOfOrders { get; set; }
    }
}