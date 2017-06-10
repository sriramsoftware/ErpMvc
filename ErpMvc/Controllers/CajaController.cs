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
    [DiaContable]
    public class CajaController : Controller
    {
        private DbContext _db;
        private PeriodoContableService _periodoContableService;
        private CuentasServices _cuentasServices;
        private SubmayorService _submayorService;

        public CajaController(DbContext context)
        {
            _db = context;
            _periodoContableService = new PeriodoContableService(context);
            _cuentasServices = new CuentasServices(context);
            _submayorService = new SubmayorService(context);
        }
        // GET: Caja
        public ActionResult Index()
        {
            if (_periodoContableService.NoHayDiaAbierto())
            {
                _periodoContableService.EmpezarDiaContable(DateTime.Now);
            }
            ViewBag.DiaContable = _periodoContableService.GetDiaContableActual();
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

        
        public ActionResult Extraccion()
        {
            return View();
        }

        
        [HttpPost]
        public ActionResult Extraccion(OperacionCajaViewModel extraccion)
        {
            if (ModelState.IsValid)
            {
                var cta = _cuentasServices.FindCuentaByNombre("Caja");
                var cuentaGasto = _cuentasServices.FindCuentaByNombre("Gastos");
               
                string detalle = "Extracción de efectivo: " + extraccion.Observaciones;
                if (_submayorService.AgregarOperacion(cta.Id, cuentaGasto.Id, extraccion.Importe, DateTime.Now,
                    detalle, User.Identity.GetUserId()))
                {
                    TempData["exito"] = "Extracción efectuada correctamente";
                }
                else
                {
                    TempData["error"] = "Error al extraer efectivo";
                }
                return RedirectToAction("Index");
            }
            return View(extraccion);
        }

        
        public ActionResult Deposito()
        {
            return View();
        }

        
        [HttpPost]
        public ActionResult Deposito(OperacionCajaViewModel deposito)
        {
            if (ModelState.IsValid)
            {
                var cta = _cuentasServices.FindCuentaByNombre("Caja");
                var cuentaBanco = _cuentasServices.FindCuentaByNombre("Banco");

                string detalle = "Deposito de efectivo: " + deposito.Observaciones;
                if (_submayorService.AgregarOperacion( cuentaBanco.Id, cta.Id, deposito.Importe, DateTime.Now,
                    detalle, User.Identity.GetUserId()))
                {
                    TempData["exito"] = "Deposito efectuado correctamente";
                }
                else
                {
                    TempData["error"] = "Error al depositar efectivo";
                }
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
