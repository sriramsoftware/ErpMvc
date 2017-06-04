using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlmacenCore.Models;
using CompraVentaBL;
using ContabilidadCore.Models;
using ErpMvc.Models;
using ErpMvc.ViewModels;

namespace ErpMvc.Controllers
{
    [Authorize(Roles = RolesMontin.Administrador)]
    public class CentrosDeCostosController : Controller
    {
        private DbContext _db;
        private CentroDeCostoService _centroDeCostoService;

        public CentrosDeCostosController(DbContext context)
        {
            _db = context;
            _centroDeCostoService = new CentroDeCostoService(context);
        }

        public JsonResult ListaCentrosDeCosto()
        {
            return Json(_centroDeCostoService.CentrosDeCosto().Select(c => new {Id = c.Id, Nombre = c.Nombre}), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Existencias(int id)
        {
            return Json(_db.Set<ExistenciaCentroDeCosto>().Where(e => e.CentroDeCostoId == id).Select(e => new
            {
                Id = e.ProductoId,
                Nombre = e.Producto.Producto.Nombre,
                Cantidad = e.Cantidad,
                UnidadId = e.Producto.UnidadDeMedidaId,
                Unidad = e.Producto.UnidadDeMedida.Siglas
            }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SePuedeDarSalida(DetalleProductoViewModel producto)
        {
            return _centroDeCostoService.PuedeDarSalida(producto.ProductoId,producto.CentroCostoId,producto.Cantidad,producto.UnidadId) ? Json(true, JsonRequestBehavior.AllowGet) : Json(false, JsonRequestBehavior.AllowGet);
        }

        // GET: CentrosDeCostos
        public ActionResult Index()
        {
            return View(_centroDeCostoService.CentrosDeCosto());
        }

        public ActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Agregar(CentroDeCosto centroDeCosto)
        {
            if (_centroDeCostoService.AgregarCentroDeCosto(centroDeCosto))
            {
                TempData["exito"] = "Centro de costo agregado correctamente";
                return RedirectToAction("Index");
            }
            return View(centroDeCosto);
        }

        public ActionResult Editar(int id)
        {
            var centro = _centroDeCostoService.CentrosDeCosto().Find(id);
            return View(centro);
        }

        [HttpPost]
        public ActionResult Editar(CentroDeCosto centroDeCosto)
        {
            if (_centroDeCostoService.ModificarCentroDeCosto(centroDeCosto))
            {
                TempData["exito"] = "Centro de costo modificado correctamente";
                return RedirectToAction("Index");
            }
            return View(centroDeCosto);
        }
    }
}