using System;

namespace ErpMvc.ViewModels
{
    public class ConsumoViewModel
    {
        public int ProductoId { get; set; }

        public string Cantidad { get; set; }

        public string Producto { get; set; }

        public DateTime Fecha { get; set; }
    }
}