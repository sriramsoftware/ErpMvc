using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlmacenCore.Models;
using CompraVentaBL;
using CompraVentaCore.Models;
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
        private CentroDeCostoService _centroCostoService;

        public AlmacenController(DbContext context)
        {
            _db = context;
            _almacenService = new AlmacenService(context);
            _centroCostoService = new CentroDeCostoService(context);
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

        public ActionResult ValesDeSalida()
        {
            return View(_db.Set<ValeSalidaDeAlmacen>().ToList().GroupBy(v => v.DiaContable).Select(v => 
            new ValeSalidaDeAlmacen()
            {
                DiaContable = v.Key,
                Productos = v.SelectMany(p => p.Productos).ToList()
            })
            .OrderByDescending(v => v.DiaContable.Fecha).ToList());
        }

        public ActionResult DetalleDeVale(int id)
        {
            return View(_db.Set<DetalleSalidaAlmacen>().Where(d => d.Vale.DiaContableId == id).OrderByDescending(v => v.Producto.Producto.Producto.Nombre).ToList());
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
            return RedirectToAction("Mermas", "Almacen");
        }

        [HttpPost]
        public JsonResult SePuedeDarSalida(SalidaPorMerma producto)
        {
            return _almacenService.SePuedeDarSalidaPorMerma(producto) ? Json(true, JsonRequestBehavior.AllowGet) : Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TrasladoDeCentroAAlmacen()
        {
            ViewBag.AlmacenId = _db.Set<Almacen>().FirstOrDefault().Id;
            ViewBag.CentroDeCostoId = new SelectList(_centroCostoService.CentrosDeCosto(), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        [DiaContable]
        public ActionResult TrasladoDeCentroAAlmacen(Compra compra, int centroDeCostoId)
        {
            var usuario = User.Identity.GetUserId();
            compra.UsuarioId = usuario;
            var almacenId = _almacenService.GetAlmacenIdPorDefecto();
            if (ModelState.IsValid)
            {
                if (!compra.Productos.Any())
                {
                    TempData["error"] = "No se puede efectuar un traslado sin productos";
                    return View();
                }
                foreach (var prod in compra.Productos)
                {
                    _almacenService.EntradaDesdeCentroDeCosto(almacenId, centroDeCostoId, prod.ProductoId, prod.Cantidad, prod.UnidadDeMedidaId, User.Identity.GetUserId());
                }

                if (_almacenService.GuardarCambios())
                {
                    TempData["exito"] = "Salida registrada correctamente";
                    return RedirectToAction("CentroDeCosto", "Inventario");
                }
                TempData["error"] = "No se pudo registrar la salida correctamente";
                return RedirectToAction("CentroDeCosto", "Inventario");
            }
            ViewBag.CentroDeCostoId = new SelectList(_centroCostoService.CentrosDeCosto(), "Id", "Nombre", centroDeCostoId);
            return View(compra);
        }
    }
}