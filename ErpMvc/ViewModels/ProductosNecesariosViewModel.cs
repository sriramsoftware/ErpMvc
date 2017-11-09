using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ErpMvc.ViewModels
{
    public class ProductosNecesariosViewModel
    {
        public int ProductoId { get; set; }

        public string Producto { get; set; }

        public decimal Cantidad { get; set; }

        public string Unidad { get; set; }

        public string Menus { get; set; }
    }
}
