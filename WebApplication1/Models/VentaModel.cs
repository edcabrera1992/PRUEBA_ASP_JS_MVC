namespace WebApplication1.Models
{
    public class VentaModel
    {
        public int id { get; set; }
        public DateTime FechaVenta { get; set; }
        public decimal MontoTotal { get; set; }
        public int ClienteId { get; set; }
        public string?  nombre { get; set; }
        //  public PagoModel? pago { get; set; }
        public List<PagoModel> pagos { get; set; }
    }
}
