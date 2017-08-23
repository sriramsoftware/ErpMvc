using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpMvc.ViewModels
{
    public class ResumenProductoVM
    {
        public DateTime Fecha { get; set; }

        public int Comanda { get; set; }

        public decimal Cantidad { get; set; }
    }
}
