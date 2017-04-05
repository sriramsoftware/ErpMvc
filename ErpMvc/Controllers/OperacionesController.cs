using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompraVentaCore.Models;
using ContabilidadBL;
using ContabilidadCore.Models;
using ErpMvc.Models;
using ErpMvc.ViewModels;

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

        public ActionResult Index()
        {
            
                if (_periodoContableService.NoHayDiaAbierto())
                {
                    _periodoContableService.EmpezarDiaContable(DateTime.Now);
                }
                ViewBag.DiaContable = _periodoContableService.GetDiaContableActual();
                return View();
            
        }

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

        public PartialViewResult ResumenDeOperacionesContables(int id)
        {
            //var diaContable = _periodoContableService.BuscarDiaContable(id);
            var operaciones = new List<ResumenDeOperaciones>();
            var movimientos =
                _cuentasServices.GetMovimientosDeCuenta("Caja").Where(m => m.Asiento.DiaContableId == id);
            operaciones.AddRange(movimientos.Select(m => new ResumenDeOperaciones()
            {
                Fecha = m.Asiento.Fecha,
                Importe = m.TipoDeOperacion == TipoDeOperacion.Debito? m.Importe: -m.Importe,
                Tipo = m.Asiento.Detalle.Substring(0, m.Asiento.Detalle.IndexOf(" ")),
                Detalle = m.Asiento.Detalle,
                Usuario = m.Asiento.Usuario.UserName
            }));
            ViewBag.ImporteAnterior = 0;
            return PartialView("_ResumenDeOperacionesConteblesPartial", operaciones);
        }
    }
}