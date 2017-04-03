using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlmacenCore.Models;
using CompraVentaBL;
using ContabilidadCore.Models;
using ErpMvc.ViewModels;
using Microsoft.AspNet.Identity;

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
            ViewBag.AlmacenId = _db.Set<Almacen>().FirstOrDefault().Id;
            //ViewBag.AlmacenId = new SelectList(_db.Set<Almacen>(), "Id", "Descripcion");
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

            return PartialView("_ListaProductosPartial", existencias);
        }

        public ActionResult CentroDeCosto()
        {
            ViewBag.CentroDeCostoId = new SelectList(_db.Set<CentroDeCosto>(), "Id", "Nombre");
            return View();
        }

        // GET: Inventario
        public PartialViewResult ProductosEnCentroDeCosto(int id)
        {
            var existencias = _db.Set<ExistenciaCentroDeCosto>().Include(e => e.Producto).Where(e => e.CentroDeCostoId == id).Select(p => new ProductoConcretoViewModel()
            {
                Producto = p.Producto,
                Existencias = new List<ExistenciaViewModel>()
                {
                    new ExistenciaViewModel() {Lugar = p.CentroDeCosto.Nombre, Cantidad = p.Cantidad}
                }
            }).ToList();

            return PartialView("_ListaProductosPartial", existencias);
        }

        public ActionResult MoverEntreCentrosDeCosto()
        {
            ViewBag.OrigenId = new SelectList(_db.Set<CentroDeCosto>(), "Id","Nombre");
            return View();
        }

        [HttpPost]
        public ActionResult MoverEntreCentrosDeCosto(MovimientoProductosViewModel movimiento)
        {
            if (ModelState.IsValid)
            {
                var centroCostoService = new CentroDeCostoService(_db);
                var usurioId = User.Identity.GetUserId();
                foreach (var producto in movimiento.Productos)
                {
                    if (!centroCostoService.TrasladarProductoDeCentroDeCosto(movimiento.OrigenId,
                        movimiento.DestinoId, producto.ProductoId, producto.Cantidad, producto.UnidadDeMedidaId, usurioId))
                    {
                        TempData["error"] = "No se pudo realizar el movimiento";
                        return RedirectToAction("CentroDeCosto");
                    }
                }
                if (centroCostoService.GuardarCambios())
                {
                    TempData["exito"] = "Movimiento realizado correctmente";
                }
                else
                {
                    TempData["error"] = "No se pudo realizar el movimiento";
                }
                return RedirectToAction("CentroDeCosto");
            }
            return View();
        }
    }
}