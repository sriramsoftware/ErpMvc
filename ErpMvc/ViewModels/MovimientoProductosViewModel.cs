using System.Collections.Generic;
using CompraVentaCore.Models;

namespace ErpMvc.ViewModels
{
    public class MovimientoProductosViewModel
    {
        public int OrigenId { get; set; }

        public int DestinoId { get; set; }

        public ICollection<DetalleDeCompra> Productos { get; set; }

        public MovimientoProductosViewModel()
        {
            Productos= new HashSet<DetalleDeCompra>();
        }
    }
}