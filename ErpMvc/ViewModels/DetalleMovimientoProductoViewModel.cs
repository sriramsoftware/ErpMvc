using System;

namespace ErpMvc.ViewModels
{
    public class DetalleMovimientoProductoViewModel
    {
        public string Lugar { get; set; }

        public DateTime Fecha { get; set; }

        public string TipoDeMovimiento { get; set; }

        public string Detalle { get; set; }

        public decimal Cantidad { get; set; }

        public string Unidad { get; set; }

        public string Usuario { get; set; }
    }
}