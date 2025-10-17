namespace AdventureWorks.Enterprise.Api.Models
{
    /// <summary>
    /// DTO para representar empleados con m�s tiempo en un departamento.
    /// Incluye informaci�n de antig�edad y cargo.
    /// </summary>
    public class EmpleadoMasTiempoDto
    {
        /// <summary>
        /// Identificador del empleado.
        /// </summary>
        public int EmployeeID { get; set; }

        /// <summary>
        /// Login del empleado.
        /// </summary>
        public string LoginID { get; set; } = string.Empty;

        /// <summary>
        /// Cargo o puesto del empleado.
        /// </summary>
        public string JobTitle { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del departamento.
        /// </summary>
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de inicio en el departamento.
        /// </summary>
        public DateTime DepartmentStartDate { get; set; }

        /// <summary>
        /// A�os en el departamento.
        /// </summary>
        public int YearsInDepartment { get; set; }
    }
}