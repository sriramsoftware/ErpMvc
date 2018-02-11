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

        public decimal Importe { get; set; }

        public decimal ImporteSeleccionado { get; set; }

        public decimal PorCiento
        {
            get { return Importe > 0? ImporteSeleccionado*100/Importe:0; }
        }
    }
}
