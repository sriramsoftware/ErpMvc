using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpMvc.Models
{
    [Table("rst_orden_por_detalle")]
    public class OrdenPorDetalle
    {
        public int Id { get; set; }

        public int DetalleDeComandaId { get; set; }

        public virtual DetalleDeComanda DetalleDeComanda { get; set; }

        public int OrdenId { get; set; }

        public virtual Orden Orden { get; set; }

        public virtual ICollection<Anotacion> Anotaciones { get; set; }

        public OrdenPorDetalle()
        {
            Anotaciones = new HashSet<Anotacion>();
        }
        
    }
}