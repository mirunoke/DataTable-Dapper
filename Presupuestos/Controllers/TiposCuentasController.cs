using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Presupuestos.Filters;
using Presupuestos.Infraestructure;
using Presupuestos.Models;
using Presupuestos.Services;


namespace Presupuesto.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly string connectionString;
        private readonly ITiposCuentasServices _tipoCuentasServices;
        private readonly IUsuariosServices _usuariosServices;

        public TiposCuentasController(IConfiguration configuration, ITiposCuentasServices tipoCuentasServices, IUsuariosServices usuariosServices)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
            this._tipoCuentasServices = tipoCuentasServices;
            _usuariosServices = usuariosServices;
        }

        [ServiceFilter(typeof(GlobalExceptionFilter))]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TipoCuenta tipoCuenta)
        {
            var ExistAccount = await _tipoCuentasServices.Existe(tipoCuenta.Nombre, tipoCuenta.UsuarioID);
            if (ExistAccount)
            {
                ModelState.AddModelError(nameof(tipoCuenta.Nombre),
                    $"El nombre {tipoCuenta} ya existe");
                return View(tipoCuenta);
            }

            await _tipoCuentasServices.CrearAsync(tipoCuenta);

            return Ok();
        }

        [ServiceFilter(typeof(GlobalExceptionFilter))]
        public IActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> VerificarExisteTipoCuenta(string nombre)
        {
            var usuarioId = _usuariosServices.ObtenerUsuarioId();
            var existe = await _tipoCuentasServices.Existe(nombre, usuarioId);
            if (existe)
            {
                return Json($"El nombre {nombre} ya existe");
            }
            return Json(true);
        }
        public async Task<IActionResult> Index()
        {
            var usuarioId = _usuariosServices.ObtenerUsuarioId();
            var tiposCuentas = await _tipoCuentasServices.Obtener(usuarioId);
            return Json(tiposCuentas);
        }

        public async Task Actualizar([FromBody] TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync
                ($@"UPDATE TipoCuenta SET Nombre = @Nombre, Orden = @Orden WHERE Id = @Id", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>
                (@"SELECT Id, Nombre, UsuarioId, 
                Orden FROM TipoCuenta WHERE Id = @Id AND UsuarioId = @UsuarioId",
                new { id, usuarioId });
        }

        public IActionResult AllTiposCuentas()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> GetDatosPaginados(int draw, int start, int length)

        {
            int skip = start;
            int take = length;

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var pagedProductsQuery = @"SELECT * FROM (SELECT 
                                    ROW_NUMBER() OVER (ORDER BY Id) AS RowNum, 
                                    Id, 
                                    UsuarioId, 
                                    Nombre, 
                                    Orden
                                    FROM TipoCuenta) AS RowConstrainedResult WHERE RowNum > @Skip AND RowNum <= (@Skip + @Take) 
                                    ORDER BY RowNum";

                var products = await connection.QueryAsync<dynamic>(
                    pagedProductsQuery,
                    new { Skip = skip, Take = take }
                );

                // Consulta para obtener el total de productos
                var countQuery = "SELECT COUNT(*) FROM TipoCuenta";
                var totalRecords = await connection.ExecuteScalarAsync<int>(countQuery);

                return Ok(new
                {
                    draw = draw,
                    recordsTotal = totalRecords,
                    recordsFiltered = totalRecords,
                    data = products
                });
            }
        }

        [HttpGet]
        [ServiceFilter(typeof(GlobalExceptionFilter))]
        public async Task<ActionResult> Editar(int id)
        {
            var usuarioId = _usuariosServices.ObtenerUsuarioId();
            var tipoCuenta = await _tipoCuentasServices.ObtenerPorId(id, usuarioId);
            if (tipoCuenta == null)
            {
                return NotFound();
            }
            return View(tipoCuenta);
        }
        [ServiceFilter(typeof(GlobalExceptionFilter))]
        public async Task<ActionResult> Editar(TipoCuenta tipoCuenta)
        {
            var usuarioId = _usuariosServices.ObtenerUsuarioId();
            var tipoCuentaExiste = await _tipoCuentasServices.ObtenerPorId(tipoCuenta.Id, usuarioId);
            if (tipoCuentaExiste == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await _tipoCuentasServices.Actualizar(tipoCuenta);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [ServiceFilter(typeof(GlobalExceptionFilter))]
        public async Task<ActionResult> Borrar(int id)
        {
            var usuarioId = _usuariosServices.ObtenerUsuarioId();
            var tipoCuenta = await _tipoCuentasServices.ObtenerPorId(id, usuarioId);
            if (tipoCuenta == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            return View(tipoCuenta);
        }

        [HttpPost]
        [ServiceFilter(typeof(GlobalExceptionFilter))]
        public async Task<ActionResult> BorrarTipoCuenta(int id, int UsuarioId)
        {

            var tipoCuentaExiste = await _tipoCuentasServices.ObtenerPorId(id, UsuarioId);
            if (tipoCuentaExiste == null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await _tipoCuentasServices.Borrar(id);
            return Ok();
        }
    }


}