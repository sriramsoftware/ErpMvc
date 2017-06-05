using System;

namespace ErpMvc.ViewModels
{
    public class ParametrosMovProdViewModel
    {
        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }

        public int ProductoId { get; set; }

        public string Lugar { get; set; } 
    }
}