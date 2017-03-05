using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContabilidadBL;
using ErpMvc.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SeguridadCore.Models;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class PeriodoContableController : Controller
    {
        private PeriodoContableService _service;
        private DbContext _db;

        public PeriodoContableController(DbContext context)
        {
            _service = new PeriodoContableService(context);
            _db = context;
        }
        // GET: PeriodoContable
        public PartialViewResult DiaContable()
        {
            var diaContable = _service.GetDiaContableActual();
            return PartialView("_DiaContablePartial", diaContable);
        }

        public JsonResult DiaContableData()
        {
            var diaContable = _service.GetDiaContableActual();
            return Json(diaContable.Fecha.ToShortDateString(),JsonRequestBehavior.AllowGet);
        }

        public ActionResult CerrarDia()
        {
            var diaContable = _service.GetDiaContableActual();
            
            if (!_db.Set<Asistencia>().Any(a => a.DiaContableId == diaContable.Id) || _db.Set<Asistencia>().Any(a => a.DiaContableId == diaContable.Id && a.Salida == null))
            {
                TempData["error"] = "Debe registrar la asistencia de los trabajadores!";
                return RedirectToAction("Index", "ControlDeAsistencia");
            }
            _service.CerrarDiaContable();
            HttpContext.GetOwinContext().Authentication.SignOut();
            //_service.EmpezarDiaContable(diaContable.Fecha.AddDays(1));
            //TempData["exito"] = "Se cerro el dia correctamente!";
            return RedirectToAction("Index", "Inicio");
        }

        public ActionResult AbrirDia()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AbrirDia(DateTime? fecha)
        {
            if (fecha != null)
            {
                _service.CerrarDiaContable();
                _service.EmpezarDiaContable(fecha.Value);
                TempData["exito"] = "Dia abierto correctamente, registre la laentrada de los trabajadores";
                return RedirectToAction("Index", "ControlDeAsistencia");
            }
            return View();
        }
    }
}