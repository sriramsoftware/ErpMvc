using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErpMvc.Models
{
    [Table("rst_anotaciones")]
    public class Anotacion
    {
        public int Id { get; set; }

        public string Abreviatura { get; set; }

        public string Descripcion { get; set; }

        public virtual ICollection<OrdenPorDetalle> OrdenPorDetalles { get; set; }

        public Anotacion()
        {
            OrdenPorDetalles = new HashSet<OrdenPorDetalle>();
        }
    }
}