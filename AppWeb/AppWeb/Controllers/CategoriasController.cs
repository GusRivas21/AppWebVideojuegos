using AppWeb.Coll;
using AppWeb.Data;
using AppWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppWeb.Controllers
{
    [SessionAuthorize]
    public class CategoriasController : Controller
    {
        private readonly AppDbContext _context;

        public CategoriasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Categorias
        public async Task<IActionResult> Index()
        {
            var rol = HttpContext.Session.GetInt32("IdRol");
            if (rol != 1) return RedirectToAction("Login", "Account");

            return View(await _context.Categorias.ToListAsync());
        }

        // GET: Categorias/Create
        public IActionResult Create()
        {
            var rol = HttpContext.Session.GetInt32("IdRol");
            if (rol != 1) return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: Categorias/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("idcategoria,categoria")] Categoria objCategoria)
        {
            if (ModelState.IsValid)
            {
                _context.Add(objCategoria);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(objCategoria);
        }

        // GET: Categorias/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var categoriaEncontrada = await _context.Categorias.FindAsync(id);
            if (categoriaEncontrada == null) return NotFound();

            return View(categoriaEncontrada);
        }

        // POST: Categorias/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("idcategoria,categoria")] Categoria objCategoria)      
        {
            if (id != objCategoria.idcategoria) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(objCategoria);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoriaExists(objCategoria.idcategoria)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(objCategoria);
        }

        // GET: Categorias/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var categoria = await _context.Categorias
                .FirstOrDefaultAsync(m => m.idcategoria == id);
            if (categoria == null) return NotFound();

            return View(categoria);
        }

        // POST: Categorias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria != null)
            {
                _context.Categorias.Remove(categoria);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(e => e.idcategoria == id);
        }
    }
}
