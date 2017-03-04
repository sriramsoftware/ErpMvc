using System;
using CompraVentaCore.Models;
using ContabilidadCore.Models;
using HumanResourcesCore.Models;

namespace ErpMvc.Models
{
    public class Asistencia
    {
        public int Id { get; set; }

        public int VendedorId { get; set; }

        public virtual Vendedor Vendedor { get; set; }

        public int DiaContableId { get; set; }

        public virtual DiaContable DiaContable { get; set; }

        public DateTime? Entrada { get; set; }

        public DateTime? Salida { get; set; }
    }
}