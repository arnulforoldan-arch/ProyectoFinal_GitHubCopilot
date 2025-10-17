using AdventureWorks.Enterprise.Api.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdventureWorks.Enterprise.Api.Services
{
    /// <summary>
    /// Servicio para operaciones relacionadas con productos.
    /// Incluye consultas de productos m�s vendidos.
    /// </summary>
    public class ProductoService
    {
        private readonly string _connectionString;

        /// <summary>
        /// Inicializa el servicio de productos con la configuraci�n de conexi�n.
        /// </summary>
        /// <param name="config">Configuraci�n de la aplicaci�n.</param>
        public ProductoService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Obtiene el top 10 de productos m�s vendidos usando un procedimiento almacenado.
        /// </summary>
        /// <returns>Lista de productos m�s vendidos.</returns>
        public async Task<IEnumerable<TopProductoVendidoDto>> ObtenerTop10ProductosVendidosAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<TopProductoVendidoDto>(
                "Production.sp_Top10ProductosMasVendidos",
                commandType: CommandType.StoredProcedure
            );
        }
    }
}