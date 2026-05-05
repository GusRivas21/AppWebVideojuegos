namespace AppWeb.Models
{
    public class PayPalOrderRequest
    {
        public int VideojuegoId { get; set; }
        public int Cantidad { get; set; }
    }

    public class PayPalCaptureRequest
    {
        public string OrderId { get; set; }
        public int VideojuegoId { get; set; }
        public int Cantidad { get; set; }
    }
}
