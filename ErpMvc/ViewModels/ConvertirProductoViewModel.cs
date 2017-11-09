namespace ErpMvc.ViewModels
{
    public class ConvertirProductoViewModel
    {
        public int CentroDeCostoId { get; set; }

        public int OrigenId{ get; set; }

        public decimal Cantidad { get; set; }

        public int UnidadDeMedidaId { get; set; }

        public int DestinoId{ get; set; } 
    }
}