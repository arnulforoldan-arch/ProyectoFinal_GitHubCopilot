using AdventureWorks.Enterprise.Api.Models;
using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace AdventureWorks.Enterprise.Api.Services
{
    /// <summary>
    /// Servicio para operaciones relacionadas con inventario de productos.
    /// Incluye consultas de productos con bajo inventario.
    /// </summary>
    public class ProductoInventarioService
    {
        private readonly string _connectionString;

        /// <summary>
        /// Inicializa el servicio de inventario de productos con la configuración de conexión.
        /// </summary>
        /// <param name="config">Configuración de la aplicación.</param>
        public ProductoInventarioService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        /// <summary>
        /// Obtiene los productos con bajo inventario usando un procedimiento almacenado.
        /// </summary>
        /// <returns>Lista de productos con bajo inventario.</returns>
        public async Task<IEnumerable<ProductoBajoInventarioDto>> ObtenerProductosBajoInventarioAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            return await connection.QueryAsync<ProductoBajoInventarioDto>(
                "Production.sp_ProductosBajoInventario",
                commandType: CommandType.StoredProcedure
            );
        }
    }
}