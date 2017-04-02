using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlmacenCore.Models;
using CompraVentaBL;

namespace ErpMvc.Controllers
{
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
                if (!_almacenService.DarSalidaDeAlmacen(vale))
                {
                    TempData["error"] = "Error al dar salida";
                }
                else
                {
                    TempData["exito"] = "Salida efectuada correctamente";
                }
                return RedirectToAction("Almacen","Inventario");
            }
            return View();
        }
    }
}