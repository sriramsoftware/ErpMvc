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
                Ventas = _db.Set<SeleccionVenta>().Any(v => v.Venta.DiaContableId == g.Key.Id)? _db.Set<SeleccionVenta>().Where(v => v.Venta.DiaContableId == g.Key.Id).Sum(v => v.Venta.Importe):0,
                Importe = g.Sum(co => co.Productos.Sum(p => p.ImporteTotal)),
                ImporteSeleccionado = _db.Set<SeleccionCompra>().Any(c => c.Compra.DiaContableId == g.Key.Id) ?_db.Set<SeleccionCompra>().Where(c => c.Compra.DiaContableId == g.Key.Id).Sum(c => c.Compra.Productos.Sum(com => com.ImporteTotal)):0,
                ConComprobante = _db.Set<SeleccionCompra>().Any(c => c.Compra.DiaContableId == g.Key.Id && c.Compra.TieneComprobante) ?_db.Set<SeleccionCompra>().Where(c => c.Compra.DiaContableId == g.Key.Id && c.Compra.TieneComprobante).Sum(c => c.Compra.Productos.Sum(com => com.ImporteTotal)):0,
                SinComprobante = _db.Set<SeleccionCompra>().Any(c => c.Compra.DiaContableId == g.Key.Id && !c.Compra.TieneComprobante) ?_db.Set<SeleccionCompra>().Where(c => c.Compra.DiaContableId == g.Key.Id && !c.Compra.TieneComprobante).Sum(c => c.Compra.Productos.Sum(com => com.ImporteTotal)):0,
            });
            return View(compras);
        }
        
        public ActionResult SeleccionarCompras(int id)
        {
            if (_db.Set<SeleccionVenta>().Any(v => v.Venta.DiaContableId == id))
            {
                ViewBag.Ventas =
                    _db.Set<SeleccionVenta>().Where(v => v.Venta.DiaContableId == id).Sum(v => v.Venta.Importe);
                /*if (_db.Set<SeleccionCompra>().Any(c => c.Compra.DiaContableId == id))
                {
                    return View("ComprasSeleccionadas", _db.Set<SeleccionCompra>().Where(c => c.Compra.DiaContableId == id).Select(c => c.Compra));
                }*/
                ViewBag.Seleccionadas = _db.Set<SeleccionVenta>().Where(v => v.Venta.DiaContableId == id).ToList();
                ViewBag.DiaContableId = id;
                var compras = _db.Set<Compra>().Where(c => c.DiaContableId == id).Select( c => new SeleccionarCompraViewModel() {Seleccionado = _db.Set<SeleccionCompra>().Any(s => s.CompraId == c.Id),Compra = c});
                return View(compras);
            }
            TempData["error"] = "No existen ventas seleccionadas, por favor seleccion las venas primero";
            return RedirectToAction("Compras");
        }

        [HttpPost]
        public ActionResult SeleccionarCompras(List<int> comprasIds, int diaContableId )
        {
            try
            {
                var comprasSelActuales = _db.Set<SeleccionCompra>().Where(c => c.Compra.DiaContableId == diaContableId).Select(c => c.CompraId).ToList();
                if (comprasIds == null)
                {
                    comprasIds = new List<int>();
                }
                var comprasAQuitar = comprasSelActuales.Where(c => comprasIds.All(i => i != c)).ToList();
                
                foreach (var id in comprasIds)
                {
                    if (!_db.Set<SeleccionCompra>().Any(c => c.CompraId == id))
                    {
                        _db.Set<SeleccionCompra>().Add(new SeleccionCompra() { CompraId = id });
                    }
                }
                foreach (var id in comprasAQuitar)
                {
                    var sel = _db.Set<SeleccionCompra>().SingleOrDefault(s => s.CompraId == id);
                    _db.Set<SeleccionCompra>().Remove(sel);
                }
                _db.SaveChanges();
                return RedirectToAction("Compras");
            }
            catch (Exception)
            {
                return View();
            }
        }

        public JsonResult VentasData(int id)
        {
            return Json(_db.Set<SeleccionVenta>().Where(s => s.Venta.DiaContableId == id).Sum(s => s.Venta.Importe), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ComprasSeleccionadas(int id)
        {
            var compras = _db.Set<SeleccionCompra>().Where(s => s.Compra.DiaContableId == id);
            var ventas = _db.Set<SeleccionVenta>().Where(s => s.Venta.DiaContableId == id).Sum(s => s.Venta.Importe);
            if (compras.Any()){
                
                var data = new
                {
                    comprasIds = compras.Select(s => s.CompraId).ToList(),
                    importe = compras.Sum(c => c.Compra.Productos.Sum(p => p.ImporteTotal)),
                    importeComprobante = compras.Any(c => c.Compra.TieneComprobante)? compras.Where(c => c.Compra.TieneComprobante).Sum(c => c.Compra.Productos.Sum(p => p.ImporteTotal)):0,
                    importeSinComprobante = compras.Any(c => !c.Compra.TieneComprobante)?compras.Where(c => !c.Compra.TieneComprobante).Sum(c => c.Compra.Productos.Sum(p => p.ImporteTotal)):0,
                    ventas = ventas
                };
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            var data1 = new
            {
                comprasIds = new List<int>(),
                importe = 0,
                importeComprobante =  0,
                importeSinComprobante =  0,
                ventas = ventas
            };
            return Json(data1, JsonRequestBehavior.AllowGet);
        }
    }
}