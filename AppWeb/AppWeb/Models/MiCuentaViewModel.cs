using System.ComponentModel.DataAnnotations;

namespace AppWeb.Models
{
    public class MiCuentaViewModel
    {
        [Required(ErrorMessage = "El nombre es requerido")]
        public string Nombre { get; set; }

        public string Correo { get; set; } // Solo lectura

        [DataType(DataType.Password)]
        public string? NuevaContrasena { get; set; }
    }
}