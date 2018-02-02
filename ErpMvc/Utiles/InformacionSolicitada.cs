using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CompraVentaCore.Models;
using ErpMvc.Models;

namespace ErpMvc.Utiles
{
    public class InformacionSolicitada
    {
        private DbContext _db;
        private decimal _porcientoVentas = 30;

        public InformacionSolicitada(DbContext context)
        {
            _db = context;
        }

        public void ResumirVentas(int diaContableId)
        {
            if (_db.Set<SeleccionVenta>().Any(v => v.Venta.DiaContableId == diaContableId))
            {
                return;
            }
            var ventas = _db.Set<Venta>().Where(v => v.DiaContableId == diaContableId);
            if (ventas.Any())
            {
                var totalVentas = ventas.Sum(v => v.Importe);
                var importeAMostar = (totalVentas * _porcientoVentas) / 100;

                decimal incremental = 0;
                var listaAMostar = new List<int>();
                foreach (var venta in ventas)
                {
                    if (incremental <= importeAMostar)
                    {
                        listaAMostar.Add(venta.Id);
                        incremental += venta.Importe;
                    }
                    else
                    {
                        break;
                    }
                }
                foreach (var idVenta in listaAMostar)
                {
                    _db.Set<SeleccionVenta>().Add(new SeleccionVenta() {VentaId = idVenta});
                }
                _db.SaveChanges();
            }
            
        }
    }
}