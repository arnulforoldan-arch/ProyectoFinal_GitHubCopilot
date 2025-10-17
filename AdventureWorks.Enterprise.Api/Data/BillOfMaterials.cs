using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdventureWorks.Enterprise.Api.Data
{
    /// <summary>
    /// Representa la relaci�n de materiales de ensamblaje.
    /// Mapea la tabla Production.BillOfMaterials de la base de datos.
    /// </summary>
    [Table("BillOfMaterials", Schema = "Production")]
    public class BillOfMaterials
    {
        /// <summary>
        /// Identificador �nico del registro de materiales (BillOfMaterialsID).
        /// </summary>
        [Column("BillOfMaterialsID")]
        public int BillOfMaterialsId { get; set; }

        /// <summary>
        /// Identificador del producto ensamblado.
        /// </summary>
        [Column("ProductAssemblyID")]
        public int? ProductAssemblyId { get; set; }

        // Puedes agregar m�s propiedades si necesitas consultar otros campos
    }
}