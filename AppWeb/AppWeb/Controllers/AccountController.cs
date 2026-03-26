using AppWeb.Data;
using AppWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(Login model) // login o index, weno tengo que revisar
        {
            //var user = _context.Usuarios
            //    .FirstOrDefault(u => u.Correo == model.correo && u.Contrasena == model.Contrasena);

            //if(user != null)
            //{
            //    HttpContext.Session.SetString("usuario", user.Nombre);
            //    Console.WriteLine("usuario " + user.Nombre);
            //    return RedirectToAction("Index", "Home");
            //}

            //ViewBag.Error = "Credenciales Incorrectas";
            //return View();

            var user = _context.Usuarios
                .FirstOrDefault(u => u.Correo == model.correo);


            if (user == null)
            { 
                string saltedPassword = user.Salt + model.Contrasena;

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(saltedPassword);
                    byte[] hashBytes = sha256.ComputeHash(inputBytes);

                    if (hashBytes.SequenceEqual(user.Contrasena))
                    { 
                        HttpContext.Session.SetString("usuario", user.Nombre);
                        return RedirectToAction("Index", "Home");
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
