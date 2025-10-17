using AdventureWorks.Enterprise.Api.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdventureWorks.Enterprise.Api.Services
{
    /// <summary>
    /// Servicio para operaciones relacionadas con empleados.
    /// Incluye consultas de empleados con m�s antig�edad.
    /// </summary>
    public class EmpleadoService
    {
        private readonly string _connectionString;

        /// <summary>
        /// Inicializa el servicio de empleados con la configuraci�n de conexi�n.
        /// </summary>
        /// <param name="config">Configuraci�n de la aplicaci�n.</param>
        public EmpleadoService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Obtiene los empleados con m�s tiempo en la empresa usando un procedimiento almacenado.
        /// </summary>
        /// <returns>Lista de empleados con mayor antig�edad.</returns>
        public async Task<IEnumerable<EmpleadoMasTiempoDto>> ObtenerEmpleadosMasTiempoAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<EmpleadoMasTiempoDto>(
                "HumanResources.sp_EmpleadosMasTiempo",
                commandType: CommandType.StoredProcedure
            );
        }
    }
}