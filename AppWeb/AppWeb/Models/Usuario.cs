using System.ComponentModel.DataAnnotations;

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

        [Required]
        [StringLength(255)]
        public byte[] Contrasena { get; set; }

        [StringLength(255)]
        public string Salt { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

  
    }
}
