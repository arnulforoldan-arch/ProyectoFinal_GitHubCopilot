using System;

namespace AdventureWorks.Enterprise.WebApp.Models
{
    /// <summary>
    /// Modelo que representa una orden de venta en la aplicación cliente.
    /// Incluye información básica de estado y montos.
    /// </summary>
    public class SalesOrder
    {
        /// <summary>Identificador único de la orden.</summary>
        public int SalesOrderId { get; set; }
        /// <summary>Fecha en que se genera la orden.</summary>
        public DateTime OrderDate { get; set; }
        /// <summary>Fecha límite o de vencimiento.</summary>
        public DateTime DueDate { get; set; }
        /// <summary>Fecha de envío (si ya fue enviada).</summary>
        public DateTime? ShipDate { get; set; }
        /// <summary>Estado numérico de la orden (1..5).</summary>
        public byte Status { get; set; }
        /// <summary>Indica si la orden fue realizada en línea.</summary>
        public bool OnlineOrderFlag { get; set; }
        /// <summary>Número interno de la orden.</summary>
        public string? SalesOrderNumber { get; set; }
        /// <summary>Número de orden de compra asociado.</summary>
        public string? PurchaseOrderNumber { get; set; }
        /// <summary>Número de cuenta del cliente.</summary>
        public string? AccountNumber { get; set; }
        /// <summary>Identificador del cliente.</summary>
        public int CustomerId { get; set; }
        /// <summary>Identificador del vendedor (si aplica).</summary>
        public int? SalesPersonId { get; set; }
        /// <summary>Identificador del territorio (si aplica).</summary>
        public int? TerritoryId { get; set; }
        /// <summary>Subtotal de la orden.</summary>
        public decimal SubTotal { get; set; }
        /// <summary>Impuesto aplicado.</summary>
        public decimal TaxAmt { get; set; }
        /// <summary>Costo de flete.</summary>
        public decimal Freight { get; set; }
        /// <summary>Total a pagar (subtotal + impuestos + flete).</summary>
        public decimal TotalDue { get; set; }
        /// <summary>Comentarios u observaciones.</summary>
        public string? Comment { get; set; }
        /// <summary>Código de aprobación de tarjeta de crédito.</summary>
        public string? CreditCardApprovalCode { get; set; }
    }
}