namespace AdventureWorks.Enterprise.WebApp.Models
{
    /// <summary>
    /// DTO que representa empleados con mayor tiempo en un departamento.
    /// Útil para reportes de antigüedad y análisis de permanencia.
    /// </summary>
    public class EmpleadoMasTiempoDto
    {
        /// <summary>Identificador del empleado.</summary>
        public int EmployeeID { get; set; }
        /// <summary>Login del empleado.</summary>
        public string LoginID { get; set; } = string.Empty;
        /// <summary>Cargo o título del puesto.</summary>
        public string JobTitle { get; set; } = string.Empty;
        /// <summary>Nombre del departamento.</summary>
        public string DepartmentName { get; set; } = string.Empty;
        /// <summary>Años de permanencia en el departamento.</summary>
        public int YearsInDepartment { get; set; }
    }
}