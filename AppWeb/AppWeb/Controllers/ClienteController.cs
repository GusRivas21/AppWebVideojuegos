using AppWeb.Coll;
using AppWeb.Data;
using AppWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AppWeb.Controllers
{
    [SessionAuthorize]
    public class ClienteController : Controller
    {
        private readonly AppDbContext _context;
        private readonly PayPalClient _payPalClient;

        public ClienteController(AppDbContext context, PayPalClient payPalClient)
        {
            _context = context;
            _payPalClient = payPalClient;
        }

        // Vista principal (Juegos disponibles 10 en 10)
        public async Task<IActionResult> Index(int pagina = 1)
        {
            var rol = HttpContext.Session.GetInt32("IdRol");
            if (rol != 2) return RedirectToAction("Login", "Account");

            int paginador = 10;
            var query = _context.Videojuegos.AsQueryable();

            var total_registros = await query.CountAsync();
            var juegos = await query
                .OrderByDescending(j => j.Id)
                .Skip((pagina - 1) * paginador)
                .Take(paginador)
                .ToListAsync();

            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total_registros / paginador);
            ViewBag.PaginaActual = pagina;

            return View(juegos);
        }

        // Comprar juego - Vista de confirmación
        [HttpGet]
        public async Task<IActionResult> ConfirmarCompra(int id)
        {
            var juego = await _context.Videojuegos.FindAsync(id);
            if (juego == null) return RedirectToAction("Index");

            return View(juego);
        }

        [HttpPost]
        public async Task<IActionResult> CrearOrdenPayPal([FromBody] PayPalOrderRequest request)
        {
            var videojuego = await _context.Videojuegos.FindAsync(request.VideojuegoId);
            if (videojuego == null) return BadRequest();

            var total = videojuego.Precio * request.Cantidad;
            var orderId = await _payPalClient.CrearOrden(total);

            return Ok(new { id = orderId });
        }

        [HttpPost]
        public async Task<IActionResult> FinalizarCompraPayPal([FromBody] PayPalCaptureRequest request)
        {
            var success = await _payPalClient.CapturarOrden(request.OrderId);
            if (success)
            {
                var usuario = HttpContext.Session.GetString("usuario");
                var dbUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == usuario);
                var videojuego = await _context.Videojuegos.FindAsync(request.VideojuegoId);

                if (dbUser != null && videojuego != null)
                {
                    var compra = new Compra
                    {
                        FechaCompra = DateTime.Now,
                        UsuarioId = dbUser.Id
                    };
                    _context.Compras.Add(compra);
                    await _context.SaveChangesAsync();

                    var detalle = new DetalleCompra
                    {
                        VideoJuegosId = request.VideojuegoId,
                        Cantidad = request.Cantidad,
                        Total = videojuego.Precio * request.Cantidad,
                        EstadoCompra = "Completado",
                        FechaHoraTransaccion = DateTime.Now,
                        CodigoTransaccion = request.OrderId,
                        IdCompra = compra.Id
                    };
                    _context.Detalle_Compra.Add(detalle);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"Has adquirido {request.Cantidad} unidad(es) de '{videojuego.Titulo}' con éxito vía PayPal.";
                    return Ok();
                }
            }
            return BadRequest();
        }

        // Comprar juego - Proceso
        [HttpPost]
        public async Task<IActionResult> Comprar(int videojuegoId, int cantidad)
        {
            var usuario = HttpContext.Session.GetString("usuario");
            var dbUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == usuario);
            var videojuego = await _context.Videojuegos.FindAsync(videojuegoId);

            if (dbUser != null && videojuego != null && cantidad > 0)
            {
                var compra = new Compra
                {
                    FechaCompra = DateTime.Now,
                    UsuarioId = dbUser.Id
                };

                _context.Compras.Add(compra);
                await _context.SaveChangesAsync();

                var detalle = new DetalleCompra
                {
                    VideoJuegosId = videojuegoId,
                    Cantidad = cantidad,
                    Total = videojuego.Precio * cantidad,
                    EstadoCompra = "Completado",
                    FechaHoraTransaccion = DateTime.Now,
                    CodigoTransaccion = Guid.NewGuid().ToString(),
                    IdCompra = compra.Id
                };

                _context.Detalle_Compra.Add(detalle);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = $"Has adquirido {cantidad} unidad(es) de '{videojuego.Titulo}' con éxito.";
            }

            return RedirectToAction("Index");
        }

        // Juegos comprados con filtros
        public async Task<IActionResult> MisCompras(DateTime? desde, DateTime? hasta, string videojuego, int pagina = 1)
        {
            var usuario = HttpContext.Session.GetString("usuario");
            var dbUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == usuario);

            if (dbUser == null) return RedirectToAction("Login", "Account");

            int paginador = 10;

            var query = _context.Detalle_Compra
                .Include(d => d.Compra)
                .Include(d => d.VideoJuegos)
                .Where(d => d.Compra.UsuarioId == dbUser.Id)
                .AsQueryable();

            if (desde.HasValue)
            {
                query = query.Where(d => d.FechaHoraTransaccion >= desde.Value);
            }
            if (hasta.HasValue)
            {
                query = query.Where(d => d.FechaHoraTransaccion <= hasta.Value);
            }
            if (!string.IsNullOrEmpty(videojuego))
            {
                query = query.Where(d => d.VideoJuegos.Titulo.Contains(videojuego));
            }

            var total_registros = await query.CountAsync();
            var data = await query
                .OrderByDescending(d => d.FechaHoraTransaccion)
                .Skip((pagina - 1) * paginador)
                .Take(paginador)
                .ToListAsync();

            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total_registros / paginador);
            ViewBag.PaginaActual = pagina;
            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;
            ViewBag.Videojuego = videojuego;

            return View(data);
        }

        // Mi Cuenta
        [HttpGet]
        public async Task<IActionResult> MiCuenta()
        {
            var usuario = HttpContext.Session.GetString("usuario");
            var dbUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == usuario);

            if (dbUser == null) return RedirectToAction("Login", "Account");

            var model = new MiCuentaViewModel
            {
                Nombre = dbUser.Nombre,
                Correo = dbUser.Correo
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MiCuenta(MiCuentaViewModel model)
        {
            var usuarioSession = HttpContext.Session.GetString("usuario");
            var dbUser = await _context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == usuarioSession);

            if (dbUser == null) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                dbUser.Nombre = model.Nombre;

                if (!string.IsNullOrEmpty(model.NuevaContrasena))
                {
                    string saltedPassword = dbUser.salt + model.NuevaContrasena;
                    using (SHA256 sha256 = SHA256.Create())
                    {
                        byte[] inputBytes = Encoding.Unicode.GetBytes(saltedPassword);
                        dbUser.Contrasena = sha256.ComputeHash(inputBytes);
                    }
                }

                _context.Usuarios.Update(dbUser);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("usuario", dbUser.Nombre); // Actualizar sesión si cambió el nombre
                ViewBag.Success = "Cuenta actualizada con éxito.";
            }

            model.Correo = dbUser.Correo; // Asegurar que el correo se mantenga
            return View(model);
        }
    }
}