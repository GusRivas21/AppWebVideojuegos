using System.ComponentModel.DataAnnotations;

namespace AppWeb.Models
{
    public class Roles
    {
        [Key]
        public int IdRol { get; set; }

        [Required]
        public string? Rol { get; set; }
    }
}
