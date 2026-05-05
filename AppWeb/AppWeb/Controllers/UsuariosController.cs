using AppWeb.Coll;
using AppWeb.Data;
using AppWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AppWeb.Controllers
{
    [SessionAuthorize]
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetInt32("IdRol");
            if (rol != 1) return RedirectToAction("Login", "Account");

            var usuarios = await _context.Usuarios.Include(u => u.Roles).ToListAsync();
            return View(usuarios);
        }

        // GET: Usuarios/Create
        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetInt32("IdRol");
            if (rol != 1) return RedirectToAction("Login", "Account");

            ViewBag.IdRol = new SelectList(_context.Set<Roles>(), "IdRol", "Rol");
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Usuario objUsuario, string password)
        {
            // Removemos la validación del objeto Roles ya que se carga por IdRol
            ModelState.Remove("Roles");

            if (ModelState.IsValid && !string.IsNullOrEmpty(password))
            {
                // Lógica de hash similar a AccountController
                string salt = Guid.NewGuid().ToString();
                string saltedPassword = salt + password;

                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes = Encoding.Unicode.GetBytes(saltedPassword);
                    objUsuario.Contrasena = sha256.ComputeHash(inputBytes);
                }
                objUsuario.salt = salt;
                objUsuario.FechaRegistro = DateTime.Now;

                _context.Add(objUsuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.IdRol = new SelectList(_context.Set<Roles>(), "IdRol", "Rol", objUsuario.IdRol);
            return View(objUsuario);
        }

        // GET: Usuarios/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            ViewBag.IdRol = new SelectList(_context.Set<Roles>(), "IdRol", "Rol", usuario.IdRol);
            return View(usuario);
        }

        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario objUsuario, string? nuevaContrasena)
        {
            if (id != objUsuario.Id) return NotFound();

            // Removemos la validación del objeto Roles ya que se carga por IdRol
            ModelState.Remove("Roles");

            if (ModelState.IsValid)
            {
                try
                {
                    var userInDb = await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
                    if (userInDb == null) return NotFound();

                    objUsuario.salt = userInDb.salt;
                    objUsuario.Contrasena = userInDb.Contrasena;
                    objUsuario.FechaRegistro = userInDb.FechaRegistro;

                    if (!string.IsNullOrEmpty(nuevaContrasena))
                    {
                        string salt = Guid.NewGuid().ToString();
                        string saltedPassword = salt + nuevaContrasena;
                        using (SHA256 sha256 = SHA256.Create())
                        {
                            byte[] inputBytes = Encoding.Unicode.GetBytes(saltedPassword);
                            objUsuario.Contrasena = sha256.ComputeHash(inputBytes);
                        }
                        objUsuario.salt = salt;
                    }

                    _context.Update(objUsuario);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(objUsuario.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.IdRol = new SelectList(_context.Set<Roles>(), "IdRol", "Rol", objUsuario.IdRol);
            return View(objUsuario);
        }

        // GET: Usuarios/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var usuario = await _context.Usuarios
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null) return NotFound();

            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}
