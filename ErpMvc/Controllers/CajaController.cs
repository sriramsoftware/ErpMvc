using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContabilidadBL;
using ContabilidadCore.Models;
using ErpMvc.Models;
using ErpMvc.Utiles;
using ErpMvc.ViewModels;
using Microsoft.AspNet.Identity;

namespace ErpMvc.Controllers
{
    [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
    public class CajaController : Controller
    {
        private DbContext _db;
        private PeriodoContableService _periodoContableService;
        private CuentasServices _cuentasServices;

        public CajaController(DbContext context)
        {
            _db = context;
            _periodoContableService = new PeriodoContableService(context);
            _cuentasServices = new CuentasServices(context);
        }
        // GET: Caja
        public ActionResult Index()
        {
            var diaContable = _periodoContableService.GetDiaContableActual();
            if (diaContable == null)
            {
                diaContable = _db.Set<DiaContable>().ToList().Last();
            }
            ViewBag.DiaContable = diaContable;
            return View();
        }

        private List<ResumenDeOperaciones> OperacionDeCaja()
        {
            var operaciones = new List<ResumenDeOperaciones>();
            var diaContableId = _periodoContableService.GetDiaContableActual().Id;
            var movimientos =
                _cuentasServices.GetMovimientosDeCuenta("Caja").Where(m => m.Asiento.DiaContableId == diaContableId);
            operaciones.AddRange(movimientos.Select(m => new ResumenDeOperaciones()
            {
                Fecha = m.Asiento.Fecha,
                Importe = m.TipoDeOperacion == TipoDeOperacion.Debito ? m.Importe : -m.Importe,
                Tipo = m.Asiento.Detalle.Substring(0, m.Asiento.Detalle.IndexOf(" ")),
                Detalle = m.Asiento.Detalle,
                Usuario = m.Asiento.Usuario.UserName
            }));
            return operaciones;
        }

        [DiaContable]
        public ActionResult Extraccion()
        {
            return View();
        }


        [HttpPost]
        [DiaContable]
        public ActionResult Extraccion(ExtraccionDeCajaViewModel extraccion)
        {
            if (ModelState.IsValid)
            {
                var cta = _cuentasServices.FindCuentaByNombre("Caja");
                var cuentaGasto = _cuentasServices.FindCuentaByNombre("Gastos");

                string detalle = "Extracción de efectivo: " + extraccion.Observaciones;
                _cuentasServices.AgregarOperacion(cta.Id, cuentaGasto.Id, extraccion.Importe, DateTime.Now,
                    detalle, User.Identity.GetUserId());
                _db.SaveChanges();
                TempData["exito"] = "Extracción efectuada correctamente";
                return RedirectToAction("Index");
            }
            return View(extraccion);
        }

        [DiaContable]
        public ActionResult Deposito()
        {
            return View();
        }


        [HttpPost]
        [DiaContable]
        public ActionResult Deposito(OperacionCajaViewModel deposito)
        {
            if (ModelState.IsValid)
            {
                var cta = _cuentasServices.FindCuentaByNombre("Caja");
                var cuentaBanco = _cuentasServices.FindCuentaByNombre("Banco");

                string detalle = "Deposito de efectivo: " + deposito.Observaciones;
                _cuentasServices.AgregarOperacion(cuentaBanco.Id, cta.Id, deposito.Importe, DateTime.Now,
                    detalle, User.Identity.GetUserId());
                _db.SaveChanges();
                TempData["exito"] = "Deposito efectuado correctamente";
                return RedirectToAction("Index");
            }
            return View(deposito);
        }

        public JsonResult SePuedeExtraer(decimal importe)
        {
            var cta = _cuentasServices.FindCuentaByNombre("Caja");
            var result = cta.Disponibilidad.Saldo >= importe;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
