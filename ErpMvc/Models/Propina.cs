using System.ComponentModel.DataAnnotations;
using CompraVentaCore.Models;

namespace ErpMvc.Models
{
    public class Propina
    {
        [Key]
        public int VentaId { get; set; }

        public virtual Venta Venta { get; set; }

        public decimal Importe { get; set; } 
    }
}