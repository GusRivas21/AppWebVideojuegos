using System.ComponentModel.DataAnnotations;

namespace AppWeb.Models
{
    public class Categoria
    {
        [Key]
        public int idcategoria { get; set; }
        public string categoria { get; set; }
    }
}
