using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpMvc.Models
{
    [Table("rst_ordenes_comanda")]
    public class Orden
    {
        public int Id { get; set; }

        public int Numero { get; set; }

        public Comensal Comensal { get; set; }

        public int ComandaId { get; set; }

        public virtual Comanda Comanda { get; set; }

        public virtual ICollection<OrdenPorDetalle> Detalles { get; set; }

        public Orden()
        {
            Detalles = new HashSet<OrdenPorDetalle>();
        }
    }
}