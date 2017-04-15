using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using CompraVentaCore.Models;
using ContabilidadBL;
using DevExpress.XtraReports.UI;
using ErpMvc.Models;
using ErpMvc.Reportes;
using ErpMvc.ViewModels;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private ErpContext _db = new ErpContext();
        private PeriodoContableService _periodoContableService;

        static Dictionary<string, XtraReport> reports = new Dictionary<string, XtraReport>();

        public ActionResult ReportViewerPartial(string reporteId)
        {
            return PartialView("ReportViewerPartial", reports[reporteId]);
        }
        public ActionResult ExportReportViewer(string reporteId)
        {
            var reporte = reports[reporteId];
            reports.Remove(reporteId);
            return DevExpress.Web.Mvc.ReportViewerExtension.ExportTo(reporte);
        }


        public ActionResult Inventario()
        {
            var report = new Inventario();
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


        public ActionResult Operaciones()
        {
            return View("OperacionesEnPeriodo");
        }

        [HttpPost]
        public ActionResult Operaciones(ParametrosViewModel parametros)
        {
            //var diaInicial = _periodoContableService.BuscarDiaContable(fechaInicio);
            //var diaFinal = _periodoContableService.BuscarDiaContable(fechaFin);

            var operaciones = new List<ResumenDeOperaciones>();

            operaciones.AddRange(_db.Set<OtrosGastos>().ToList().Where(g => g.DiaContable.Fecha.Date >= parametros.FechaInicio.Date && g.DiaContable.Fecha.Date <= parametros.FechaFin.Date).Select(o => new ResumenDeOperaciones() { Fecha = o.DiaContable.Fecha,Detalle = o.ConceptoDeGasto.Nombre,Tipo = "Gasto", Importe = -o.Importe }));
            var ventas = _db.Set<Venta>().ToList().Where(g => g.DiaContable.Fecha.Date >= parametros.FechaInicio.Date && g.DiaContable.Fecha.Date <= parametros.FechaFin.Date).ToList();
            var compras = _db.Set<Compra>().ToList().Where(g => g.DiaContable.Fecha.Date >= parametros.FechaInicio.Date && g.DiaContable.Fecha.Date <= parametros.FechaFin.Date).ToList();
            foreach (var venta in ventas)
            {
                operaciones.Add(new ResumenDeOperaciones() {Fecha = venta.DiaContable.Fecha, Detalle = "Venta: " + string.Join(",", venta.Elaboraciones.Select(p => p.Elaboracion.Nombre)), Tipo = "Venta", Importe = venta.Importe });
            }
            foreach (var compra in compras)
            {
                operaciones.Add(new ResumenDeOperaciones() {Fecha = compra.DiaContable.Fecha, Detalle = "Compra de: " + string.Join(",", compra.Productos.Select(p => p.Producto.Nombre)),Tipo = "Compra", Importe = -compra.Productos.Sum(p => p.ImporteTotal) });
            }
            var report = new Operaciones(operaciones,parametros.FechaInicio,parametros.FechaFin);
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
        }

        //public ActionResult VisitasEnPeriodo()
        //{
        //    return View("VisitasEnPeriodo");
        //}

        //[HttpPost]
        //public ActionResult VisitasEnPeriodo(ParamViewModel param)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var report = new VisitasEnPeriodo(param);
        //        string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
        //        reports.Add(random, report);
        //        ViewData["ReporteId"] = random;
        //        return View("Plantilla");
        //    }
        //    ViewBag.VisitanteId = new SelectList(db.Visitantes, "Id", "FullName", param.VisitanteId);
        //    ViewBag.EntidadId = new SelectList(db.EntidadesSisca, "Id", "Descripcion", param.EntidadId);
        //    ViewBag.TrabajadorId = new SelectList(db.Trabajadores, "Id", "NombreCompleto", param.TrabajadorId);
        //    ViewBag.AutorizadoPorId = new SelectList(db.AutorizaVisitante, "TrabajadorId", "FullName", param.AutorizadoPorId);
        //    return View("VisitasEnPeriodo");
        //}

        //public ActionResult Visitante(int? id)
        //{
        //    if (id.Value == 1)
        //    {
        //        ViewBag.VisitanteId = new SelectList(db.VisitanteExtranjero, "Id", "FullName");
        //    }
        //    if (id.Value == 0)
        //    {
        //        ViewBag.VisitanteId = new SelectList(db.VisitantesNacionales, "Id", "FullName");
        //    }
        //    return PartialView("_VisitantePartial");
        //}

        //public ActionResult VehiculosEnPeriodo()
        //{
        //    ViewBag.VehiculoId = new SelectList(db.Vehiculos, "Id", "Matricula");
        //    ViewBag.ChoferId = new SelectList(db.Choferes, "Id", "FullName");
        //    ViewBag.AutorizaVehiculoId = new SelectList(db.AutorizaVehiculos, "TrabajadorId", "Trabajador.FullName");
        //    return View("VehiculosEnPeriodo");
        //}

        //[HttpPost]
        //public ActionResult VehiculosEnPeriodo(VehiculoViewModel periodov)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var report = new VehiculosEnPeriodo(periodov);
        //        string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
        //        reports.Add(random, report);
        //        ViewData["ReporteId"] = random;
        //        return View("Plantilla");
        //    }
        //    ViewBag.VehiculoId = new SelectList(db.Vehiculos, "Id", "Matricula", periodov.VehiculoId);
        //    ViewBag.ChoferId = new SelectList(db.Choferes, "Id", "FullName", periodov.ChoferId);
        //    ViewBag.AutorizaVehiculoId = new SelectList(db.AutorizaVehiculos, "TrabajadorId", "Trabajador.FullName", periodov.AutorizaVehiculoId);
        //    return View("VehiculosEnPeriodo");
        //}

    }
}