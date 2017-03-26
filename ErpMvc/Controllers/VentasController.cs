using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CompraVentaBL;
using CompraVentaCore.Models;
using ContabilidadBL;
using ContabilidadCore.Models;
using ErpMvc.Models;
using HumanResourcesCore.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private VentasService _ventasService;
        private PeriodoContableService _periodoContableService;

        public VentasController(DbContext context)
        {
            _ventasService = new VentasService(context);
            _periodoContableService = new PeriodoContableService(context);
        }


        // GET: Ventas
        public ActionResult Index(int id = 0)
        {
            if (id == -1)
            {
                TempData["error"] = "El dia seleccionado no existe!";
                id = 0;
            }
            var diaContable = id == 0
                ? _periodoContableService.GetDiaContableActual()
                : _periodoContableService.BuscarDiaContable(id);
            ViewBag.DiaContable = diaContable;
            return View();
        }

        public PartialViewResult ListaDeVentasPartial(int id)
        {
            return PartialView("_ListaDeVentasPartial", _ventasService.Ventas().Where(v => v.DiaContableId == id).ToList());
        }

        public ActionResult NuevaVenta()
        {
            ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre");
            ViewBag.VendedorId = new SelectList(_ventasService.Vendedores().Where(v => v.Estado == EstadoTrabajador.Activo), "Id", "NombreCompleto");
            return View();
        }

        [HttpPost]
        public ActionResult BuscarVentas(DateTime fecha)
        {
            var diaContable = _periodoContableService.BuscarDiaContable(fecha);
            if (diaContable == null)
            {
                return RedirectToAction("ListaDeVentasPartial", new { Id = -1 });
            }
            return RedirectToAction("ListaDeVentasPartial", new { Id = diaContable.Id });
        }

        [HttpPost]
        public ActionResult NuevaVenta(Venta venta)
        {
            if (User.IsInRole(RolesMontin.Vendedor) || User.IsInRole(RolesMontin.UsuarioAvanzado))
            {
                var usuarioId = User.Identity.GetUserId();
                var vendedor = _ventasService.Vendedores().SingleOrDefault(v => v.UsuarioId == usuarioId);
                if (vendedor == null)
                {
                    TempData["error"] = "No existe vendedor asociado a la cuenta, consulte al administrador";
                    return RedirectToAction("Index");
                }
                venta.VendedorId = vendedor.Id;
            }
            if (!venta.Elaboraciones.Any())
            {
                TempData["error"] = "No se puede efectuar una venta vacia";
                ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre");
                ViewBag.VendedorId = new SelectList(_ventasService.Vendedores(), "Id", "NombreCompleto");
                return View();
            }
            if (_ventasService.Vender(venta, User.Identity.GetUserId()))
            {
                TempData["exito"] = "Venta agregada correctamente";
                return RedirectToAction("Index");
            }
            ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre");
            ViewBag.VendedorId = new SelectList(_ventasService.Vendedores(), "Id", "NombreCompleto");
            return View();
        }

        public PartialViewResult DetalleDeVentaPartial()
        {
            return PartialView("_DetalleDeVentaPartial");
        }

        public PartialViewResult MenusEnVentaPartial(int id)
        {
            return PartialView("_MenusVendidosPartial", _ventasService.Ventas().Find(id).Elaboraciones);
        }



        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(int id)
        {
            //var centro = _centroDeCostoService.CentrosDeCosto().Find(id);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(CentroDeCosto centroDeCosto)
        {
            //if (_centroDeCostoService.ModificarCentroDeCosto(centroDeCosto))
            //{
            //    TempData["exito"] = "Centro de costo modificado correctamente";
            //    return RedirectToAction("Index");
            //}
            return View(centroDeCosto);
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult Eliminar(int id)
        {
            return PartialView("_EliminarTrabajadorPartial");
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        [HttpPost]
        public ActionResult EliminarConfirmado(int id)
        {
            //var trabajador = _vendedorService.Vendedores().Find(id);
            //if (trabajador == null)
            //{
            //    return new HttpNotFoundResult();
            //}
            //trabajador.Estado = EstadoTrabajador.Baja;
            //if (trabajador.Usuario != null)
            //{
            //    trabajador.Usuario.Activo = false;
            //}
            //_vendedorService.ModificarVendedor(trabajador);
            //TempData["exito"] = "Trabajador eliminado correctamente";
            return RedirectToAction("Index");
        }

        public JsonResult SePuedeVender(int menuId, int cantidad)
        {
            var result = _ventasService.SePuedeVender(menuId, cantidad);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ImporteVenta(int id)
        {
            var venta = _ventasService.Ventas().Find(id);
            return Json(venta.Importe, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public ActionResult PagarVale(int? id)
        {
            //var lqv = Request.Form["id"];
            //int? id = int.Parse(lqv);
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var venta = _ventasService.Ventas().Find(id);
            if (venta == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            var result =_ventasService.PagarVentaEnEfectivo(venta.Id, User.Identity.GetUserId());
            if (result)
            {
                TempData["exito"] = "Se pago el vale correctamente";
            }
            else
            {
                TempData["errro"] = "Error al pagar";
            }
            return RedirectToAction("Index");
        }

    }
}