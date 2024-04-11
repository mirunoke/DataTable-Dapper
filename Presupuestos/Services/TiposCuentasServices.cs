using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Presupuestos.Infraestructure;
using Presupuestos.Models;

namespace Presupuestos.Services
{
    public class TiposCuentasServices : ITiposCuentasServices
    {
        private readonly string connectionString;
        private readonly ITiposCuentasServices _tiposCuentasServices;
        private readonly IUsuariosServices usuariosServices;

        public TiposCuentasServices(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public void Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = connection.QuerySingle<int>
            ($@"INSERT INTO TiposCuentas (Nombre, UsuarioId,Orden)
            VALUES (@Nombre, @UsuarioId, 0) 
            SELECT SCOPE_IDENTITY();", tipoCuenta);
            tipoCuenta.Id = id;

        }

        public async Task CrearAsync(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>
            ($@"INSERT INTO TipoCuenta (Nombre, UsuarioId,Orden)
            VALUES (@Nombre, @UsuarioId, @Orden) 
            SELECT SCOPE_IDENTITY();", tipoCuenta);
            tipoCuenta.Id = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>(@"SELECT 1 FROM TipoCuenta
                                                                        WHERE Nombre= @Nombre AND UsuarioId = @UsuarioId;",
                                                                        new { nombre, usuarioId });
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT Id, Nombre,Orden FROM TiposCuentas WHERE UsuarioId = @UsuarioId", new { usuarioId });
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT Id, Nombre, Orden FROM TipoCuenta 
                                                WHERE Id =@Id AND UsuarioId = @UsuarioId",
                new { id, usuarioId });
        }


        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TiposCuentas SET Nombre = @Nombre WHERE Id = @Id", tipoCuenta);
        }


        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE TipoCuenta WHERE Id = @Id", new {id });
        }
    }
}
