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
    [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
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
            lista.Add(new { Nombre = "Almacen" });
            lista.AddRange(_db.Set<CentroDeCosto>().Select(c => new { Nombre = c.Nombre }));
            ViewBag.OrigenId = new SelectList(lista, "Nombre", "Nombre");
            return View();
        }

        public ActionResult ComprasConComprobante()
        {
            ViewBag.Mes = new SelectList(new List<dynamic>()
            {
                new {Id = 1, Mes = "Enero"},
                new {Id = 2, Mes = "Febrero"},
                new {Id = 3, Mes = "Marzo"},
                new {Id = 4, Mes = "Abril"},
                new {Id = 5, Mes = "Mayo"},
                new {Id = 6, Mes = "Junio"},
                new {Id = 7, Mes = "Julio"},
                new {Id = 8, Mes = "Agosto"},
                new {Id = 9, Mes = "Septiembre"},
                new {Id = 10, Mes = "Octubre"},
                new {Id = 11, Mes = "Noviembre"},
                new {Id = 12, Mes = "Diciembre"},
            },"Id","Mes");
            return View();
        }

        [HttpPost]
        public ActionResult ComprasConComprobante(int mes)
        {
            var report = new ComprasConComprobantes(mes, NombreMeses(mes));
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
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
            var report = new Operaciones(parametros.FechaInicio, parametros.FechaFin);
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
            var fFin = fecha.Date.AddHours(23).AddMinutes(59);
            ViewBag.Producto = _db.Set<ProductoConcreto>().SingleOrDefault(p => p.Id == id).Producto.Nombre;

            var menus = _db.Set<DetalleDeVenta>().Where(m => m.Venta.DiaContable.Fecha >= fIni && m.Venta.DiaContable.Fecha <= fFin && (m.Elaboracion.Productos.Any(p => p.ProductoId == id) || m.Agregados.Any(p => p.Agregado.ProductoId == id))).GroupBy(m => m.Elaboracion).Select(m => new MenusPorProductoViewModel()
            {
                Menu = m.Key.Nombre,
                CantidadVendida = (int)m.Sum(e => e.Cantidad),
            });
            return View("VentasDeProducto", menus);
        }

        public ActionResult MovimientosDeProductos()
        {
            ViewBag.ProductoId = new SelectList(_db.Set<ProductoConcreto>().ToList(), "Id", "Producto.Nombre");
            var lista = new List<string>() { "Almacen" };
            lista.AddRange(_db.Set<CentroDeCosto>().Select(c => c.Nombre).ToList());
            ViewBag.Lugar = new SelectList(lista);
            return View();
        }

        [HttpPost]
        public PartialViewResult MovimientosDeProductos(ParametrosMovProdViewModel parametros)
        {
            var fIni = parametros.FechaInicio.Date;
            var fFin = parametros.FechaFin.Date.AddHours(23).AddMinutes(59);
            if (parametros.Lugar == "Almacen")
            {
                var entradas =
                    _db.Set<EntradaAlmacen>().Where(e => e.DiaContable.Fecha >= fIni &&
                    e.DiaContable.Fecha <= fFin && e.ProductoId == parametros.ProductoId).ToList();
                var salidas =
                    _db.Set<DetalleSalidaAlmacen>()
                        .Where(e => e.Vale.DiaContable.Fecha >= fIni && e.Vale.DiaContable.Fecha <= fFin &&
                        e.Producto.ProductoId == parametros.ProductoId)
                        .ToList();
                var mermas =
                    _db.Set<SalidaPorMerma>()
                        .Where(e => e.DiaContable.Fecha >= fIni && e.DiaContable.Fecha <= fFin &&
                        e.ExistenciaAlmacen.ProductoId == parametros.ProductoId)
                        .ToList();

                var result = entradas.Select(ent => new DetalleMovimientoProductoViewModel()
                {
                    Fecha = ent.Fecha,
                    Lugar = ent.Almacen.Descripcion,
                    TipoDeMovimiento = "Entrada Almacen",
                    Cantidad = ent.Cantidad,
                    Unidad = ent.Producto.UnidadDeMedida.Siglas,
                    Usuario = ent.Usuario.UserName,
                }).ToList();
                result.AddRange(salidas.Select(sal => new DetalleMovimientoProductoViewModel()
                {
                    Fecha = sal.Vale.Fecha,
                    Lugar = sal.Vale.CentroDeCosto.Nombre,
                    TipoDeMovimiento = "Salida",
                    Cantidad = -sal.Cantidad,
                    Unidad = sal.Producto.Producto.UnidadDeMedida.Siglas,
                    Usuario = sal.Vale.Usuario.UserName,
                }));
                result.AddRange(mermas.Select(merma => new DetalleMovimientoProductoViewModel()
                {
                    Fecha = merma.Fecha,
                    Lugar = "Almacen",
                    TipoDeMovimiento = "Merma",
                    Cantidad = -merma.Cantidad,
                    Unidad = merma.ExistenciaAlmacen.Producto.UnidadDeMedida.Siglas,
                    Usuario = merma.Usuario.UserName,
                }));
                ViewBag.SaldoAnterior = (_db.Set<EntradaAlmacen>().Any(e => e.DiaContable.Fecha < fIni && e.ProductoId == parametros.ProductoId) ?
                    _db.Set<EntradaAlmacen>().Where(e => e.DiaContable.Fecha < fIni && e.ProductoId == parametros.ProductoId).Sum(e => e.Cantidad) : 0m) -
                    (_db.Set<DetalleSalidaAlmacen>().Any(e => e.Vale.DiaContable.Fecha < fIni && e.ProductoId == parametros.ProductoId) ?
                    _db.Set<DetalleSalidaAlmacen>().Where(e => e.Vale.DiaContable.Fecha < fIni && e.ProductoId == parametros.ProductoId).Sum(e => e.Cantidad) : 0m) -
                    (_db.Set<SalidaPorMerma>().Any(e => e.DiaContable.Fecha < fIni && e.ExistenciaAlmacen.ProductoId == parametros.ProductoId) ?
                    _db.Set<SalidaPorMerma>().Where(e => e.DiaContable.Fecha < fIni && e.ExistenciaAlmacen.ProductoId == parametros.ProductoId).Sum(e => e.Cantidad) : 0m);
                return PartialView("_MovDeProductosPartial", result.OrderBy(r => r.Fecha));
            }
            else
            {
                var movimientos =
                   _db.Set<MovimientoDeProducto>().Include(e => e.Tipo).Include(e => e.Usuario).Include(e => e.CentroDeCosto).Include(e => e.Producto).Include(e => e.Producto.UnidadDeMedida).Where(e => e.DiaContable.Fecha >= fIni &&
                   e.DiaContable.Fecha <= fFin && e.ProductoId == parametros.ProductoId && e.CentroDeCosto.Nombre == parametros.Lugar).ToList();

                var result = movimientos.Select(m => new DetalleMovimientoProductoViewModel()
                {
                    Fecha = m.Fecha,
                    Lugar = m.CentroDeCosto.Nombre,
                    TipoDeMovimiento = m.Tipo.Descripcion,
                    Cantidad = m.Tipo.Factor * (m.Cantidad),
                    Unidad = m.Producto.UnidadDeMedida.Siglas,
                    Usuario = m.Usuario.UserName,
                }).ToList();

                ViewBag.SaldoAnterior = (_db.Set<MovimientoDeProducto>().Any(e => e.DiaContable.Fecha < fIni &&
                e.ProductoId == parametros.ProductoId && e.CentroDeCosto.Nombre == parametros.Lugar) ?
                    _db.Set<MovimientoDeProducto>().Where(e => e.DiaContable.Fecha < fIni &&
                e.ProductoId == parametros.ProductoId && e.CentroDeCosto.Nombre == parametros.Lugar).Sum(e => e.Cantidad * e.Tipo.Factor) : 0m);
                return PartialView("_MovDeProductosPartial", result.OrderBy(r => r.Fecha));
            }
            return PartialView("_MovDeProductosPartial");
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
                ventasSinPorciento = ventas.Sum(v => v.Elaboraciones.Where(e => porcientos.Any(p => p.ElaboracioId == e.ElaboracionId && !p.SeCalcula)).Sum(s => s.ImporteTotal)) + ventas.Where(v => v.Observaciones == "Venta al costo").Sum(v => v.Importe);
                centrosDeCosto = ventas.GroupBy(v => v.PuntoDeVenta.CentroDeCosto).Select(v => new { v.Key.Nombre, Importe = v.Sum(s => s.Importe) }).ToList();
            }

            var extracciones =
                _cuentasServices.GetMovimientosDeCuenta("Caja")
                .Where(m => m.Asiento.DiaContableId == dia.Id && (m.Asiento.Detalle.StartsWith("Extracción") || m.Asiento.Detalle.StartsWith("Pago") || m.Asiento.Detalle.StartsWith("Compra"))).Sum(m => m.Importe);

            var extraccionCierre =
                _cuentasServices.GetMovimientosDeCuenta("Caja")
                .Where(m => m.Asiento.DiaContableId == dia.Id && (m.Asiento.Detalle.StartsWith("Cierre"))).Sum(m => m.Importe);

            var pagoTrabajadores =
                            _cuentasServices.GetMovimientosDeCuenta("Caja")
                            .Where(m => m.Asiento.DiaContableId == dia.Id && (m.Asiento.Detalle.StartsWith("Trabajadores"))).Sum(m => m.Importe);

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
                Fecha = cierre.DiaContable.Fecha,
                EfectivoAnterior = 100,
                Ventas = totalVentas,
                VentasSinPorciento = ventasSinPorciento,
                Depositos = depositos,
                Extracciones = extracciones,
                Propinas = propinas,
                ExtraccionCierre = extraccionCierre,
                PagoTrabajadores = pagoTrabajadores,
                Desgloce = cierre.Desglose.ToList()
                //CentrosDeCosto = centrosDeCosto
            };
            return resumen;
        }

        private string NombreMeses(int mes)
        {
            switch (mes)
            {
                case 1:
                    return "Enero";
                case 2:
                    return "Febrero";
                case 3:
                    return "Marzo";
                case 4:
                    return "Abril";
                case 5:
                    return "Mayo";
                case 6:
                    return "Junio";
                case 7:
                    return "Julio";
                case 8:
                    return "Agosto";
                case 9:
                    return "Septiembre";
                case 10:
                    return "Octubre";
                case 11:
                    return "Noviembre";
                case 12:
                    return "Diciembre";

            }
            return "";
        }

    }
}