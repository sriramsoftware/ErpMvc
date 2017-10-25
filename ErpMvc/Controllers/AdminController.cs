using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlmacenCore.Models;
using CompraVentaCore.Models;
using ContabilidadCore.Models;
using ErpMvc.Models;
using Microsoft.AspNet.Identity;

namespace ErpMvc.Controllers
{
    [Authorize(Roles = RolesMontin.Administrador)]
    public class AdminController : Controller
    {
        public ActionResult DeleteInfo()
        {
            return View();
        }

        // GET: Admin
        public ActionResult DeleteInfoConfirmado()
        {
            var db = new ErpContext();
            db.Set<DetalleDeCompra>().RemoveRange(db.Set<DetalleDeCompra>());
            db.Set<Compra>().RemoveRange(db.Set<Compra>());

            db.Set<EntradaAlmacen>().RemoveRange(db.Set<EntradaAlmacen>());

            db.Set<DetalleSalidaAlmacen>().RemoveRange(db.Set<DetalleSalidaAlmacen>());
            db.Set<ValeSalidaDeAlmacen>().RemoveRange(db.Set<ValeSalidaDeAlmacen>());
            db.Set<SalidaPorMerma>().RemoveRange(db.Set<SalidaPorMerma>());

            db.Set<MovimientoDeProducto>().RemoveRange(db.Set<MovimientoDeProducto>());

            db.Set<OrdenPorDetalle>().RemoveRange(db.Set<OrdenPorDetalle>());
            db.Set<AgregadoDeComanda>().RemoveRange(db.Set<AgregadoDeComanda>());
            db.Set<DetalleDeComanda>().RemoveRange(db.Set<DetalleDeComanda>());
            db.Set<Orden>().RemoveRange(db.Set<Orden>());
            db.Set<Comanda>().RemoveRange(db.Set<Comanda>());

            db.Set<AgregadosVendidos>().RemoveRange(db.Set<AgregadosVendidos>());
            db.Set<DetalleDeVenta>().RemoveRange(db.Set<DetalleDeVenta>());
            db.Set<Venta>().RemoveRange(db.Set<Venta>());
            db.Set<Propina>().RemoveRange(db.Set<Propina>());

            db.Set<Asiento>().RemoveRange(db.Set<Asiento>());
            db.Set<DiaContable>().RemoveRange(db.Set<DiaContable>());

            var dia = new DiaContable() { Abierto = false, Fecha = DateTime.Now, HoraEnQueCerro = DateTime.Now};
            db.Set<DiaContable>().Add(dia);

            var existencias = db.Set<ExistenciaAlmacen>().ToList();

            foreach (var existencia in existencias)
            {
                if (existencia.ExistenciaEnAlmacen > 0)
                {
                    db.Set<EntradaAlmacen>()
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
            var existenciasCC = db.Set<ExistenciaCentroDeCosto>().ToList();
            foreach (var existencia in existenciasCC)
            {
                if (existencia.Cantidad > 0)
                {
                    db.Set<MovimientoDeProducto>().Add(new MovimientoDeProducto()
                    {
                        Fecha = DateTime.Now,
                        DiaContable = dia,
                        CentroDeCostoId = existencia.CentroDeCostoId,
                        ProductoId = existencia.ProductoId,
                        TipoId = db.Set<TipoDeMovimiento>().SingleOrDefault(t => t.Descripcion == TipoDeMovimientoConstantes.Entrada).Id,
                        Cantidad = existencia.Cantidad,
                        UsuarioId = User.Identity.GetUserId(),
                    });
                }
            }

            db.SaveChanges();

            TempData["exito"] = "Todo borrado";
            return RedirectToAction("Index", "Inicio");
        }
    }
}