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
        private SubmayorService _submayorService;
        private DbContext _db;

        public PeriodoContableController(DbContext context)
        {
            _service = new PeriodoContableService(context);
            _cuentasServices = new CuentasServices(context);
            _submayorService = new SubmayorService(context);
            _db = context;
        }

        public ActionResult Cierres()
        {
            var cierres = _db.Set<CierreDeCaja>().ToList();
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
            var dia = _service.BuscarDiaContable(diaId);
            var cierreAnterior = _db.Set<CierreDeCaja>().OrderByDescending(d => d.DiaContable.Fecha).FirstOrDefault(d => d.DiaContable.Fecha < dia.Fecha);

            var porcientos = _db.Set<PorcientoMenu>().ToList();
            var efectivoAnterior = cierreAnterior != null ? cierreAnterior.Desglose.Sum(e => e.DenominacionDeMoneda.Valor * e.Cantidad) : 0;
            var totalVentas = 0m;
            var ventasSinPorciento = 0m;
            dynamic centrosDeCosto = 0;
            if (_db.Set<Venta>().Any(v => v.DiaContableId == dia.Id && (v.EstadoDeVenta == EstadoDeVenta.PagadaEnEfectivo || v.EstadoDeVenta == EstadoDeVenta.PagadaPorTarjeta)))
            {
                var ventas = _db.Set<Venta>()
                    .Where(
                        v =>
                            v.DiaContableId == dia.Id &&
                            (v.EstadoDeVenta == EstadoDeVenta.PagadaEnEfectivo ||
                             v.EstadoDeVenta == EstadoDeVenta.PagadaPorTarjeta)).ToList();
                totalVentas = ventas.Sum(v => v.Importe);
                ventasSinPorciento = ventas.Sum(v => v.Elaboraciones.Where(e => porcientos.Any(p => p.ElaboracioId == e.ElaboracionId && !p.SeCalcula)).Sum(s => s.ImporteTotal));
                centrosDeCosto = ventas.GroupBy(v => v.PuntoDeVenta.CentroDeCosto).Select(v => new { v.Key.Nombre, Importe = v.Sum(s => s.Importe) }).ToList();
            }

            var extracciones =
                _cuentasServices.GetMovimientosDeCuenta("Caja")
                .Where(m => m.Asiento.DiaContableId == dia.Id && m.TipoDeOperacion == TipoDeOperacion.Credito && (m.Asiento.Detalle.StartsWith("Extracción") ||m.Asiento.Detalle.StartsWith("Pago") ||m.Asiento.Detalle.StartsWith("Compra"))).Sum(m => m.Importe);

            var extraccionCierre =
                _cuentasServices.GetMovimientosDeCuenta("Caja")
                .Where(m => m.Asiento.DiaContableId == dia.Id && m.TipoDeOperacion == TipoDeOperacion.Credito && (m.Asiento.Detalle.StartsWith("Cierre"))).Sum(m => m.Importe);

            var depositos =
                _cuentasServices.GetMovimientosDeCuenta("Caja")
                .Where(m => m.Asiento.DiaContableId == dia.Id && m.TipoDeOperacion == TipoDeOperacion.Debito && m.Asiento.Detalle.StartsWith("Deposito")).Sum(m => m.Importe);

            //var compras = _db.Set<Compra>().Any(v => v.DiaContableId == dia.Id)
            //    ? _db.Set<Compra>()
            //        .Where(v => v.DiaContableId == dia.Id)
            //        .Sum(c => c.Productos.Any() ? c.Productos.Sum(p => p.ImporteTotal) : 0.0m)
            //    : 0;
            //var gastos = _db.Set<OtrosGastos>().Any(v => v.DiaContableId == dia.Id)
            //    ? _db.Set<OtrosGastos>().Where(v => v.DiaContableId == dia.Id).Sum(c => c.Importe)
            //    : 0;

            var propinas = _db.Set<Propina>().Any(v => v.Venta.DiaContableId == dia.Id)
                ? _db.Set<Propina>().Where(v => v.Venta.DiaContableId == dia.Id).Sum(c => c.Importe)
                : 0;
            var resumen = new
            {
                EfectivoAnterior = efectivoAnterior,
                Ventas = totalVentas,
                VentasSinPorciento = ventasSinPorciento,
                Depositos = depositos,
                Extracciones = extracciones,
                Propinas = propinas,
                CentrosDeCosto = centrosDeCosto
            };
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
        public JsonResult CerrarPeriodo(DesgloceEfectivoViewModel desgloceEfectivoViewModel, decimal importeAExtraer)
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
                var result = _submayorService.AgregarOperacion(cuentaCaja.Id, cuentaBanco.Id, importeAExtraer, DateTime.Now, "Cierre del dia",
                    User.Identity.GetUserId());
                if (result)
                {
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
                    _service.CerrarDiaContable(cierre);
                    //HttpContext.GetOwinContext().Authentication.SignOut();
                    return Json(new { result = true, cierreId = cierre.Id, mensaje = "Cierre correcto" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { result = false, mensaje = "No se puede realizar la extraccion de la caja" }, JsonRequestBehavior.AllowGet);
                }

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
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult AbrirDia(DateTime? fecha)
        {
            if (fecha != null)
            {
                //_service.CerrarDiaContable();
                _service.EmpezarDiaContable(fecha.Value);
                TempData["exito"] = "Dia abierto correctamente";
                return RedirectToAction("Index", "Inicio");
            }
            return View();
        }
    }
}