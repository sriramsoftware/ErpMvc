using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CompraVentaCore.Models;

namespace ErpMvc.Models
{
    public class SeleccionVenta
    {
        public int Id{ get; set; }

        public int VentaId { get; set; }

        public virtual Venta Venta { get; set; }
    }
}
