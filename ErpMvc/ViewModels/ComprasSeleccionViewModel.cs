using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContabilidadCore.Models;

namespace ErpMvc.ViewModels
{
    public class ComprasSeleccionViewModel
    {
        public DiaContable DiaContable { get; set; }

        public decimal Importe { get; set; }
    }
}
