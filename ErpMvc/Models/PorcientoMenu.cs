using System.ComponentModel.DataAnnotations;
using CompraVentaCore.Models;

namespace ErpMvc.Models
{
    public class PorcientoMenu
    {
        [Key]
        public int ElaboracioId { get; set; }

        public virtual Elaboracion Elaboracion { get; set; }

        public bool SeCalcula { get; set; } 
    }
}