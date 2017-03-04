using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompraVentaBL;
using CompraVentaCore.Models;
using ContabilidadCore.Models;
using ErpMvc.Models;

namespace ErpMvc.Controllers
{
    [Authorize(Roles = RolesMontin.Administrador)]
    public class PuntoDeVentaController : Controller
    {
        private PuntoDeVentaService _puntoDeVentaService;

        public PuntoDeVentaController(DbContext context)
        {
            _puntoDeVentaService = new PuntoDeVentaService(context);
        }


        // GET: Puntos de ventas
        public ActionResult Index()
        {
            return View(_puntoDeVentaService.PuntosDeVentas().ToList());
        }

        public ActionResult Agregar()
        {
            ViewBag.CentroDeCostoId = new SelectList(_puntoDeVentaService.CentrosDeCostos(),"Id","Nombre");
            return View();
        }

        [HttpPost]
        public ActionResult Agregar(PuntoDeVenta puntoDeVenta)
        {
            if (_puntoDeVentaService.AgregarPuntoDeVenta(puntoDeVenta))
            {
                TempData["exito"] = "Punto de venta agregado correctamente";
                return RedirectToAction("Index");
            }
            ViewBag.CentroDeCostoId = new SelectList(_puntoDeVentaService.CentrosDeCostos(), "Id", "Nombre", puntoDeVenta.CentroDeCostoId);
            return View(puntoDeVenta);
        }

        public ActionResult Editar(int id)
        {
            var punto = _puntoDeVentaService.PuntosDeVentas().Find(id);
            ViewBag.CentroDeCostoId = new SelectList(_puntoDeVentaService.CentrosDeCostos(), "Id", "Nombre", punto.CentroDeCostoId);
            return View(punto);
        }

        [HttpPost]
        public ActionResult Editar(PuntoDeVenta puntoDeVenta)
        {
            if (_puntoDeVentaService.ModificarPuntoDeVenta(puntoDeVenta))
            {
                TempData["exito"] = "Punto de venta modificado correctamente";
                return RedirectToAction("Index");
            }
            ViewBag.CentroDeCostoId = new SelectList(_puntoDeVentaService.CentrosDeCostos(), "Id", "Nombre", puntoDeVenta.CentroDeCostoId);
            return View(puntoDeVenta);
        }
        
        public ActionResult Eliminar(int id)
        {
            return PartialView("_EliminarTrabajadorPartial");
        }

        [HttpPost]
        [ActionName("Eliminar")]
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

    }
}