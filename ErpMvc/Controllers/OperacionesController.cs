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

namespace ErpMvc.Controllers
{
    [Authorize]
    public class OperacionesController : Controller
    {
        private DbContext _db;
        private PeriodoContableService _periodoContableService;
        private CuentasServices _cuentasServices;

        public OperacionesController(DbContext context)
        {
            _db = context;
            _periodoContableService = new PeriodoContableService(context);
            _cuentasServices = new CuentasServices(context);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Index()
        {
            ViewBag.DiaContable = _periodoContableService.GetDiaContableActual();
            return View();
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public PartialViewResult ResumenDeOperaciones(int id)
        {
            var diaContable = _periodoContableService.BuscarDiaContable(id);

            var operaciones = new List<ResumenDeOperaciones>();

            operaciones.AddRange(_db.Set<OtrosGastos>().Where(g => g.DiaContableId == id).Select(o => new ResumenDeOperaciones()
            {
                Detalle = o.ConceptoDeGasto.Nombre,
                Importe = -o.Importe,
                CentroDeCosto = "Ninguno",
                Fecha = o.DiaContable.Fecha,
                Usuario = "No tiene",
                Tipo = "Otros gastos"
            }));
            var ventas = _db.Set<Venta>().Where(g => g.DiaContableId == id).ToList();
            var compras = _db.Set<Compra>().Where(g => g.DiaContableId == id).ToList();
            foreach (var venta in ventas)
            {
                var fecha = venta.Fecha;

                operaciones.Add(new ResumenDeOperaciones()
                {
                    Tipo = "Venta",
                    Detalle = venta.Resumen,
                    Importe = venta.Importe,
                    Fecha = fecha,
                    CentroDeCosto = venta.PuntoDeVenta.CentroDeCosto.Nombre,
                    Usuario = venta.Usuario.UserName
                });
            }
            foreach (var compra in compras)
            {
                operaciones.Add(new ResumenDeOperaciones()
                {
                    Tipo = "Compra",
                    Detalle = string.Join(",", compra.Productos.Select(p => p.Producto.Nombre)),
                    Importe = -compra.Productos.Sum(p => p.ImporteTotal),
                    Fecha = compra.Fecha,
                    CentroDeCosto = "Ninguno",
                    Usuario = compra.Usuario.UserName
                });
            }
            return PartialView("_ResumenDeOperacionesPartial", operaciones);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult ResumenDeOperacionesContablesPorFecha(DateTime fecha)
        {
            var fIni = fecha.Date;
            var fFin = fecha.Date.AddHours(23).AddMinutes(59);
            var dia = _db.Set<DiaContable>().FirstOrDefault(d => d.Fecha >= fIni && d.Fecha <= fFin);
            if (dia == null)
            {
                return PartialView("_ResumenDeOperacionesConteblesPartial");
            }
            return RedirectToAction("ResumenDeOperacionesContables", "Operaciones", new { Id = dia.Id });
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public PartialViewResult ResumenDeOperacionesContables(int id)
        {
            var operaciones = ResumenDeOperaciones(id, "Caja");
            var diaAnterior = _db.Set<DiaContable>().Find(id - 1);
            
            ViewBag.ImporteAnterior = 100;
            return PartialView("_ResumenDeOperacionesConteblesPartial", operaciones);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult AgregarOperacion()
        {
            ViewBag.CuentaCreditoId = new SelectList(_db.Set<Cuenta>().ToList(), "Id", "Nombre");
            ViewBag.CuentaDebitoId = new SelectList(_db.Set<Cuenta>().ToList(), "Id", "Nombre");
            return View();
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public JsonResult Cuentas()
        {
            var cuentas = _db.Set<Cuenta>().ToList().Select(c => new { Id = c.Id, Nombre = c.Nombre });
            return Json(cuentas, JsonRequestBehavior.AllowGet);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult AgregarOperacion(AsientoContableViewModel asiento)
        {
            if (ModelState.IsValid)
            {
                _cuentasServices.AgregarOperacion(asiento.CuentaCreditoId, asiento.CuentaDebitoId, asiento.Importe,
                    DateTime.Now, "Ajuste: " + asiento.Observaciones, User.Identity.GetUserId());
                _db.SaveChanges();
                TempData["exito"] = "Operacion realizada correctamente";
                return RedirectToAction("Index");
            }
            ViewBag.CuentaCreditoId = new SelectList(_db.Set<Cuenta>().ToList(), "Id", "Nombre", asiento.CuentaCreditoId);
            ViewBag.CuentaDebitoId = new SelectList(_db.Set<Cuenta>().ToList(), "Id", "Nombre", asiento.CuentaDebitoId);
            return View(asiento);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        private List<ResumenDeOperaciones> ResumenDeOperaciones(int diaContableId, string nombreCuenta)
        {
            var operaciones = new List<ResumenDeOperaciones>();
            var movimientos =
                _cuentasServices.GetMovimientosDeCuenta(nombreCuenta).Where(m => m.Asiento.DiaContableId == diaContableId);
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

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        private decimal CalculoImporte(ICollection<DenominacionesEnCierreDeCaja> denominaciones)
        {
            return denominaciones.Sum(d => (d.Cantidad * d.DenominacionDeMoneda.Valor));
        }
    }
}