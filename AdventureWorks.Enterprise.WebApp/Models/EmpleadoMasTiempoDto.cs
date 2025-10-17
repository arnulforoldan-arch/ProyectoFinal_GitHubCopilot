namespace AdventureWorks.Enterprise.WebApp.Models
{
    /// <summary>
    /// DTO que representa empleados con mayor tiempo en un departamento.
    /// �til para reportes de antig�edad y an�lisis de permanencia.
    /// </summary>
    public class EmpleadoMasTiempoDto
    {
        /// <summary>Identificador del empleado.</summary>
        public int EmployeeID { get; set; }
        /// <summary>Login del empleado.</summary>
        public string LoginID { get; set; } = string.Empty;
        /// <summary>Cargo o t�tulo del puesto.</summary>
        public string JobTitle { get; set; } = string.Empty;
        /// <summary>Nombre del departamento.</summary>
        public string DepartmentName { get; set; } = string.Empty;
        /// <summary>A�os de permanencia en el departamento.</summary>
        public int YearsInDepartment { get; set; }
    }
}