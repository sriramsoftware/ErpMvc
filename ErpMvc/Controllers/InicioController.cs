using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using CompraVentaCore.Models;
using ContabilidadBL;
using ContabilidadCore.Models;
using ErpMvc.Models;
using ErpMvc.ViewModels;
using SeguridadCore.Utiles;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class InicioController : Controller
    {
        private DbContext _db;
        private PeriodoContableService _periodoContableService;
        private CuentasServices _cuentasService;

        public InicioController(DbContext context)
        {
            _db = context;
            _periodoContableService = new PeriodoContableService(context);
            _cuentasService = new CuentasServices(context);
        }

        //private ErpContext db = new ErpContext();        
        public ActionResult Index()
        {
            //throw new Exception("Hola mi exception.");
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole(RolesMontin.Vendedor) || User.IsInRole(RolesMontin.CapitanDeSalon))
                {
                    return RedirectToAction("Index", "Comandas");
                }
                ViewBag.DiaContable = _periodoContableService.GetDiaContableActual() != null? _periodoContableService.GetDiaContableActual():null;
                ViewBag.VentasDiarias = 0;
                ViewBag.ImporteVentasDiarias = 0;

                ViewBag.ComprasDiarias = 0;
                ViewBag.ImporteComprasDiarias = 0;

                var porcientoCalcula = _db.Set<PorcientoMenu>().Where(p => p.SeCalcula).Select(p => p.ElaboracioId).ToList();

                ViewBag.MasVendidos =
                    _db.Set<DetalleDeVenta>()
                        .GroupBy(v => v.Elaboracion).Where(e => porcientoCalcula.Contains(e.Key.Id)).OrderByDescending(v => v.Sum(e => e.Cantidad))
                        .Take(10)
                        .Select(v => new
                        {
                            label = v.Key.Nombre,
                            value = v.Sum(e => e.Cantidad)
                        });


                var finanzas = new List<dynamic>();
                var movimientos = _cuentasService.GetMovimientosDeCuenta("Gastos").Where(g => g.TipoDeOperacion == TipoDeOperacion.Debito).ToList();
                movimientos.AddRange(_cuentasService.GetMovimientosDeCuenta("Caja").Where(g => g.TipoDeOperacion == TipoDeOperacion.Debito));
                //todo: arreglar grafica que no funciona bien
                finanzas.AddRange(movimientos.GroupBy(m => m.Asiento.DiaContable.Fecha.Date).Select(m => new
                {
                    period = String.Format("{0:yyyy-MM-dd}", m.Key),
                    gastos = m.Where(e => e.Cuenta.Nivel.Nombre == "Gastos").Sum(e => e.Importe),
                    ingresos = m.Where(e => e.Cuenta.Nivel.Nombre == "Caja").Sum(e => e.Importe)
                }));

                ViewBag.Finanzas = finanzas;
                return View();
            }
            return RedirectToAction("Autenticarse", "Seguridad");

        }
    }
}