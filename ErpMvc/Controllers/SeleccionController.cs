using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompraVentaCore.Models;
using ContabilidadBL;
using ContabilidadCore.Models;
using ErpMvc.Models;
using ErpMvc.Utiles;
using ErpMvc.ViewModels;

namespace ErpMvc.Controllers
{
    public class SeleccionController : Controller
    {
        private DbContext _db;
        private PeriodoContableService _periodoContableService;

        public SeleccionController(DbContext context)
        {
            _db = context;
            _periodoContableService = new PeriodoContableService(context);
        }

        // GET: Seleccion
        public ActionResult Ventas()
        {
            var ventas = _db.Set<Venta>().Where(v => !v.DiaContable.Abierto).GroupBy(c => c.DiaContable).Select(g => new SeleccionViewModel()
            {
                DiaContable = g.Key,
                Importe = g.Sum(co => co.Importe),
                ImporteSeleccionado = _db.Set<SeleccionVenta>().Any(s => s.Venta.DiaContableId == g.Key.Id)? _db.Set<SeleccionVenta>().Where(s => s.Venta.DiaContableId == g.Key.Id).Sum(s => s.Venta.Importe):0
            }); ;
            return View(ventas);
        }

        public ActionResult SeleccionarVentas(int id)
        {
            if (!_db.Set<SeleccionVenta>().Any(s => s.Venta.DiaContableId == id))
            {
                var info = new InformacionSolicitada(_db);
                info.ResumirVentas(id);
            }
            var ventas = _db.Set<SeleccionVenta>().Where(s => s.Venta.DiaContableId == id);
            return View(ventas.Select(v => v.Venta).ToList());
        }

        public ActionResult Compras()
        {
            var compras = _db.Set<Compra>().Where(c => !c.DiaContable.Abierto).GroupBy(c => c.DiaContable).Select(g => new SeleccionViewModel()
            {
                DiaContable = g.Key,
                Importe = g.Sum(co => co.Productos.Sum(p => p.ImporteTotal)),
                ImporteSeleccionado = _db.Set<SeleccionCompra>().Any(c => c.Compra.DiaContableId == g.Key.Id) ?_db.Set<SeleccionCompra>().Where(c => c.Compra.DiaContableId == g.Key.Id).Sum(c => c.Compra.Productos.Sum(com => com.ImporteTotal)):0
            });
            return View(compras);
        }
        
        public ActionResult SeleccionarCompras(int id)
        {
            if (_db.Set<SeleccionVenta>().Any(v => v.Venta.DiaContableId == id))
            {
                ViewBag.Ventas =
                    _db.Set<SeleccionVenta>().Where(v => v.Venta.DiaContableId == id).Sum(v => v.Venta.Importe);
                if (_db.Set<SeleccionCompra>().Any(c => c.Compra.DiaContableId == id))
                {
                    return View("ComprasSeleccionadas", _db.Set<SeleccionCompra>().Where(c => c.Compra.DiaContableId == id).Select(c => c.Compra));
                }
                var compras = _db.Set<Compra>().Where(c => c.DiaContableId == id);
                return View(compras);
            }
            TempData["error"] = "No existen ventas seleccionadas, por favor seleccion las venas primero";
            return RedirectToAction("Compras");
        }

        [HttpPost]
        public ActionResult SeleccionarCompras(List<int> comprasIds )
        {
            try
            {
                foreach (var id in comprasIds)
                {
                    _db.Set<SeleccionCompra>().Add(new SeleccionCompra() {CompraId = id});
                }
                _db.SaveChanges();
                return RedirectToAction("Compras");
            }
            catch (Exception)
            {
                return View();
            }
        }
    }
}