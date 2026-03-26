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
        public async Task<IActionResult> Create(Videojuego juego, IFormFile archivoImagen)
        {
            if (!ModelState.IsValid)
                return View(juego);

            juego.FechaRegistro = DateTime.Now;

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

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            
            var juego = await _context.Videojuegos.FindAsync(id);
            if (juego == null) return NotFound();
            return View(juego);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Videojuego juego, IFormFile? archivoImagen)
        {
            if (id != juego.Id) 
                return NotFound();

            var juegoBD = await _context.Videojuegos.FindAsync(id);
            if (juegoBD == null) return NotFound();

            if (ModelState.IsValid)
            {
                juegoBD.Titulo = juego.Titulo;
                juegoBD.Precio = juego.Precio;
                juegoBD.imagen = juego.imagen;
                juegoBD.Categoria = juego.Categoria;
                juegoBD.Descripcion = juego.Descripcion;
                juegoBD.Edad = juego.Edad;
                juegoBD.Promocion = juego.Promocion;

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

        public async Task<IActionResult> NuevosJuego()
        {
            var juegos = await _context.Videojuegos
                .OrderByDescending(v => v.FechaRegistro)
                .Take(15)
                .ToListAsync();
            return View("~/Views/VideoJuegos/NuevosJuegos.cshtml", juegos);
        }

        public async Task<IActionResult> Promocion()
        {
            var juegos = await _context.Videojuegos
                .Where(v => v.Promocion == true)
                .ToListAsync();
            return View("~/Views/VideoJuegos/Promociones.cshtml", juegos);
        }

        public async Task<IActionResult> Categoria(string Categoria)
        {
            ViewBag.TodasLasCategorias = await _context.Videojuegos
                .Select(v => v.Categoria)
                .Distinct()
                .ToListAsync();

            var juegos = string.IsNullOrEmpty(Categoria)
                ? await _context.Videojuegos.ToListAsync()
                : await _context.Videojuegos.Where(v => v.Categoria == Categoria).ToListAsync();

            return View("~/Views/Categorias/Index.cshtml", juegos);
        }
    }
}
