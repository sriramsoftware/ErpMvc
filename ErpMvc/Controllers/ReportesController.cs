using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Data.Entity;
using AlmacenCore.Models;
using CajaCore.Models;
using ComercialCore.Models;
using CompraVentaCore.Models;
using ContabilidadBL;
using ContabilidadCore.Models;
using DevExpress.XtraReports.UI;
using ErpMvc.Models;
using ErpMvc.Reportes;
using ErpMvc.Utiles;
using ErpMvc.ViewModels;

namespace ErpMvc.Controllers
{
    [Authorize()]
    public class ReportesController : Controller
    {
        private PeriodoContableService _periodoContableService;
        private CuentasServices _cuentasServices;
        private DbContext _db;

        public ReportesController(DbContext context)
        {
            _db = context;
            _cuentasServices = new CuentasServices(context);
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
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Inventario()
        {
            var lista = new List<dynamic>();
            lista.Add(new { Nombre = "Almacen" });
            lista.AddRange(_db.Set<CentroDeCosto>().Select(c => new { Nombre = c.Nombre }));
            ViewBag.OrigenId = new SelectList(lista, "Nombre", "Nombre");
            return View();
        }
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
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
            }, "Id", "Mes");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult ComprasConComprobante(int mes)
        {
            var report = new ComprasConComprobantes(mes, NombreMeses(mes));
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Inventario(string origenId, DateTime? fecha)
        {
            var report = new Inventario(origenId, fecha);
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult ValeDeVenta(int id)
        {
            var report = new ValeDeVenta(id);
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);

            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
        }

        [Authorize(Roles = RolesMontin.Vendedor + ","+ RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Comanda(int id)
        {
            var report = new ComandaReport(id);
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);

            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
        }

        [AllowAnonymous]
        public ActionResult Cierre(int id)
        {
            var resumenCierre = new ResumenCierre(_db);
            var report = new Cierre(resumenCierre.VerResumen(id));
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);

            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Operaciones()
        {
            return View("OperacionesEnPeriodo");
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Operaciones(ParametrosViewModel parametros)
        {
            var report = new Operaciones(parametros.FechaInicio, parametros.FechaFin);
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult ResumenDeGanaciasDiario()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult ResumenDeGanaciasDiario(ParametrosViewModel parametros)
        {
            var report = new ResumenDeGanaciaDiaria(parametros.FechaInicio);
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
        }
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Ventas()
        {
            return View();
        }
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult VentasAlCosto()
        {
            return View();
        }
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult VentasCuentaCasa()
        {
            return View();
        }
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult VentasPorFacturas()
        {
            return View();
        }
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Consumo()
        {
            return View();
        }
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult VentasPorProducto(int id, DateTime fechaInicio, DateTime fechaFin)
        {
            var productoConcreto = _db.Set<ProductoConcreto>().SingleOrDefault(p => p.Id == id);
            ViewBag.Producto = productoConcreto.Producto.Nombre;

            var menus = _db.Set<DetalleDeVenta>().Where(m => m.Venta.DiaContable.Fecha >= fechaInicio && m.Venta.DiaContable.Fecha <= fechaFin && (m.Elaboracion.Productos.Any(p => p.ProductoId == productoConcreto.ProductoId) || m.Agregados.Any(p => p.Agregado.ProductoId == id))).GroupBy(m => m.Elaboracion).Select(m => new MenusPorProductoViewModel()
            {
                Menu = m.Key.Nombre,
                CantidadVendida = (int)m.Sum(e => e.Cantidad),
            });
            return View("VentasDeProducto", menus);
        }
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult MovimientosDeProductos()
        {
            ViewBag.ProductoId = new SelectList(_db.Set<ProductoConcreto>().ToList(), "Id", "Producto.Nombre");
            var lista = new List<string>() { "Almacen" };
            lista.AddRange(_db.Set<CentroDeCosto>().Select(c => c.Nombre).ToList());
            ViewBag.Lugar = new SelectList(lista);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
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
                    (_db.Set<DetalleSalidaAlmacen>().Any(e => e.Vale.DiaContable.Fecha < fIni && e.Producto.ProductoId == parametros.ProductoId) ?
                    _db.Set<DetalleSalidaAlmacen>().Where(e => e.Vale.DiaContable.Fecha < fIni && e.Producto.ProductoId == parametros.ProductoId).Sum(e => e.Cantidad) : 0m) -
                    (_db.Set<SalidaPorMerma>().Any(e => e.DiaContable.Fecha < fIni && e.ExistenciaAlmacen.ProductoId == parametros.ProductoId) ?
                    _db.Set<SalidaPorMerma>().Where(e => e.DiaContable.Fecha < fIni && e.ExistenciaAlmacen.ProductoId == parametros.ProductoId).Sum(e => e.Cantidad * (e.ExistenciaAlmacen.Producto.UnidadDeMedida.FactorDeConversion / e.UnidadDeMedida.FactorDeConversion)) : 0m);
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
            //return PartialView("_MovDeProductosPartial");
        }
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult ResumenDeProducto()
        {
            ViewBag.ProductoId = new SelectList(_db.Set<ProductoConcreto>().ToList(), "Producto.Id", "Producto.Nombre");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public PartialViewResult ResumenDeProducto(ParametrosMovProdViewModel parametros)
        {
            var fIni = parametros.FechaInicio.Date;
            var fFin = parametros.FechaFin.Date.AddHours(23).AddMinutes(59);
            var productoConcreto = _db.Set<ProductoConcreto>()
                .FirstOrDefault(p => p.ProductoId == parametros.ProductoId);
            var compras = _db.Set<EntradaAlmacen>().Where(e => e.DiaContable.Fecha >= fIni &&
                                                                e.DiaContable.Fecha <= fFin &&
                                                                e.ProductoId == productoConcreto.Id).ToList();

            var ventas = _db.Set<DetalleDeVenta>().Include(d => d.Venta).Include(d => d.Elaboracion).Include(d => d.Agregados).Where(e => e.Venta.DiaContable.Fecha >= fIni &&
                                                                e.Venta.DiaContable.Fecha <= fFin &&
                                                                (e.Elaboracion.Productos.Any(p => p.ProductoId == parametros.ProductoId)
                                                                || e.Agregados.Any(a => a.Agregado.ProductoId == parametros.ProductoId))).ToList();

            var ventasNormales =
                ventas.Where(
                    d =>
                        d.Venta.Importe > 0 && (d.Venta.Observaciones ==null || !d.Venta.Observaciones.Contains("Venta al costo")) &&
                        d.Venta.EstadoDeVenta != EstadoDeVenta.PagadaPorFactura).ToList();

            var ventasAlCosto =
                ventas.Where(d => d.Venta.Observaciones != null && d.Venta.Observaciones.Contains("Venta al costo"));
            var ventasPorLaCasa =
                ventas.Where(d => d.Venta.Importe == 0);
            var ventasPorFactura =
                ventas.Where(d => d.Venta.EstadoDeVenta == EstadoDeVenta.PagadaPorFactura);

            var mermas =
               _db.Set<MovimientoDeProducto>().Include(e => e.Tipo).Include(e => e.Usuario).Include(e => e.CentroDeCosto).Include(e => e.Producto).Include(e => e.Producto.UnidadDeMedida).Where(e => e.DiaContable.Fecha >= fIni &&
               e.DiaContable.Fecha <= fFin && e.ProductoId == parametros.ProductoId && e.Tipo.Descripcion == TipoDeMovimientoConstantes.Merma).ToList();

            var entradasAjustes =
               _db.Set<MovimientoDeProducto>().Include(e => e.Tipo).Include(e => e.Usuario).Include(e => e.CentroDeCosto).Include(e => e.Producto).Include(e => e.Producto.UnidadDeMedida).Where(e => e.DiaContable.Fecha >= fIni &&
               e.DiaContable.Fecha <= fFin && e.ProductoId == parametros.ProductoId && e.Tipo.Descripcion == TipoDeMovimientoConstantes.EntradaPorAjuste).ToList();

            var salidasAjustes =
               _db.Set<MovimientoDeProducto>().Include(e => e.Tipo).Include(e => e.Usuario).Include(e => e.CentroDeCosto).Include(e => e.Producto).Include(e => e.Producto.UnidadDeMedida).Where(e => e.DiaContable.Fecha >= fIni &&
               e.DiaContable.Fecha <= fFin && e.ProductoId == parametros.ProductoId && e.Tipo.Descripcion == TipoDeMovimientoConstantes.SalidaPorAjuste).ToList();

            ViewBag.Entrada = compras;
            ViewBag.Ventas = ventas.Select(d => new ResumenProductoVM
            {
                Fecha = d.Venta.DiaContable.Fecha,
                Comanda = d.VentaId,
                Cantidad = d.Elaboracion.Productos.Where(p => p.ProductoId == parametros.ProductoId).Sum(p => d.Cantidad * p.Cantidad * productoConcreto.UnidadDeMedida.FactorDeConversion / p.UnidadDeMedida.FactorDeConversion) +
                d.Agregados.Where(a => a.Agregado.ProductoId == parametros.ProductoId).Sum(a => a.Cantidad * (a.Agregado.Cantidad * (productoConcreto.UnidadDeMedida.FactorDeConversion / a.Agregado.UnidadDeMedida.FactorDeConversion)))
            }).ToList();
            ViewBag.VentasNormales = ventasNormales.Select(d => new ResumenProductoVM
            {
                Fecha = d.Venta.DiaContable.Fecha,
                Comanda = d.VentaId,
                Cantidad = d.Elaboracion.Productos.Where(p => p.ProductoId == parametros.ProductoId).Sum(p => d.Cantidad * p.Cantidad * productoConcreto.UnidadDeMedida.FactorDeConversion / p.UnidadDeMedida.FactorDeConversion) +
                d.Agregados.Where(a => a.Agregado.ProductoId == parametros.ProductoId).Sum(a => a.Cantidad * (a.Agregado.Cantidad * (productoConcreto.UnidadDeMedida.FactorDeConversion / a.Agregado.UnidadDeMedida.FactorDeConversion)))
            }).ToList();
            ViewBag.AlCosto = ventasAlCosto.Select(d => new ResumenProductoVM
            {
                Fecha = d.Venta.DiaContable.Fecha,
                Comanda = d.VentaId,
                Cantidad = d.Elaboracion.Productos.Where(p => p.ProductoId == parametros.ProductoId).Sum(p => d.Cantidad * p.Cantidad * productoConcreto.UnidadDeMedida.FactorDeConversion / p.UnidadDeMedida.FactorDeConversion) +
                d.Agregados.Where(a => a.Agregado.ProductoId == parametros.ProductoId).Sum(a => a.Cantidad * (a.Agregado.Cantidad * (productoConcreto.UnidadDeMedida.FactorDeConversion / a.Agregado.UnidadDeMedida.FactorDeConversion)))
            }).ToList();
            ViewBag.PorLaCasa = ventasPorLaCasa.Select(d => new ResumenProductoVM
            {
                Fecha = d.Venta.DiaContable.Fecha,
                Comanda = d.VentaId,
                Cantidad = d.Elaboracion.Productos.Where(p => p.ProductoId == parametros.ProductoId).Sum(p => d.Cantidad * p.Cantidad * productoConcreto.UnidadDeMedida.FactorDeConversion / p.UnidadDeMedida.FactorDeConversion) +
                d.Agregados.Where(a => a.Agregado.ProductoId == parametros.ProductoId).Sum(a => a.Cantidad * (a.Agregado.Cantidad * (productoConcreto.UnidadDeMedida.FactorDeConversion / a.Agregado.UnidadDeMedida.FactorDeConversion)))
            }).ToList();
            ViewBag.PorFactura = ventasPorFactura.Select(d => new ResumenProductoVM
            {
                Fecha = d.Venta.DiaContable.Fecha,
                Comanda = d.VentaId,
                Cantidad = d.Elaboracion.Productos.Where(p => p.ProductoId == parametros.ProductoId).Sum(p => d.Cantidad * p.Cantidad * productoConcreto.UnidadDeMedida.FactorDeConversion / p.UnidadDeMedida.FactorDeConversion) +
                d.Agregados.Where(a => a.Agregado.ProductoId == parametros.ProductoId).Sum(a => a.Cantidad * (a.Agregado.Cantidad * (productoConcreto.UnidadDeMedida.FactorDeConversion / a.Agregado.UnidadDeMedida.FactorDeConversion)))
            }).ToList();
            ViewBag.Merma = mermas;
            ViewBag.EntradasPorAjuste = entradasAjustes;
            ViewBag.SalidasPorAjuste = salidasAjustes;
            ViewBag.ProductoId = parametros.ProductoId;
            return PartialView("_ResumenDeProductoPartial");
        }

        public ActionResult MenusPorProducto()
        {
            ViewBag.ProductoId = new SelectList(_db.Set<Producto>(),"Id","Nombre");
            return View();
        }

        public PartialViewResult MenusProducto(int productoId)
        {
            var menus = _db.Set<ProductosPorElaboracion>().Where(p => p.ProductoId == productoId).Select(p => p.Elaboracion).ToList();
            return PartialView("_MenusPorProductoPartial",menus);
        }

        public ActionResult FichaDeCosto(int id)
        {
            var report = new FichaDeCosto(id);
            string random = System.IO.Path.GetRandomFileName().Replace(".", string.Empty);
            reports.Add(random, report);
            ViewData["ReporteId"] = random;
            return View("Plantilla");
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