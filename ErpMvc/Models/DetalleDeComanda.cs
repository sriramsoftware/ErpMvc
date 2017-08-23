using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using CompraVentaCore.Models;

namespace ErpMvc.Models
{
    [Table("rst_detalles_de_comandas")]
    public class DetalleDeComanda
    {
        public int Id { get; set; }

        public int ComandaId { get; set; }

        public virtual Comanda Comanda { get; set; }

        public int ElaboracionId { get; set; }

        public virtual Elaboracion Elaboracion { get; set; }

        public int Cantidad { get; set; }

        public virtual ICollection<AgregadoDeComanda> Agregados { get; set; }

        public virtual ICollection<OrdenPorDetalle> Ordenes { get; set; }

        public DetalleDeComanda()
        {
            Agregados = new HashSet<AgregadoDeComanda>();
            Ordenes = new HashSet<OrdenPorDetalle>();
        }

        [NotMapped]
        public string Detalle
        {
            get
            {
                if (Elaboracion == null)
                {
                    return "";
                }
                if (Agregados == null)
                {
                    return Elaboracion.Nombre;
                }
                return Elaboracion.Nombre +
                       (Agregados.Any() ? " " + String.Join(",", Agregados.Select(a => a.Agregado.Producto.Nombre)) : "");
            }
        }
    }
}