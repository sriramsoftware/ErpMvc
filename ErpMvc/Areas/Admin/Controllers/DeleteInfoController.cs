using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlmacenCore.Models;
using CompraVentaCore.Models;
using ContabilidadCore.Models;
using ErpMvc.Models;
using Microsoft.AspNet.Identity;

namespace ErpMvc.Areas.Admin.Controllers
{
    [Authorize(Roles = RolesMontin.Administrador)]
    public class DeleteInfoController : Controller
    {
        private DbContext _db;

        public DeleteInfoController(DbContext context)
        {
            _db = context;
        }

        // GET: Admin/DeleteInfo
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult DeleteCompras(DateTime fechaInicio, DateTime fechaFin)
        {
            var fIni = fechaInicio.Date;
            var fFin = fechaFin.Date.AddHours(23).AddMinutes(59);
            var compras = _db.Set<Compra>().Where(c => c.Fecha >= fIni && c.Fecha <= fFin);
            var comprasABorrar = compras.Where(c => !_db.Set<SeleccionCompra>().Any(sc => sc.CompraId == c.Id));
            _db.Set<DetalleDeCompra>().RemoveRange(comprasABorrar.SelectMany(c => c.Productos));
            _db.Set<Compra>().RemoveRange(comprasABorrar);
            try
            {
                _db.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        public JsonResult DeleteNotCompras(DateTime? fechaInicio, DateTime? fechaFin)
        {
            //var fIni = fechaInicio.Value.Date;
            //var fFin = fechaFin.Value.Date.AddHours(23).AddMinutes(59);
            _db.Set<EntradaAlmacen>().RemoveRange(_db.Set<EntradaAlmacen>());

            _db.Set<DetalleSalidaAlmacen>().RemoveRange(_db.Set<DetalleSalidaAlmacen>());
            _db.Set<ValeSalidaDeAlmacen>().RemoveRange(_db.Set<ValeSalidaDeAlmacen>());
            _db.Set<SalidaPorMerma>().RemoveRange(_db.Set<SalidaPorMerma>());

            _db.Set<MovimientoDeProducto>().RemoveRange(_db.Set<MovimientoDeProducto>());
            //borrando all menos ventas seleccionadas y todas las compras
            var ventasNoBorrar = _db.Set<SeleccionVenta>().Select(s => s.VentaId).ToList();

            _db.Set<OrdenPorDetalle>().RemoveRange(_db.Set<OrdenPorDetalle>().Where(o => ventasNoBorrar.All(v => v != o.DetalleDeComanda.Comanda.VentaId)));
            _db.Set<AgregadoDeComanda>().RemoveRange(_db.Set<AgregadoDeComanda>().Where(o => ventasNoBorrar.All(v => v != o.DetalleDeComanda.Comanda.VentaId)));
            _db.Set<DetalleDeComanda>().RemoveRange(_db.Set<DetalleDeComanda>().Where(o => ventasNoBorrar.All(v => v != o.Comanda.VentaId)));
            _db.Set<Orden>().RemoveRange(_db.Set<Orden>().Where(o => ventasNoBorrar.All(v => v != o.Comanda.VentaId)));
            _db.Set<Comanda>().RemoveRange(_db.Set<Comanda>().Where(o => ventasNoBorrar.All(v => v != o.VentaId)));

            _db.Set<AgregadosVendidos>().RemoveRange(_db.Set<AgregadosVendidos>().Where(o => ventasNoBorrar.All(v => v != o.DetalleDeVenta.VentaId)));
            _db.Set<DetalleDeVenta>().RemoveRange(_db.Set<DetalleDeVenta>().Where(o => ventasNoBorrar.All(v => v != o.VentaId)));
            _db.Set<Venta>().RemoveRange(_db.Set<Venta>().Where(o => ventasNoBorrar.All(v => v != o.Id)));
            _db.Set<Propina>().RemoveRange(_db.Set<Propina>());

            _db.Set<Asiento>().RemoveRange(_db.Set<Asiento>());
            // db.Set<DiaContable>().RemoveRange(db.Set<DiaContable>());

            //var dia = new DiaContable() { Abierto = false, Fecha = DateTime.Now, HoraEnQueCerro = DateTime.Now };
            //db.Set<DiaContable>().Add(dia);
            var dia = _db.Set<DiaContable>().ToList().OrderBy(d => d.Fecha).Last();

            var existencias = _db.Set<ExistenciaAlmacen>().ToList();

            foreach (var existencia in existencias)
            {
                if (existencia.ExistenciaEnAlmacen > 0)
                {
                    _db.Set<EntradaAlmacen>()
                        .Add(new EntradaAlmacen()
                        {
                            Fecha = DateTime.Now,
                            DiaContable = dia,
                            AlmacenId = existencia.AlmacenId,
                            ProductoId = existencia.ProductoId,
                            UsuarioId = User.Identity.GetUserId(),
                            Cantidad = existencia.ExistenciaEnAlmacen
                        });
                }
            }
            var existenciasCC = _db.Set<ExistenciaCentroDeCosto>().ToList();
            foreach (var existencia in existenciasCC)
            {
                if (existencia.Cantidad > 0)
                {
                    _db.Set<MovimientoDeProducto>().Add(new MovimientoDeProducto()
                    {
                        Fecha = DateTime.Now,
                        DiaContable = dia,
                        CentroDeCostoId = existencia.CentroDeCostoId,
                        ProductoId = existencia.ProductoId,
                        TipoId = _db.Set<TipoDeMovimiento>().SingleOrDefault(t => t.Descripcion == TipoDeMovimientoConstantes.Entrada).Id,
                        Cantidad = existencia.Cantidad,
                        UsuarioId = User.Identity.GetUserId(),
                    });
                }
            }

            try
            {
                _db.SaveChanges();
                return Json(true);
            }
            catch (Exception ex)
            {
                return Json(false);
            }
        }

        public ActionResult DeleteInfoConfirmado()
        {
            //db.Set<DetalleDeCompra>().RemoveRange(db.Set<DetalleDeCompra>());
            //db.Set<Compra>().RemoveRange(db.Set<Compra>());

            _db.Set<EntradaAlmacen>().RemoveRange(_db.Set<EntradaAlmacen>());

            _db.Set<DetalleSalidaAlmacen>().RemoveRange(_db.Set<DetalleSalidaAlmacen>());
            _db.Set<ValeSalidaDeAlmacen>().RemoveRange(_db.Set<ValeSalidaDeAlmacen>());
            _db.Set<SalidaPorMerma>().RemoveRange(_db.Set<SalidaPorMerma>());

            _db.Set<MovimientoDeProducto>().RemoveRange(_db.Set<MovimientoDeProducto>());

            _db.Set<OrdenPorDetalle>().RemoveRange(_db.Set<OrdenPorDetalle>());
            _db.Set<AgregadoDeComanda>().RemoveRange(_db.Set<AgregadoDeComanda>());
            _db.Set<DetalleDeComanda>().RemoveRange(_db.Set<DetalleDeComanda>());
            _db.Set<Orden>().RemoveRange(_db.Set<Orden>());
            _db.Set<Comanda>().RemoveRange(_db.Set<Comanda>());

            _db.Set<AgregadosVendidos>().RemoveRange(_db.Set<AgregadosVendidos>());
            _db.Set<DetalleDeVenta>().RemoveRange(_db.Set<DetalleDeVenta>());
            _db.Set<Venta>().RemoveRange(_db.Set<Venta>());
            _db.Set<Propina>().RemoveRange(_db.Set<Propina>());

            _db.Set<Asiento>().RemoveRange(_db.Set<Asiento>());
           // db.Set<DiaContable>().RemoveRange(db.Set<DiaContable>());

            //var dia = new DiaContable() { Abierto = false, Fecha = DateTime.Now, HoraEnQueCerro = DateTime.Now };
            //db.Set<DiaContable>().Add(dia);
            var dia = _db.Set<DiaContable>().Last();

            var existencias = _db.Set<ExistenciaAlmacen>().ToList();

            foreach (var existencia in existencias)
            {
                if (existencia.ExistenciaEnAlmacen > 0)
                {
                    _db.Set<EntradaAlmacen>()
                        .Add(new EntradaAlmacen()
                        {
                            Fecha = DateTime.Now,
                            DiaContable = dia,
                            AlmacenId = existencia.AlmacenId,
                            ProductoId = existencia.ProductoId,
                            UsuarioId = User.Identity.GetUserId(),
                            Cantidad = existencia.ExistenciaEnAlmacen
                        });
                }
            }
            var existenciasCC = _db.Set<ExistenciaCentroDeCosto>().ToList();
            foreach (var existencia in existenciasCC)
            {
                if (existencia.Cantidad > 0)
                {
                    _db.Set<MovimientoDeProducto>().Add(new MovimientoDeProducto()
                    {
                        Fecha = DateTime.Now,
                        DiaContable = dia,
                        CentroDeCostoId = existencia.CentroDeCostoId,
                        ProductoId = existencia.ProductoId,
                        TipoId = _db.Set<TipoDeMovimiento>().SingleOrDefault(t => t.Descripcion == TipoDeMovimientoConstantes.Entrada).Id,
                        Cantidad = existencia.Cantidad,
                        UsuarioId = User.Identity.GetUserId(),
                    });
                }
            }

            _db.SaveChanges();

            TempData["exito"] = "Todo borrado";
            return RedirectToAction("Index", "Inicio");
        }
    }
}