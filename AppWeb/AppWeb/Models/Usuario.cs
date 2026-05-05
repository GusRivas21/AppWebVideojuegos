using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppWeb.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [StringLength(255)]
        public byte[]? Contrasena { get; set; }

        [StringLength(255)]
        public string? salt { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [ForeignKey("IdRol")]
        public Roles Roles { get; set; }

        public int IdRol { get; set; }
    }
}
