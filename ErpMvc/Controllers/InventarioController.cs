using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlmacenCore.Models;
using ContabilidadCore.Models;
using ErpMvc.ViewModels;

namespace ErpMvc.Controllers
{
    public class InventarioController : Controller
    {
        private DbContext _db;

        public InventarioController(DbContext context)
        {
            _db = context;
        }

        public ActionResult Almacen()
        {
            ViewBag.AlmacenId = new SelectList(_db.Set<Almacen>(), "Id", "Descripcion");
            return View();
        }

        // GET: Inventario
        public PartialViewResult ProductosEnAlmacen(int id)
        {
            var existencias = _db.Set<ExistenciaAlmacen>().Include(e => e.Producto).Where(e => e.AlmacenId == id).Select(p => new ProductoConcretoViewModel()
            {
                Producto = p.Producto,
                Existencias = new List<ExistenciaViewModel>()
                {
                    new ExistenciaViewModel() {Lugar = p.Almacen.Descripcion, Cantidad = p.ExistenciaEnAlmacen}
                }
            }).ToList();
            
            return PartialView("_ListaProductosPartial",existencias);
        }

        public ActionResult CentroDeCosto()
        {
            ViewBag.CentroDeCostoId = new SelectList(_db.Set<CentroDeCosto>(), "Id", "Nombre");
            return View();
        }

        // GET: Inventario
        public PartialViewResult ProductosEnCentroDeCosto(int id)
        {
            var existencias = _db.Set<ExistenciaCentroDeCosto>().Include(e => e.Producto).Where(e => e.ProductoId == id).Select(p => new ProductoConcretoViewModel()
            {
                Producto = p.Producto,
                Existencias = new List<ExistenciaViewModel>()
                {
                    new ExistenciaViewModel() {Lugar = p.CentroDeCosto.Nombre, Cantidad = p.Cantidad}
                }
            }).ToList();
            
            return PartialView("_ListaProductosPartial",existencias);
        }
    }
}