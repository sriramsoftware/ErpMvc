using System.Collections.Generic;
using CompraVentaCore.Models;

namespace ErpMvc.ViewModels
{
    public class VerificarVentaViewModel
    {
        public int PuntoDeVentaId { get; set; }

        public DetalleDeVenta NuevoDetalle { get; set; }

        public ICollection<DetalleDeVenta> Detalles { get; set; }

        public VerificarVentaViewModel()
        {
            Detalles = new List<DetalleDeVenta>();
        }
    }
}