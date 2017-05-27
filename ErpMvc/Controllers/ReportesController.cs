using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using AlmacenCore.Models;
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
            var lista = new List<dynamic>();
            lista.Add(new {Nombre = "Almacen"});
            lista.AddRange(_db.Set<CentroDeCosto>().Select(c => new { Nombre = c.Nombre}));
            ViewBag.OrigenId = new SelectList(lista,"Nombre","Nombre");
            return View();
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
    }
}