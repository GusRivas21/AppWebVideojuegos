using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppWeb.Models
{
    public class Videojuego
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Titulo { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Precio { get; set; }

        [Required]
        [StringLength(100)]
        public string Categoria { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        public string? imagen { get; set; }

        public ICollection<Compra>? Compras { get; set; }
    }
}
