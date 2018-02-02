using CompraVentaCore.Models;

namespace ErpMvc.Models
{
    public class SeleccionCompra
    {
        public int Id { get; set; }

        public int CompraId { get; set; }

        public virtual Compra Compra { get; set; } 
    }
}