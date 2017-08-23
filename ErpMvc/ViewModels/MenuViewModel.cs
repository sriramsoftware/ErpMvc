namespace ErpMvc.ViewModels
{
    public class MenuViewModel
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public decimal Precio { get; set; }

        public int CentroDeCostoId { get; set; } 
    }
}