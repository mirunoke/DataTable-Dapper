using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using Presupuestos.Models;

namespace Presupuestos.Infraestructure
{
    public interface ITiposCuentasServices
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        void Crear(TipoCuenta tipoCuenta);
        Task CrearAsync(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorId(int id, int usuarioId);
    }
}
