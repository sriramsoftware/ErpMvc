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
            var ventas = _db.Set<SeleccionVenta>().Include(v => v.Venta).ToList();
            return View(ventas);
        }

        public ActionResult SeleccionarVentas(int id)
        {
            return View();
        }

        public ActionResult Compras()
        {
            var compras = _db.Set<Compra>().GroupBy(c => c.DiaContable).Select(g => new ComprasSeleccionViewModel()
            {
                DiaContable = g.Key,
                Importe = g.Sum(co => co.Productos.Sum(p => p.ImporteTotal))
            });
            return View(compras);
        }
        
        public ActionResult SeleccionarCompras(int id)
        {
            var compras = _db.Set<Compra>().Where(c => c.DiaContableId == id);
            return View(compras);
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