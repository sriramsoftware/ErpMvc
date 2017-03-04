using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContabilidadBL;
using ErpMvc.Models;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class PeriodoContableController : Controller
    {
        private PeriodoContableService _service;

        public PeriodoContableController(PeriodoContableService service)
        {
            _service = service;
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
            return Json(DiaContable(),JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult CerrarDia()
        {
            _service.CerrarDiaContable();
            var diaContable = _service.GetDiaContableActual();
            _service.EmpezarDiaContable(diaContable.Fecha.AddDays(1));
            TempData["exito"] = "Se cerro el dia correctamente!";
            return RedirectToAction("Index", "Inicio");
        }
    }
}