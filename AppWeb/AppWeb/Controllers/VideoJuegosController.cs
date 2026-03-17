using AppWeb.Data;
using AppWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace AppWeb.Controllers
{
    public class VideoJuegosController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            var juegos = await _context.Videojuegos.ToListAsync();
            return View(juegos);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Create(Videojuego juego, IFormFile archivoImagen)
        {
            if (!ModelState.IsValid)
                return View(juego);

            if (archivoImagen != null && archivoImagen.Length > 0)
            { 
                var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivoImagen.FileName);

                var ruta = Path.Combine(Directory.GetCurrentDirectory(),
                    "wwwroot/Imagenes", nombreArchivo);

                using (var stream = new FileStream(ruta, FileMode.Create))
                { 
                    await archivoImagen.CopyToAsync(stream);
                }

                juego.imagen = "/Imagenes/" + nombreArchivo;
            }

            _context.Videojuegos.Add(juego);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id, Videojuego juego, IFormFile? archivoImagen)
        {
            if (id == null) return NotFound();

            var juegoBD = await _context.Videojuegos.FindAsync(id);
            if (juegoBD == null) return NotFound();

            if (ModelState.IsValid)
            {
                juegoBD.Titulo = juego.Titulo;
                juegoBD.Precio = juego.Precio;
                juegoBD.Categoria = juego.Categoria;
                juegoBD.Descripcion = juego.Descripcion;

                if (archivoImagen != null && archivoImagen.Length > 0)
                {
                    if (!string.IsNullOrEmpty(juegoBD.imagen))
                    {
                        var rutaAnterior = Path.Combine(
                            Directory.GetCurrentDirectory(),
                            "wwwroot",
                            juegoBD.imagen.TrimStart('/')
                        );

                        if (System.IO.File.Exists(rutaAnterior))
                            System.IO.File.Delete(rutaAnterior);
                    }

                    var nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(archivoImagen.FileName);

                    var rutaNueva = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/Imagenes", nombreArchivo
                    );

                    using (var stream = new FileStream(rutaNueva, FileMode.Create))
                    {
                        await archivoImagen.CopyToAsync(stream);
                    }

                    juegoBD.imagen = "/Imagenes/" + nombreArchivo;
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(juegoBD);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Videojuego juego)
        {
            if (id != juego.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(juego);

            try
            {
                _context.Update(juego);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Videojuegos.Any(e => e.Id == juego.Id))
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var juego = await _context.Videojuegos.FirstOrDefaultAsync(m => m.Id == id);

            if (juego == null) return NotFound();

            return View(juego);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var juego = await _context.Videojuegos.FindAsync(id);

            if (juego != null)
            {
                _context.Videojuegos.Remove(juego);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
