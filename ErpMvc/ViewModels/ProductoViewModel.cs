using System.ComponentModel.DataAnnotations;
using AlmacenCore.Models;
using ComercialCore.Models;

namespace ErpMvc.ViewModels
{
    public class ProductoViewModel
    {
        [Required]
        public string Nombre { get; set; }

        public int? ProductoId { get; set; }
        public int? ProductoConcretoId { get; set; }

        public string Descripcion { get; set; }

        public int GrupoId { get; set; }

        public virtual GrupoDeProducto Grupo { get; set; }

        public int UnidadDeMedidaId { get; set; }

        public virtual UnidadDeMedida UnidadDeMedida { get; set; }

        public decimal Cantidad { get; set; }

        public decimal PrecioUnitario { get; set; }

        public bool EsInventariable { get; set; }

        public ProductoViewModel()
        {
            
        }

        public ProductoViewModel(ProductoConcreto producto)
        {
            Nombre = producto.Producto.Nombre;
            ProductoId = producto.ProductoId;
            ProductoConcretoId = producto.Id;
            Descripcion = producto.Producto.Descripcion;
            GrupoId = producto.Producto.GrupoId;
            UnidadDeMedidaId = producto.UnidadDeMedidaId;
            Cantidad = producto.Cantidad;
            PrecioUnitario = producto.PrecioDeVenta;
            EsInventariable = producto.Producto.EsInventariable;
        }
    }
}