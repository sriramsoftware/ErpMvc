using System;

namespace ErpMvc.ViewModels
{
    public class ConsumoViewModel
    {
        public int ProductoId { get; set; }

        public decimal Cantidad { get; set; }

        public string Unidad { get; set; }

        public string Producto { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFin { get; set; }
    }
}