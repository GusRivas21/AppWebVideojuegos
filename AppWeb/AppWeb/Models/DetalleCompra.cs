using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppWeb.Models
{
    public class DetalleCompra
    {
        [Key]
        public int Id { get; set; }

        public int VideoJuegosId { get; set; }

        [ForeignKey("VideoJuegosId")]
        public Videojuego VideoJuegos { get; set; }


        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        public string EstadoCompra { get; set; }

        public DateTime FechaHoraTransaccion { get; set; }

        public string CodigoTransaccion { get; set; }

        public int IdCompra { get; set; }

        [ForeignKey("IdCompra")]
        public Compra Compra { get; set; }
    }
}
