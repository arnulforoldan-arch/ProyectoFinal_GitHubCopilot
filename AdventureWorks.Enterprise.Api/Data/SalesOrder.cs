using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdventureWorks.Enterprise.Api.Data
{
    /// <summary>
    /// Representa una orden de venta.
    /// Mapea la tabla Sales.SalesOrderHeader de la base de datos.
    /// </summary>
    [Table("SalesOrderHeader", Schema = "Sales")]
    public class SalesOrder
    {
        /// <summary>
        /// Identificador �nico de la orden de venta (SalesOrderID).
        /// </summary>
        [Column("SalesOrderID")]
        public int SalesOrderId { get; set; }

        /// <summary>
        /// Fecha de la orden.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Fecha de vencimiento.
        /// </summary>
        public DateTime DueDate { get; set; }

        /// <summary>
        /// Fecha de env�o.
        /// </summary>
        public DateTime? ShipDate { get; set; }

        /// <summary>
        /// Estado de la orden.
        /// </summary>
        public byte Status { get; set; }

        /// <summary>
        /// Indica si la orden fue realizada en l�nea.
        /// </summary>
        public bool OnlineOrderFlag { get; set; }

        /// <summary>
        /// N�mero de la orden de venta.
        /// </summary>
        public string? SalesOrderNumber { get; set; }

        /// <summary>
        /// N�mero de orden de compra.
        /// </summary>
        public string? PurchaseOrderNumber { get; set; }

        /// <summary>
        /// N�mero de cuenta.
        /// </summary>
        public string? AccountNumber { get; set; }

        /// <summary>
        /// Identificador del cliente.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Identificador del vendedor.
        /// </summary>
        public int? SalesPersonId { get; set; }

        /// <summary>
        /// Identificador del territorio.
        /// </summary>
        public int? TerritoryId { get; set; }

        /// <summary>
        /// Subtotal de la orden.
        /// </summary>
        public decimal SubTotal { get; set; }

        /// <summary>
        /// Importe de impuestos.
        /// </summary>
        public decimal TaxAmt { get; set; }

        /// <summary>
        /// Importe de flete.
        /// </summary>
        public decimal Freight { get; set; }

        /// <summary>
        /// Total a pagar.
        /// </summary>
        public decimal TotalDue { get; set; }

        /// <summary>
        /// Comentarios de la orden.
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// C�digo de aprobaci�n de tarjeta de cr�dito.
        /// </summary>
        public string? CreditCardApprovalCode { get; set; }
    }
}