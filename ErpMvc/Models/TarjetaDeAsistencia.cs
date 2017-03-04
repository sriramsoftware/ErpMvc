using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CompraVentaCore.Models;

namespace ErpMvc.Models
{
    public class TarjetaDeAsistencia
    {
        public int VendedorId { get; set; }

        public virtual Vendedor Vendedor { get; set; }

        public string Usuario { get; set; }

        public string Contraseña { get; set; }
    }
}
