using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CompraVentaCore.Models;

namespace ErpMvc.Models
{
    [Table("rst_agregados_de_comandas")]
    public class AgregadoDeComanda
    {
        [Key]
        [Column(Order = 1)]
        public int DetalleDeComandaId { get; set; }

        public virtual DetalleDeComanda DetalleDeComanda { get; set; }

        [Key]
        [Column(Order = 2)]
        public int AgregadoId { get; set; }

        public virtual Agregado Agregado { get; set; }

        public int Cantidad { get; set; }
    }
}