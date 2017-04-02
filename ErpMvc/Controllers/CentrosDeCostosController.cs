using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompraVentaBL;
using ContabilidadCore.Models;
using ErpMvc.Models;

namespace ErpMvc.Controllers
{
    [Authorize(Roles = RolesMontin.Administrador)]
    public class CentrosDeCostosController : Controller
    {
        private CentroDeCostoService _centroDeCostoService;

        public CentrosDeCostosController(DbContext context)
        {
            _centroDeCostoService = new CentroDeCostoService(context);
        }

        public JsonResult ListaCentrosDeCosto()
        {
            return Json(_centroDeCostoService.CentrosDeCosto().Select(c => new {Id = c.Id, Nombre = c.Nombre}), JsonRequestBehavior.AllowGet);
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
        
        //public ActionResult Eliminar(int id)
        //{
        //    return PartialView("_EliminarTrabajadorPartial");
        //}

        //[HttpPost]
        //public ActionResult EliminarConfirmado(int id)
        //{
        //    return RedirectToAction("Index");
        //}

    }
}