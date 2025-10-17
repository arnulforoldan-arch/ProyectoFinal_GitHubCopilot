using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdventureWorks.Enterprise.Api.Data
{
    /// <summary>
    /// Representa la relación de materiales de ensamblaje.
    /// Mapea la tabla Production.BillOfMaterials de la base de datos.
    /// </summary>
    [Table("BillOfMaterials", Schema = "Production")]
    public class BillOfMaterials
    {
        /// <summary>
        /// Identificador único del registro de materiales (BillOfMaterialsID).
        /// </summary>
        [Column("BillOfMaterialsID")]
        public int BillOfMaterialsId { get; set; }

        /// <summary>
        /// Identificador del producto ensamblado.
        /// </summary>
        [Column("ProductAssemblyID")]
        public int? ProductAssemblyId { get; set; }

        // Puedes agregar más propiedades si necesitas consultar otros campos
    }
}