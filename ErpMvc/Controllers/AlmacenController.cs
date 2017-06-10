using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlmacenCore.Models;
using CompraVentaBL;
using ErpMvc.Models;
using ErpMvc.Utiles;
using Microsoft.AspNet.Identity;

namespace ErpMvc.Controllers
{
    [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
    [DiaContable]
    public class AlmacenController : Controller
    {
        private DbContext _db;
        private AlmacenService _almacenService;

        public AlmacenController(DbContext context)
        {
            _db = context;
            _almacenService = new AlmacenService(context);
        }

        public JsonResult ExistenciasAlmacen(int id)
        {
            return Json(_db.Set<ExistenciaAlmacen>().Where(a => a.AlmacenId == id).Select(a => new
            {
                Id = a.Id,
                Nombre = a.Producto.Producto.Nombre,
                Unidad = a.Producto.UnidadDeMedida.Siglas,
                ProductoId = a.Producto.ProductoId,
                Cantidad = a.ExistenciaEnAlmacen
            }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Mermas()
        {
            return View(_db.Set<SalidaPorMerma>().ToList());
        }

        // GET: Almacen
        public ActionResult DarSalida()
        {
            ViewBag.AlmacenId = _db.Set<Almacen>().FirstOrDefault().Id;
            return View();
        }

        [HttpPost]
        public ActionResult DarSalida(ValeSalidaDeAlmacen vale)
        {
            if (ModelState.IsValid)
            {
                vale.UsuarioId = User.Identity.GetUserId();
                if (!_almacenService.DarSalidaDeAlmacen(vale))
                {
                    TempData["error"] = "Error al dar salida";
                }
                else
                {
                    TempData["exito"] = "Salida efectuada correctamente";
                }
                return RedirectToAction("Almacen", "Inventario");
            }
            return View();
        }

        public ActionResult DarSalidaPorMerma()
        {
            ViewBag.AlmacenId = _db.Set<Almacen>().FirstOrDefault().Id;
            
            return View();
        }

        [HttpPost]
        public ActionResult DarSalidaPorMerma(List<SalidaPorMerma> mermas)
        {
            //todo: usar el ciclo del metodo de dar salida para recorrer una sola vez
            var userId = User.Identity.GetUserId();
            foreach (var merma in mermas)
            {
                merma.UsuarioId = userId;
            }
            if (!_almacenService.DarSalidaPorMerma(mermas))
            {
                TempData["error"] = "Error al dar salida por merma";
            }
            else
            {
                TempData["exito"] = "Salida efectuada correctamente";
            }
            return RedirectToAction("Almacen", "Inventario");
        }

        [HttpPost]
        public JsonResult SePuedeDarSalida(SalidaPorMerma producto)
        {
            return _almacenService.SePuedeDarSalidaPorMerma(producto) ? Json(true, JsonRequestBehavior.AllowGet) : Json(false, JsonRequestBehavior.AllowGet);
        }
    }
}