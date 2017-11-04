using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CajaCore.Models;
using CompraVentaCore.Models;
using ContabilidadBL;
using ContabilidadCore.Models;
using ErpMvc.Models;
using ErpMvc.Reportes;
using ErpMvc.Utiles;
using ErpMvc.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SeguridadCore.Models;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class PeriodoContableController : Controller
    {
        private PeriodoContableService _service;
        private CuentasServices _cuentasServices;
        private CierreService _cierreService;
        private DbContext _db;

        public PeriodoContableController(DbContext context)
        {
            _service = new PeriodoContableService(context);
            _cuentasServices = new CuentasServices(context);
            _cierreService = new CierreService(context);
            _db = context;
        }

        public ActionResult Cierres()
        {
            var cierres = _db.Set<CierreDeCaja>().Include(c => c.Desglose).ToList();
            return View(cierres);
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
            return Json(diaContable.Fecha.ToShortDateString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ResumenEfectivo()
        {
            return ResumenEfectivoData(_service.GetDiaContableActual().Id);
        }

        public PartialViewResult DesgloseCierrePartial(int id)
        {
            var dia = _service.BuscarDiaContable(id);
            var cierre = _db.Set<CierreDeCaja>().SingleOrDefault(c => c.DiaContableId == dia.Id);
            return PartialView("_DesgloseDeEfectivoCierrePartial", cierre.Desglose);
        }

        public JsonResult ResumenEfectivoData(int diaId)
        {
            var resumenCierre = new ResumenCierre(_db);
            var resumen = resumenCierre.VerResumen(diaId);
            return Json(resumen, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DenominacionesData()
        {
            var billetes = _db.Set<DenominacionDeMoneda>().Where(d => d.Billete).GroupBy(d => d.Valor).OrderByDescending(d => d.Key).Select(d => new DenominacionViewModel()
            {
                Valor = d.Key,
                CantidadCup = 0,
                CantidadCuc = 0,
                Cup = d.Any(e => e.Moneda.Sigla == "CUP"),
                Cuc = d.Any(e => e.Moneda.Sigla == "CUC")
            });

            var monedas = _db.Set<DenominacionDeMoneda>().Where(d => !d.Billete).GroupBy(d => d.Valor).OrderByDescending(d => d.Key).Select(d => new DenominacionViewModel()
            {
                Valor = d.Key,
                CantidadCup = 0,
                CantidadCuc = 0,
                Cup = d.Any(e => e.Moneda.Sigla == "CUP"),
                Cuc = d.Any(e => e.Moneda.Sigla == "CUC")
            });

            var denominaciones = new DesgloceEfectivoViewModel()
            {
                Billetes = billetes.ToList(),
                Monedas = monedas.ToList(),
            };
            return Json(denominaciones, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public JsonResult CerrarPeriodo(DesgloceEfectivoViewModel desgloceEfectivoViewModel, decimal importeAExtraer, decimal pagoTrabajadores)
        {
            if (importeAExtraer >= 0)
            {
                var dia = _service.GetDiaContableActual();
                if (_db.Set<Venta>().Any(v => v.DiaContableId == dia.Id && (v.EstadoDeVenta == EstadoDeVenta.Facturada || v.EstadoDeVenta == EstadoDeVenta.Pendiente)))
                {
                    return Json(new { result = false, mensaje = "No se puede cerrar, ventas pendientes de pago" }, JsonRequestBehavior.AllowGet);
                }
                var cuentaCaja = _cuentasServices.FindCuentaByNombre("Caja");
                var cuentaBanco = _cuentasServices.FindCuentaByNombre("Banco");
                var cuentaGasto = _cuentasServices.FindCuentaByNombre("Gastos");
                if (cuentaCaja.Disponibilidad.Saldo < importeAExtraer + pagoTrabajadores)
                {
                    return Json(new { result = false, mensaje = "No se puede realizar la extraccion de la caja, saldo en caja inferior al extraer" }, JsonRequestBehavior.AllowGet);
                }
                _cuentasServices.AgregarOperacion(cuentaCaja.Id, cuentaBanco.Id, importeAExtraer, DateTime.Now, "Cierre del dia",
                    User.Identity.GetUserId());
                _cuentasServices.AgregarOperacion(cuentaCaja.Id, cuentaGasto.Id, pagoTrabajadores, DateTime.Now, "Trabajadores : Pago al cierre",
                   User.Identity.GetUserId());
                var caja = _db.Set<Caja>().FirstOrDefault();
                var cierre = new CierreDeCaja()
                {
                    CajaId = caja.Id,
                    DiaContableId = dia.Id
                };
                var cuc = _db.Set<Moneda>().SingleOrDefault(m => m.Sigla == "CUC");
                var cup = _db.Set<Moneda>().SingleOrDefault(m => m.Sigla == "CUP");
                foreach (var billete in desgloceEfectivoViewModel.Billetes)
                {
                    if (billete.Cuc && billete.CantidadCuc > 0)
                    {
                        var denominacion =
                            _db.Set<DenominacionDeMoneda>().SingleOrDefault(d => d.Billete && d.MonedaId == cuc.Id && d.Valor == billete.Valor);
                        cierre.Desglose.Add(new DenominacionesEnCierreDeCaja()
                        {
                            DenominacionDeMonedaId = denominacion.Id,
                            Cantidad = billete.CantidadCuc
                        });
                    }
                    if (billete.Cup && billete.CantidadCup > 0)
                    {
                        var denominacion =
                            _db.Set<DenominacionDeMoneda>().SingleOrDefault(d => d.Billete && d.MonedaId == cup.Id && d.Valor == billete.Valor);
                        cierre.Desglose.Add(new DenominacionesEnCierreDeCaja()
                        {
                            DenominacionDeMonedaId = denominacion.Id,
                            Cantidad = billete.CantidadCup
                        });
                    }

                }
                foreach (var moneda in desgloceEfectivoViewModel.Monedas)
                {
                    if (moneda.Cuc && moneda.CantidadCuc > 0)
                    {
                        var denominacion =
                            _db.Set<DenominacionDeMoneda>().SingleOrDefault(d => !d.Billete && d.MonedaId == cuc.Id && d.Valor == moneda.Valor);
                        cierre.Desglose.Add(new DenominacionesEnCierreDeCaja()
                        {
                            DenominacionDeMonedaId = denominacion.Id,
                            Cantidad = moneda.CantidadCuc
                        });
                    }
                    if (moneda.Cup && moneda.CantidadCup > 0)
                    {
                        var denominacion =
                            _db.Set<DenominacionDeMoneda>().SingleOrDefault(d => !d.Billete && d.MonedaId == cup.Id && d.Valor == moneda.Valor);
                        cierre.Desglose.Add(new DenominacionesEnCierreDeCaja()
                        {
                            DenominacionDeMonedaId = denominacion.Id,
                            Cantidad = moneda.CantidadCup
                        });
                    }
                }
                _cierreService.CerrarCaja(cierre);
                _service.CerrarDiaContable();
                _db.SaveChanges();
                //HttpContext.GetOwinContext().Authentication.SignOut();
                return Json(new { result = true, cierreId = cierre.DiaContableId, mensaje = "Cierre correcto" }, JsonRequestBehavior.AllowGet);

            }
            return Json(new { result = false, mensaje = "No se puede cerrar, importe a extraer negativo" }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public JsonResult SePuedeCerrar(decimal importe)
        {
            var dia = _service.GetDiaContableActual();
            if (importe < 0)
            {
                return Json(new { result = false, mensaje = "Importe a extraer negativo" }, JsonRequestBehavior.AllowGet);
            }
            if (_db.Set<Venta>().Any(v => v.DiaContableId == dia.Id && (v.EstadoDeVenta == EstadoDeVenta.Facturada || v.EstadoDeVenta == EstadoDeVenta.Pendiente)))
            {
                return Json(new { result = false, mensaje = "No se puede cerrar, ventas pendientes de pago" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { result = true, mensaje = "Se puede cerrar" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VentasPendientes()
        {
            var dia = _service.GetDiaContableActual();
            if (_db.Set<Venta>().Any(v => v.DiaContableId == dia.Id && (v.EstadoDeVenta == EstadoDeVenta.Facturada || v.EstadoDeVenta == EstadoDeVenta.Pendiente)))
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult CerrarPeriodo()
        {
            ViewBag.DiaContable = _service.GetDiaContableActual();
            return View();
        }

        public ActionResult CierreReporte(int id)
        {
            var dia = _service.BuscarDiaContable(id);
            ViewBag.DiaContable = dia;
            return View("ReporteCierre");
        }


        public PartialViewResult DesgloseEfectivo()
        {
            return PartialView("_DesgloseEfectivoPartial");
        }

        public PartialViewResult ResumenDeEfectivo()
        {
            return PartialView("_ResumenDeEfectivoPartial");
        }

        public ActionResult AbrirDia()
        {
            var fecha = DateTime.Now;
            while (_service.BuscarDiaContable(fecha) != null)
            {
                fecha = fecha.AddDays(1);
            }
            ViewBag.Fecha = fecha;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult AbrirDia(DateTime? fecha)
        {
            if (fecha != null)
            {
                _service.EmpezarDiaContable(fecha.Value);
                TempData["exito"] = "Dia abierto correctamente";
                return RedirectToAction("Index", "Inicio");
            }
            return View();
        }
    }
}