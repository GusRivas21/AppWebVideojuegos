using AppWeb.Data;
using AppWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult Login()
        {
            return View("Index");
        }

        [HttpPost]
        public IActionResult Login(Login model) // login o index, weno tengo que revisar
        {
            var user = _context.Usuarios
                .FirstOrDefault(u => u.Correo == model.correo && u.Contrasena == model.Contrasena);

            if(user != null)
            {
                HttpContext.Session.SetString("usuario", user.Nombre);
                Console.WriteLine("usuario " + user.Nombre);
                return RedirectToAction("Index", "Home");
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
