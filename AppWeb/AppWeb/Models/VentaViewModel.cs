using System.ComponentModel.DataAnnotations;

namespace AppWeb.Models
{
    public class VentaViewModel
    {
        public int Id { get; set; }
        public DateTime FechaCompra { get; set; }
        public int UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
        public int VideoJuegosId { get; set; }
        public string Titulo { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
        public string EstadoCompra { get; set; }
        public DateTime FechaHoraTransaccion { get; set; }
        public string CodigoTransaccion { get; set; }
    }
}
