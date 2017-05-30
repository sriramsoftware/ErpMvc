using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using AlmacenCore.Models;
using CajaCore.Models;
using CompraVentaCore.Models;
using ContabilidadBL;
using ContabilidadCore.Models;
using DevExpress.XtraReports.UI;
using ErpMvc.Models;
using ErpMvc.Reportes;
using ErpMvc.ViewModels;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private PeriodoContableService _periodoContableService;
        private CuentasServices _cuentasServices;
        private SubmayorService _submayorService;
        private DbContext _db;

        public ReportesController(DbContext context)
        {
            _db = context;
            _cuentasServices = new CuentasServices(context);
            _submayorService = new SubmayorService(context);
            _periodoContableService = new PeriodoContableService(context);     
        }

        static Dictionary<string, XtraReport> reports = new Dictionary<string, XtraReport>();

        [AllowAnonymous]
        public ActionResult ReportViewerPartial(string reporteId)
        {
            return PartialView("ReportViewerPartial", reports[reporteId]);
        }
        [AllowAnonymous]
        public ActionResult ExportReportViewer(string reporteId)
        {
            var reporte = reports[reporteId];
            reports.Remove(reporteId);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(reporte);
        }

        public ActionResult Inventario()
        {
            var lista = new List<dynamic>();
            lista.Add(new {Nombre = "Almacen"});
            lista.AddRange(_db.Set<CentroDeCosto>().Select(c => new { Nombre = c.Nombre}));
            ViewBag.OrigenId = new SelectList(lista,"Nombre","Nombre");
            return View();
        }

        public ActionResult ComprasPersonalizado()
        {
            return View();
        }

        [HttpPost]
        public PartialViewResult ComprasPersonalizado(ParametrosViewModel parametros)
        {
            var compras =
                _db.Set<Compra>().Where(c => c.Fecha >= parametros.FechaInicio && c.Fecha <= parametros.FechaFin);
            return PartialView("_ListaDeComprasPartial", compras.ToList());
        }

        [HttpPost]
        public ActionResult Inventario(string origenId)
        {

            var report = new Inventario(origenId);
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
        }


        public ActionResult ValeDeVenta(int id)
        {
            var report = new ValeDeVenta(id);
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
            
            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
        }

        [AllowAnonymous]
        public ActionResult Cierre(int id)
        {
            var report = new Cierre(ResumenCierre(id));
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);

            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
        }


        public ActionResult Operaciones()
        {
            return View("OperacionesEnPeriodo");
        }

        [HttpPost]
        public ActionResult Operaciones(ParametrosViewModel parametros)
        {
            var report = new Operaciones(parametros.FechaInicio,parametros.FechaFin);
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
        }

        public ActionResult ResumenDeGanaciasDiario()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ResumenDeGanaciasDiario(ParametrosViewModel parametros)
        {
            var report = new ResumenDeGanaciaDiaria(parametros.FechaInicio);
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
        }

        public ActionResult Ventas()
        {
            return View();
        }

        public ActionResult Consumo()
        {
            return View();
        }

        public ActionResult VentasPorProducto(int id, DateTime fecha)
        {
            var fIni = fecha.Date;
            var fFin = fecha.Date.AddHours(23);
            ViewBag.Producto = _db.Set<ProductoConcreto>().SingleOrDefault(p => p.Id == id).Producto.Nombre;

            var menus = _db.Set<DetalleDeVenta>().Where(m => m.Venta.Fecha >= fIni && m.Venta.Fecha <= fFin && (m.Elaboracion.Productos.Any(p => p.ProductoId == id) || m.Agregados.Any(p => p.Agregado.ProductoId == id))).GroupBy(m => m.Elaboracion).Select(m => new MenusPorProductoViewModel()
            {
                Menu = m.Key.Nombre,
                CantidadVendida = (int)m.Sum(e => e.Cantidad),
            });
            return View("VentasDeProducto", menus);
        }



        public CierreViewModel ResumenCierre(int id)
        {
            var cierre = _db.Set<CierreDeCaja>().Find(id);
            var dia = cierre.DiaContable;
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
                .Where(m => m.Asiento.DiaContableId == dia.Id && m.TipoDeOperacion == TipoDeOperacion.Credito).Sum(m => m.Importe);

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
            var resumen = new CierreViewModel()
            {
                Fecha = cierre.Fecha,
                EfectivoAnterior = efectivoAnterior,
                Ventas = totalVentas,
                VentasSinPorciento = ventasSinPorciento,
                Depositos = depositos,
                Extracciones = extracciones,
                Propinas = propinas,
                Desgloce = cierre.Desglose.ToList()
                //CentrosDeCosto = centrosDeCosto
            };
            return resumen;
        }

    }
}