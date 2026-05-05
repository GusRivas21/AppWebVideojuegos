using AppWeb.Coll;
using AppWeb.Data;
using AppWeb.Models;
using AppWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using System.Security.Cryptography;
using System.Text;

namespace AppWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Registro()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existeUsuario = await _context.Usuarios.AnyAsync(u => u.Correo == model.Correo);
                if (existeUsuario)
                {
                    ViewBag.Error = "El correo ya está registrado.";
                    return View(model);
                }

                // Generar salt
                string salt = Guid.NewGuid().ToString();
                string saltedPassword = salt + model.Contrasena;

                byte[] hashBytes;
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes = Encoding.Unicode.GetBytes(saltedPassword);
                    hashBytes = sha256.ComputeHash(inputBytes);
                }

                var nuevoUsuario = new Usuario
                {
                    Nombre = model.Nombre,
                    Correo = model.Correo,
                    salt = salt,
                    Contrasena = hashBytes,
                    FechaRegistro = DateTime.Now,
                    IdRol = 2 // Asignar rol de Cliente por defecto
                };

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "¡Registro exitoso! Ya puedes iniciar sesión.";
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [SessionAuthorize]
        public IActionResult Dashboard()
        {
            var rol = HttpContext.Session.GetInt32("IdRol");
            if (rol == 1) 
            {
                // Si es admin, mostrar el dashboard
                ViewBag.ListaCategorias = _context.Categorias.ToList();
                ViewBag.TotalJuegos = _context.Videojuegos.Count();
                ViewBag.TotalVentas = _context.Detalle_Compra.Count(); // O la métrica de ventas que prefieras
                
                return View();
            }
            else if (rol == 2)
            {
                return RedirectToAction("Index", "Cliente");
            }

            return RedirectToAction("Login");
        }

        public IActionResult ObtenerDatos(string categoria)
        {
            var query = from v in _context.Videojuegos
                        join c in _context.Categorias
                        on v.idcategoria equals c.idcategoria
                        select new {c.categoria};

            if(!string.IsNullOrEmpty(categoria))
            {
                query = query.Where(x => x.categoria == categoria);
            }

            var data = query
                .GroupBy(x => x.categoria)
                .Select(g => new
                {
                    categoria = g.Key,
                    total = g.Count()
                }).ToList();

            return Json(data);
        }

        public IActionResult ObtenerDatosClasificacion()
        {
            var data = _context.Videojuegos
                .GroupBy(v => v.Edad)
                .Select(g => new
                {
                    clasificacion = g.Key == null ? "Sin Clasificar" : "+" + g.Key,
                    total = g.Count()
                })
                .OrderBy(x => x.clasificacion)
                .ToList();

            return Json(data);
        }

        public IActionResult ObtenerDatosPromocion()
        {
            var data = _context.Videojuegos
                .GroupBy(v => v.Promocion)
                .Select(g => new
                {
                    estado = g.Key ? "En Oferta" : "Precio Regular",
                    total = g.Count()
                }).ToList();

            return Json(data);
        }

        public IActionResult ObtenerDatosRegistros()
        {
            var rawData = _context.Usuarios
                .GroupBy(u => new { u.FechaRegistro.Year, u.FechaRegistro.Month })
                .Select(g => new
                {
                    Anio = g.Key.Year,
                    Mes = g.Key.Month,
                    Total = g.Count()
                })
                .OrderBy(x => x.Anio)
                .ThenBy(x => x.Mes)
                .ToList();

            var data = rawData.Select(x => new
            {
                mes = new DateTime(x.Anio, x.Mes, 1).ToString("MMMM yyyy", new System.Globalization.CultureInfo("es-ES")),
                total = x.Total
            }).ToList();

            return Json(data);
        }

        [SessionAuthorize]
        public async Task<IActionResult> DetalleVentas(DateTime? desde, DateTime? hasta, string cliente, string videojuego, int pagina=1)
        {
            int paginador = 12;

            var query = _context.Detalle_Compra
                .Include(d => d.Compra)
                .ThenInclude(c => c.Usuarios)
                .Include(d => d.VideoJuegos)
                .AsQueryable();

            if (desde.HasValue)
            {
                query = query.Where(d => d.FechaHoraTransaccion >= desde.Value);
            }
            if (hasta.HasValue)
            {
                query = query.Where(d => d.FechaHoraTransaccion < hasta.Value.Date.AddDays(1));
            }
            if (!string.IsNullOrEmpty(cliente))
            {
                query = query.Where(d => d.Compra.Usuarios.Nombre.Contains(cliente));
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
                .Select(d => new VentaViewModel
                {
                    Id = d.Id,
                    FechaCompra = d.FechaHoraTransaccion,
                    VideoJuegosId = d.VideoJuegosId,
                    Titulo = d.VideoJuegos.Titulo,
                    UsuarioId = d.Compra.UsuarioId,
                    NombreUsuario = d.Compra.Usuarios.Nombre,
                    Cantidad = d.Cantidad,
                    Total = d.Total,
                    EstadoCompra = d.EstadoCompra,
                    FechaHoraTransaccion = d.FechaHoraTransaccion,
                    CodigoTransaccion = d.CodigoTransaccion
                }).ToListAsync();

            ViewBag.TotalPaginas = (int)Math.Ceiling((double)total_registros / paginador);
            ViewBag.PaginaActual = pagina;
            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;
            ViewBag.Cliente = cliente;
            ViewBag.Videojuego = videojuego;
    
            return View(data);
        }

        [SessionAuthorize]
        public async Task<IActionResult> ExportarPDF(DateTime? desde, DateTime? hasta, string cliente, string videojuego)
        {
            var query = _context.Detalle_Compra
                .Include(d => d.Compra)
                .ThenInclude(c => c.Usuarios)
                .Include(d => d.VideoJuegos)
                .AsQueryable();

            if (desde.HasValue)
            {
                query = query.Where(d => d.FechaHoraTransaccion >= desde.Value);
            }
            if (hasta.HasValue)
            {
                query = query.Where(d => d.FechaHoraTransaccion < hasta.Value.Date.AddDays(1));
            }
            if (!string.IsNullOrEmpty(cliente))
            {
                query = query.Where(d => d.Compra.Usuarios.Nombre.Contains(cliente));
            }
            if (!string.IsNullOrEmpty(videojuego))
            {
                query = query.Where(d => d.VideoJuegos.Titulo.Contains(videojuego));
            }

            var data = await query
                .OrderByDescending(d => d.FechaHoraTransaccion)
                .Select(d => new VentaViewModel
                {
                    Id = d.Id,
                    FechaCompra = d.FechaHoraTransaccion,
                    VideoJuegosId = d.VideoJuegosId,
                    Titulo = d.VideoJuegos.Titulo,
                    UsuarioId = d.Compra.UsuarioId,
                    NombreUsuario = d.Compra.Usuarios.Nombre,
                    Cantidad = d.Cantidad,
                    Total = d.Total,
                    EstadoCompra = d.EstadoCompra,
                    FechaHoraTransaccion = d.FechaHoraTransaccion,
                    CodigoTransaccion = d.CodigoTransaccion
                }).ToListAsync();

            ViewBag.Desde = desde;
            ViewBag.Hasta = hasta;
            ViewBag.Cliente = cliente;
            ViewBag.Videojuego = videojuego;

            return new ViewAsPdf("PdfVentas", data)
            {
                FileName = "ReporteVentas.pdf",
                CustomSwitches = "--footer-center \"Página [page] de [topage]\" --footer-font-size 10"
            };
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Login model) // login o index, weno tengo que revisar
        {
            if (ModelState.IsValid)
            {
                var user = _context.Usuarios
                    .FirstOrDefault(u => u.Correo == model.correo);

                if (user != null && user.salt != null && user.Contrasena != null)
                {
                    string saltedPassword = user.salt + model.Contrasena;

                    using (SHA256 sha256 = SHA256.Create())
                    {
                        byte[] inputBytes = Encoding.Unicode.GetBytes(saltedPassword);
                        byte[] hashBytes = sha256.ComputeHash(inputBytes);

                        if (hashBytes.SequenceEqual(user.Contrasena))
                        {
                            HttpContext.Session.SetString("usuario", user.Nombre);
                            HttpContext.Session.SetInt32("IdRol", user.IdRol);
                            if (user.IdRol == 1)
                            {
                                return RedirectToAction("Dashboard", "Account");
                            }
                            else if (user.IdRol == 2)
                            {
                                return RedirectToAction("Index", "Cliente");
                            }
                        }
                    }
                }
                ViewBag.Error = "Credenciales Incorrectas";
            }
            return View(model);
        }

        // GET: AccountController/Create
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [SessionAuthorize]
        public async Task<IActionResult> UsuariosRegistrados()
        {
            var rol = HttpContext.Session.GetInt32("IdRol");
            if (rol != 1)
            {
                return RedirectToAction("Login");
            }

            var usuarios = await _context.Usuarios.Include(u => u.Roles).ToListAsync();
            return View(usuarios);
        }
    }
}
