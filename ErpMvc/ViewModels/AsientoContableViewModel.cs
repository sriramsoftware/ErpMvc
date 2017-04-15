namespace ErpMvc.ViewModels
{
    public class AsientoContableViewModel
    {
        public int CuentaCreditoId { get; set; }

        public int CuentaDebitoId { get; set; }

        public decimal Importe { get; set; }

        public string Observaciones { get; set; } 
    }
}