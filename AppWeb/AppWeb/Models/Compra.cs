using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppWeb.Models
{
    public class Compra
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime FechaCompra { get; set; } = DateTime.Now;

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuarios { get; set; }

        [Required]
        public int VideojuegoId { get; set; }

        [ForeignKey("VideojuegoId")]
        public Videojuego Videojuegos { get; set; }
    }
}
