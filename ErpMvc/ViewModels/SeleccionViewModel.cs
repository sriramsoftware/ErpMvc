using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContabilidadCore.Models;

namespace ErpMvc.ViewModels
{
    public class SeleccionViewModel
    {
        public int Id { get; set; }

        public DiaContable DiaContable { get; set; }

        public decimal Ventas { get; set; }

        public decimal Importe { get; set; }

        public decimal ImporteSeleccionado { get; set; }

        public decimal ConComprobante { get; set; }

        public decimal SinComprobante { get; set; }

        public decimal PorCiento
        {
            get { return Math.Round(Importe > 0? ImporteSeleccionado*100/Importe:0,2); }
        }

        public decimal PorCientoPorVentas
        {
            get { return Math.Round(Ventas > 0 ? ImporteSeleccionado * 100 / Ventas : 0, 2); }
        }

        public decimal PorCientoComprobante
        {
            get { return Math.Round(ImporteSeleccionado > 0 ? ConComprobante * 100 / ImporteSeleccionado : 0,2); }
        }

        public decimal PorCientoSinComprobante
        {
            get { return Math.Round(ImporteSeleccionado > 0 ? SinComprobante * 100 / ImporteSeleccionado : 0,2); }
        }
    }
}
