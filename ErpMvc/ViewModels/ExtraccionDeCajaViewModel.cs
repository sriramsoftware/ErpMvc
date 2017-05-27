using System.Web.Mvc;

namespace ErpMvc.ViewModels
{
    public class ExtraccionDeCajaViewModel
    {
        [Remote("SePuedeExtraer", "Caja", AdditionalFields = "Importe", ErrorMessage = "No existe saldo suficiente en caja para realizar la extracción")]
        public decimal Importe { get; set; }

        public string Observaciones { get; set; }
    }
}