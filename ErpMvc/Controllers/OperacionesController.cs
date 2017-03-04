using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompraVentaCore.Models;
using ContabilidadBL;
using ErpMvc.Models;
using ErpMvc.ViewModels;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class OperacionesController : Controller
    {
        private DbContext _db;
        private PeriodoContableService _periodoContableService;

        public OperacionesController(DbContext context)
        {
            _db = context;
            _periodoContableService = new PeriodoContableService(context);
        }

        public PartialViewResult ResumenDeOperacion(int id)
        {
            var diaContable = _periodoContableService.BuscarDiaContable(id);

            var operaciones = new List<ResumenDeOperaciones>();

            operaciones.AddRange(_db.Set<OtrosGastos>().Where(g => g.DiaContableId == id).Select(o => new ResumenDeOperaciones() {Detalle = o.ConceptoDeGasto.Nombre, Importe = -o.Importe}));
            var ventas = _db.Set<Venta>().Where(g => g.DiaContableId == id).ToList();
            var compras = _db.Set<Compra>().Where(g => g.DiaContableId == id).ToList();
            foreach (var venta in ventas)
            {
                operaciones.Add(new ResumenDeOperaciones() { Detalle = "Venta: "+ venta.Resumen, Importe = venta.Importe });
            }
            foreach (var compra in compras)
            {
                operaciones.Add(new ResumenDeOperaciones() { Detalle = "Compra de: " + string.Join(",", compra.Productos.Select(p => p.Producto.Nombre)), Importe = -compra.Productos.Sum(p => p.ImporteTotal) });
            }
            return PartialView("_ResumenDeOperacionesPartial",operaciones);
        }

    }
}