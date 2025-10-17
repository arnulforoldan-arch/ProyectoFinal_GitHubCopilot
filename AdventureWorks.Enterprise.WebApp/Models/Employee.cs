using System;

namespace AdventureWorks.Enterprise.WebApp.Models
{
    /// <summary>
    /// Modelo que representa un empleado en la aplicación cliente.
    /// Se utiliza para mostrar, crear y editar empleados desde la interfaz.
    /// </summary>
    public class Employee
    {
        /// <summary>Identificador único del empleado.</summary>
        public int EmployeeId { get; set; }
        /// <summary>Número de identificación nacional.</summary>
        public string NationalIdNumber { get; set; } = string.Empty;
        /// <summary>Login del empleado para ingreso al sistema.</summary>
        public string LoginId { get; set; } = string.Empty;
        /// <summary>Cargo o puesto del empleado.</summary>
        public string JobTitle { get; set; } = string.Empty;
        /// <summary>Fecha de nacimiento.</summary>
        public DateTime BirthDate { get; set; }
        /// <summary>Estado civil (S, M, D, W).</summary>
        public string MaritalStatus { get; set; } = string.Empty;
        /// <summary>Género (M/F).</summary>
        public string Gender { get; set; } = string.Empty;
        /// <summary>Fecha de contratación.</summary>
        public DateTime HireDate { get; set; }
        /// <summary>Indica si es empleado asalariado.</summary>
        public bool SalariedFlag { get; set; }
        /// <summary>Horas de vacaciones disponibles.</summary>
        public short VacationHours { get; set; }
        /// <summary>Horas de baja por enfermedad disponibles.</summary>
        public short SickLeaveHours { get; set; }
        /// <summary>Indica si el empleado está activo.</summary>
        public bool CurrentFlag { get; set; }
    }
}