using System.Collections.Generic;
using AlmacenCore.Models;

namespace ErpMvc.ViewModels
{
    public class ResumenMermaViewModel
    {
        public ProductoConcreto Producto { get; set; }

        public decimal Cantidad { get; set; }

        public List<string> Detalles { get; set; }

        public ResumenMermaViewModel()
        {
             Detalles = new List<string>();
        }
    }
}