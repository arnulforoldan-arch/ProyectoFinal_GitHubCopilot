using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdventureWorks.Enterprise.Api.Data
{
    /// <summary>
    /// Representa un empleado de la empresa.
    /// Mapea la tabla HumanResources.Employee de la base de datos.
    /// </summary>
    [Table("Employee", Schema = "HumanResources")]
    public class Employee
    {
        /// <summary>
        /// Identificador �nico del empleado (BusinessEntityID).
        /// </summary>
        [Column("BusinessEntityID")]
        public int EmployeeId { get; set; }

        /// <summary>
        /// N�mero de identificaci�n nacional.
        /// </summary>
        public string NationalIdNumber { get; set; } = string.Empty;

        /// <summary>
        /// Login del empleado.
        /// </summary>
        public string LoginId { get; set; } = string.Empty;

        /// <summary>
        /// T�tulo del puesto de trabajo.
        /// </summary>
        public string JobTitle { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de nacimiento.
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Estado civil (S: Soltero, M: Casado, D: Divorciado, W: Viudo).
        /// </summary>
        public string MaritalStatus { get; set; } = string.Empty;

        /// <summary>
        /// G�nero (M: Masculino, F: Femenino).
        /// </summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de contrataci�n.
        /// </summary>
        public DateTime HireDate { get; set; }

        /// <summary>
        /// Indica si el empleado es asalariado.
        /// </summary>
        public bool SalariedFlag { get; set; }

        /// <summary>
        /// Horas de vacaciones disponibles.
        /// </summary>
        public short VacationHours { get; set; }

        /// <summary>
        /// Horas de baja por enfermedad disponibles.
        /// </summary>
        public short SickLeaveHours { get; set; }

        /// <summary>
        /// Indica si el empleado est� activo.
        /// </summary>
        public bool CurrentFlag { get; set; }
    }
}