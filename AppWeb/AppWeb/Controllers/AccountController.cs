using AppWeb.Coll;
using AppWeb.Data;
using AppWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [SessionAuthorize]
        public IActionResult Dashboard()
        {
            ViewBag.ListaCategorias = _context.Categorias.ToList();
            return View();
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

        [SessionAuthorize]
        public async Task<IActionResult> DetalleVentas(DateTime? desde, DateTime? hasta, int pagina=1)
        {
            int paginador = 12;

            var query = _context.Detalle_Compra
                .Include(d => d.Compra)
                .Include(d => d.VideoJuegos)
                .AsQueryable();

            if (desde.HasValue)
            {
                query = query.Where(d => d.FechaHoraTransaccion >= desde.Value);
            }
            if (hasta.HasValue)
            {
                query = query.Where(d => d.FechaHoraTransaccion <= hasta.Value);
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
    
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(Login model) // login o index, weno tengo que revisar
        {
            var user = _context.Usuarios
                .FirstOrDefault(u => u.Correo == model.correo);

            if (user != null)
            { 
                string saltedPassword = user.salt + model.Contrasena;

                using (SHA256 sha256 = SHA256.Create())
                {
                    //byte[] inputBytes = Encoding.UTF8.GetBytes(saltedPassword);
                    byte[] inputBytes = Encoding.Unicode.GetBytes(saltedPassword);
                    byte[] hashBytes = sha256.ComputeHash(inputBytes);

                    //Console.WriteLine("Salt BD" + user.salt);
                    //Console.WriteLine("Password input" + model.Contrasena);
                    //Console.WriteLine("Salted: " + (user.salt + model.Contrasena));

                    //Console.WriteLine("Hash generado: " + Convert.ToBase64String(hashBytes));
                    //Console.WriteLine("Hash BD: " + Convert.ToBase64String(user.Contrasena));

                    if (hashBytes.SequenceEqual(user.Contrasena))
                    { 
                        HttpContext.Session.SetString("usuario", user.Nombre);
                        return RedirectToAction("Dashboard", "Account");
                    }
                }
            }
             ViewBag.Error = "Credenciales Incorrectas";
             return View();
        }

        // GET: AccountController/Create
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
