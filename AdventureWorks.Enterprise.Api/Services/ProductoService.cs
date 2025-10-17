using AdventureWorks.Enterprise.Api.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdventureWorks.Enterprise.Api.Services
{
    /// <summary>
    /// Servicio para operaciones relacionadas con productos.
    /// Incluye consultas de productos más vendidos.
    /// </summary>
    public class ProductoService
    {
        private readonly string _connectionString;

        /// <summary>
        /// Inicializa el servicio de productos con la configuración de conexión.
        /// </summary>
        /// <param name="config">Configuración de la aplicación.</param>
        public ProductoService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Obtiene el top 10 de productos más vendidos usando un procedimiento almacenado.
        /// </summary>
        /// <returns>Lista de productos más vendidos.</returns>
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