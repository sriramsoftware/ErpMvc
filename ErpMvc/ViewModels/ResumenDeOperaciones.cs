using System;

namespace ErpMvc.ViewModels
{
    public class ResumenDeOperaciones
    {
        public DateTime Fecha { get; set; }

        public string Detalle { get; set; }

        public string Tipo { get; set; }

        public decimal Importe { get; set; }
    }
}