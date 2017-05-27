using ContabilidadCore.Models;

namespace ErpMvc.Models
{
    public class OtrosGastos
    {
        public int Id { get; set; }

        public int DiaContableId { get; set; }

        public virtual DiaContable DiaContable { get; set; }

        public decimal Importe { get; set; }

        public int ConceptoDeGastoId { get; set; }

        public virtual ConceptoDeGasto ConceptoDeGasto { get; set; }

        public bool PagadoPorCaja { get; set; }
    }
}