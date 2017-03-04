using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AlmacenCore.Models;

namespace ErpMvc.ViewModels
{
    public class ProductoConcretoViewModel
    {
        public ProductoConcreto Producto { get; set; }

        public ExistenciaCentroDeCosto Existencia { get; set; }

        public ICollection<MovimientoDeProducto> Movimientos { get; set; } 
    }
}