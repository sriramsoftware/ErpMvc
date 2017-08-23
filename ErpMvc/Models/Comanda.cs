using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using ComercialCore.Models;
using CompraVentaCore.Models;
using ContabilidadCore.Models;
using SeguridadCore.Models;

namespace ErpMvc.Models
{
    [Table("rst_comandas")]
    public class Comanda
    {
        public int Id { get; set; }

        public int? VentaId { get; set; }

        public virtual Venta Venta { get; set; }

        public int DiaContableId { get; set; }

        public virtual DiaContable DiaContable { get; set; }

        public DateTime Fecha { get; set; }

        public int VendedorId { get; set; }

        public virtual Vendedor Vendedor { get; set; }

        public int PuntoDeVentaId { get; set; }

        public virtual PuntoDeVenta PuntoDeVenta { get; set; }

        public int CantidadPersonas { get; set; }

        public virtual ICollection<Orden> Comensales { get; set; }

        public EstadoDeVenta EstadoDeVenta { get; set; }

        public string UsuarioId { get; set; }

        public virtual Usuario Usuario { get; set; }

        public virtual ICollection<DetalleDeComanda> Detalles { get; set; }

        public Comanda()
        {
            Detalles = new HashSet<DetalleDeComanda>();
            Comensales = new HashSet<Orden>();
        }

        [NotMapped]
        public string Resumen
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(PuntoDeVenta.Nombre)
                    .Append("\n")
                    .Append(Vendedor.NombreCompleto)
                    .Append("\n");
                foreach (var el in Detalles)
                {
                    sb.Append(el.Elaboracion.Nombre + ",");
                }
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            return Resumen;
        }
    }
}