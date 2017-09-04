using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AlmacenCore.Models;
using CompraVentaBL;
using CompraVentaCore.Models;
using ContabilidadBL;
using ContabilidadCore.Models;
using ErpMvc.Models;
using ErpMvc.Utiles;
using ErpMvc.ViewModels;
using HumanResourcesCore.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class VentasController : Controller
    {
        private DbContext _db;
        private VentasService _ventasService;
        private PeriodoContableService _periodoContableService;

        public VentasController(DbContext context)
        {
            _db = context;
            _ventasService = new VentasService(context);
            _periodoContableService = new PeriodoContableService(context);
        }

        // GET: Ventas
        [DiaContable]
        public ActionResult Index()
        {
            var diaContable = _periodoContableService.GetDiaContableActual();
            ViewBag.DiaContable = diaContable;
            ViewBag.CantidadDeVentas = _ventasService.Ventas().Count(v => v.DiaContableId == diaContable.Id);
            return View();
        }

        public ActionResult Pendientes()
        {
            var diaContable = _periodoContableService.GetDiaContableActual();
            ViewBag.DiaContable = diaContable;
            ViewBag.CantidadDeVentas = _ventasService.Ventas().Count(v => v.EstadoDeVenta == EstadoDeVenta.PendienteParaOtroDia);
            return View();
        }

        public PartialViewResult ListaDeVentasPartial(int id)
        {
            ViewBag.Propinas = _db.Set<Propina>().Where(p => p.Venta.DiaContableId == id).ToList();
            var ventas = _ventasService.Ventas().Where(v => v.DiaContableId == id).ToList().OrderByDescending(v => v.Fecha);
            ViewBag.CantidadDeVentas = ventas.Count();
            return PartialView("_ListaDeVentasPartial",ventas);
        }

        public PartialViewResult ListaDeVentasPendientesPartial()
        {
            ViewBag.Propinas = _db.Set<Propina>().Where(p => p.Venta.EstadoDeVenta == EstadoDeVenta.PendienteParaOtroDia).ToList();
            var ventas = _ventasService.Ventas().Where(v => v.EstadoDeVenta == EstadoDeVenta.PendienteParaOtroDia).ToList().OrderByDescending(v => v.Fecha);
            ViewBag.CantidadDeVentas = ventas.Count();
            return PartialView("_ListaDeVentasPartial", ventas);
        }

        public PartialViewResult ListaDeVentasPorFechaPartial(DateTime fecha)
        {
            var fIni = fecha.Date;
            var fFin = fecha.Date.AddHours(23).AddMinutes(59);
            ViewBag.Propinas = _db.Set<Propina>().Where(p => p.Venta.DiaContable.Fecha >= fIni && p.Venta.DiaContable.Fecha <= fFin).ToList();
            var ventas = _ventasService.Ventas().Where(v => v.DiaContable.Fecha >= fIni && v.DiaContable.Fecha <= fFin).OrderByDescending(v => v.Fecha).ToList();
            ViewBag.CantidadDeVentas = ventas.Count();
            return PartialView("_ListaDeVentasSoloVerPartial", ventas);
        }

        public PartialViewResult ListaDeVentasFacturasPorFechaPartial(DateTime fecha)
        {
            var fIni = fecha.Date;
            var fFin = fecha.Date.AddHours(23).AddMinutes(59);
            ViewBag.Propinas = _db.Set<Propina>().Where(p => p.Venta.DiaContable.Fecha >= fIni && p.Venta.DiaContable.Fecha <= fFin && p.Venta.EstadoDeVenta == EstadoDeVenta.PagadaPorFactura).ToList();
            var ventas = _ventasService.Ventas().Where(v => v.DiaContable.Fecha >= fIni && v.DiaContable.Fecha <= fFin && v.EstadoDeVenta == EstadoDeVenta.PagadaPorFactura).OrderByDescending(v => v.Fecha).ToList();
            ViewBag.CantidadDeVentas = ventas.Count();
            return PartialView("_ListaDeVentasSoloVerPartial", ventas);
        }


        public PartialViewResult ListaDeVentasCuentaCasaPartial(DateTime fecha)
        {
            var fIni = fecha.Date;
            var fFin = fecha.Date.AddHours(23).AddMinutes(59);
            var cuentaCasa = _ventasService.Ventas().Where(v => v.DiaContable.Fecha >= fIni && v.DiaContable.Fecha <= fFin).SelectMany(v => v.Elaboraciones.Where(e => e.ImporteTotal == 0)).OrderByDescending(v => v.VentaId).ToList();
            return PartialView("_MenusCuentaCasaPartial", cuentaCasa);
        }

        public PartialViewResult ListaDeVentasAlCostoPartial(DateTime fecha)
        {
            var fIni = fecha.Date;
            var fFin = fecha.Date.AddHours(23).AddMinutes(59);
            ViewBag.Propinas = _db.Set<Propina>().Where(p => p.Venta.DiaContable.Fecha >= fIni && p.Venta.DiaContable.Fecha <= fFin).ToList();
            var ventas = _ventasService.Ventas().Where(v => v.DiaContable.Fecha >= fIni && v.DiaContable.Fecha <= fFin && v.Observaciones == "Venta al costo").OrderByDescending(v => v.Fecha).ToList();
            ViewBag.CantidadDeVentas = ventas.Count();
            return PartialView("_ListaDeVentasSoloVerPartial", ventas);
        }

        public PartialViewResult ConsumoPorFechaPartial(DateTime fechaInicio, DateTime fechaFin)
        {
            var fIni = fechaInicio.Date;
            var fFin = fechaFin.Date.AddHours(23).AddMinutes(59);

            var consumo = _db.Set<MovimientoDeProducto>().Where(m => m.DiaContable.Fecha >= fIni && m.DiaContable.Fecha <= fFin  && m.Tipo.Descripcion == TipoDeMovimientoConstantes.SalidaAProduccion).GroupBy(m => m.Producto).Select(m => new ConsumoViewModel()
            {
                ProductoId = m.Key.Id,
                Producto = m.Key.Producto.Nombre,
                Cantidad = m.Sum(p => p.Cantidad),
                Unidad = m.Key.UnidadDeMedida.Siglas,
                FechaInicio = fIni,
                FechaFin = fFin
            }).ToList();
            var errores = _db.Set<MovimientoDeProducto>().Where(m => m.DiaContable.Fecha >= fIni && m.DiaContable.Fecha <= fFin && m.Tipo.Descripcion == TipoDeMovimientoConstantes.EntradaPorErrorDeSalida).GroupBy(m => m.Producto).Select(m => new ConsumoViewModel()
            {
                ProductoId = m.Key.Id,
                Producto = m.Key.Producto.Nombre,
                Cantidad = m.Sum(p => p.Cantidad),
                Unidad = m.Key.UnidadDeMedida.Siglas,
                FechaInicio = fIni,
                FechaFin = fFin
            }).ToList();
            foreach (var error in errores)
            {
                if (consumo.Any(c => c.ProductoId == error.ProductoId))
                {
                    consumo.SingleOrDefault(c => c.ProductoId == error.ProductoId).Cantidad -= error.Cantidad;
                }
            }
            return PartialView("_ConsumoPartial", consumo);
        }
        [DiaContable]
        public ActionResult NuevaVenta()
        {
            ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre");
            ViewBag.VendedorId = new SelectList(_ventasService.Vendedores().Where(v => v.Estado == EstadoTrabajador.Activo), "Id", "NombreCompleto");
            return View();
        }

        [DiaContable]
        public ActionResult VentaAlCosto()
        {
            ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre");
            ViewBag.VendedorId = new SelectList(_ventasService.Vendedores().Where(v => v.Estado == EstadoTrabajador.Activo), "Id", "NombreCompleto");
            return View();
        }

        [DiaContable]
        public ActionResult VentaPorFactura()
        {
            ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre");
            ViewBag.VendedorId = new SelectList(_ventasService.Vendedores().Where(v => v.Estado == EstadoTrabajador.Activo), "Id", "NombreCompleto");
            return View();
        }

        [HttpPost]
        public ActionResult BuscarVentas(DateTime fecha)
        {
            return RedirectToAction("ListaDeVentasPorFechaPartial", new {Fecha = fecha});
        }

        [HttpPost]
        public ActionResult BuscarVentasAlCosto(DateTime fecha)
        {
            return RedirectToAction("ListaDeVentasAlCostoPartial", new { Fecha = fecha });
        }

        [HttpPost]
        public ActionResult BuscarVentasCuentaCasa(DateTime fecha)
        {
            return RedirectToAction("ListaDeVentasCuentaCasaPartial", new { Fecha = fecha });
        }
        
        [HttpPost]
        [DiaContable]
        public ActionResult NuevaVenta(Venta venta)
        {
            if (User.IsInRole(RolesMontin.Vendedor))
            {
                var usuarioId = User.Identity.GetUserId();
                var vendedor = _ventasService.Vendedores().SingleOrDefault(v => v.UsuarioId == usuarioId);
                if (vendedor == null)
                {
                    TempData["error"] = "No existe vendedor asociado a la cuenta, consulte al administrador";
                    return RedirectToAction("Index");
                }
                venta.VendedorId = vendedor.Id;
            }
            if (!venta.Elaboraciones.Any())
            {
                TempData["error"] = "No se puede efectuar una venta vacia";
                ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre");
                ViewBag.VendedorId = new SelectList(_ventasService.Vendedores(), "Id", "NombreCompleto");
                return View();
            }
            if (_ventasService.Vender(venta, User.Identity.GetUserId()))
            {
                TempData["exito"] = "Venta agregada correctamente";
                return RedirectToAction("Index");
            }
            ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre");
            ViewBag.VendedorId = new SelectList(_ventasService.Vendedores(), "Id", "NombreCompleto");
            return View();
        }

        [HttpPost]
        [DiaContable]
        public ActionResult VentaPorFactura(Venta venta)
        {
            if (User.IsInRole(RolesMontin.Vendedor))
            {
                var usuarioId = User.Identity.GetUserId();
                var vendedor = _ventasService.Vendedores().SingleOrDefault(v => v.UsuarioId == usuarioId);
                if (vendedor == null)
                {
                    TempData["error"] = "No existe vendedor asociado a la cuenta, consulte al administrador";
                    return RedirectToAction("Index");
                }
                venta.VendedorId = vendedor.Id;
            }
            if (!venta.Elaboraciones.Any())
            {
                TempData["error"] = "No se puede efectuar una venta vacia";
                ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre");
                ViewBag.VendedorId = new SelectList(_ventasService.Vendedores(), "Id", "NombreCompleto");
                return View();
            }
            if (_ventasService.Vender(venta, User.Identity.GetUserId()))
            {
                TempData["exito"] = "Venta agregada correctamente";
                return RedirectToAction("Index");
            }
            ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre");
            ViewBag.VendedorId = new SelectList(_ventasService.Vendedores(), "Id", "NombreCompleto");
            return View();
        }

        public PartialViewResult DetalleDeVentaAlCostoPartial()
        {
            return PartialView("_DetalleDeVentaAlCostoPartial");
        }

        public PartialViewResult DetalleDeVentaPartial()
        {
            return PartialView("_DetalleDeVentaPartial");
        }

        public PartialViewResult MenusEnVentaPartial(int id)
        {
            return PartialView("_MenusVendidosPartial", _ventasService.Ventas().Find(id).Elaboraciones);
        }


        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        [DiaContable]
        public ActionResult Editar(int id)
        {
            var venta = _ventasService.Ventas().Find(id);
            if (!User.IsInRole(RolesMontin.Administrador) && (venta.EstadoDeVenta == EstadoDeVenta.Facturada || venta.EstadoDeVenta == EstadoDeVenta.PagadaEnEfectivo || venta.EstadoDeVenta == EstadoDeVenta.PagadaPorTarjeta))
            {
                TempData["error"] = "Usted no puede editar una venta impresa o pagada";
                return RedirectToAction("Index");
            }
            
            ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre", venta.PuntoDeVentaId);
            ViewBag.VendedorId = new SelectList(_ventasService.Vendedores(), "Id", "NombreCompleto", venta.VendedorId);
            return View(venta);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        [DiaContable]
        public ActionResult Editar(Venta venta)
        {
            if (!User.IsInRole(RolesMontin.Administrador) && (venta.EstadoDeVenta == EstadoDeVenta.Facturada || venta.EstadoDeVenta == EstadoDeVenta.PagadaEnEfectivo || venta.EstadoDeVenta == EstadoDeVenta.PagadaPorTarjeta))
            {
                TempData["error"] = "Usted no puede editar una venta impresa o pagada";
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                var result = _ventasService.Editar(venta, User.Identity.GetUserId());
                if (result)
                {
                    result = _ventasService.GuardarCambios();
                    if (!result)
                    {
                        TempData["error"] = "No se pudo editar la venta, error al guardar los cambios";
                    }
                }
                else
                {
                    TempData["error"] = "No se pudo editar la venta, es posible que se cambiara el centro de costo y este no tenga los productos de la venta";
                }
                if (result)
                {
                    TempData["exito"] = "Venta editada correctamente";
                }
                return RedirectToAction("Index");
            }
            ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre", venta.PuntoDeVentaId);
            ViewBag.VendedorId = new SelectList(_ventasService.Vendedores(), "Id", "NombreCompleto", venta.VendedorId);
            return View(venta);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        [HttpPost]
        [ActionName("Eliminar")]
        [DiaContable]
        public ActionResult EliminarConfirmado(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var venta = _ventasService.Ventas().Find(id);
            if (!User.IsInRole(RolesMontin.Administrador) && (venta.EstadoDeVenta == EstadoDeVenta.Facturada || venta.EstadoDeVenta == EstadoDeVenta.PagadaEnEfectivo || venta.EstadoDeVenta == EstadoDeVenta.PagadaPorTarjeta))
            {
                TempData["error"] = "Usted no puede editar una venta impresa o pagada";
                return RedirectToAction("Index");
            }
            var comandas = _db.Set<Comanda>().Where(c => c.VentaId == id);
            foreach (var com in comandas)
            {
                com.VentaId = null;
            }
            if (_ventasService.EliminarVenta(id, User.Identity.GetUserId()))
            {
                var propina = _db.Set<Propina>().SingleOrDefault(p => p.VentaId == id);
                if (propina != null)
                {
                    _db.Set<Propina>().Remove(propina);
                }
                if (_ventasService.GuardarCambios())
                {
                    TempData["exito"] = "Comanda eliminada correctamente";
                }
                else
                {
                    TempData["error"] = "La comanda no se puede eliminar";
                }
            }
            else
            {
                TempData["error"] = "La comanda no se puede eliminar";
            }
            return RedirectToAction("Index");
        }

        public JsonResult SePuedeVender(int ventaId, int menuId, int cantidad)
        {
            var venta = _ventasService.Ventas().Find(ventaId);
            var result = _ventasService.SePuedeVender(menuId, cantidad, venta.PuntoDeVenta.CentroDeCostoId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public JsonResult DisminuirMenu(int id)
        {
            var result = _ventasService.DisminuirMenuEnVenta(id, User.Identity.GetUserId());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public JsonResult DisminuirAgregado(int detalleId, int agregadoId)
        {
            var result = _ventasService.DisminuirAgregadoEnVenta(detalleId,agregadoId, User.Identity.GetUserId());
            if (result)
            {
                result = _ventasService.GuardarCambios();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public JsonResult AumentarAgregado(int detalleId, int agregadoId)
        {
            var result = _ventasService.AumentarAgregadoEnVenta(detalleId,agregadoId, User.Identity.GetUserId());
            if (result)
            {
                result = _ventasService.GuardarCambios();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public JsonResult AumentarMenu(int id)
        {
            var result = _ventasService.AumentarMenuEnVenta(id, User.Identity.GetUserId());
            if (result)
            {
                result = _ventasService.GuardarCambios();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public JsonResult AgregarMenuAVenta(DetalleDeVenta detalle)
        {
            detalle.Elaboracion = null;
            var result = _ventasService.AgregarDetalleAVenta(detalle, User.Identity.GetUserId());
            if (result)
            {
                result = _ventasService.GuardarCambios();
            }
            return Json(new {Result = result, DetalleId = detalle.Id}, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public JsonResult EliminarDetalle(int id)
        {
            var result = _ventasService.EliminarDetalleAVenta(id, User.Identity.GetUserId());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult MenuPorCuentaCasa(int id)
        {
            var result = _ventasService.MenuPorCuentaCasa(id, User.Identity.GetUserId());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SePuedeVender(VerificarVentaViewModel venta)
        {
            var nuevaVenta = new Venta()
            {
                PuntoDeVentaId = venta.PuntoDeVentaId
            };
            foreach (var detalle in venta.Detalles)
            {
                nuevaVenta.Elaboraciones.Add(detalle);
            }
            if (venta.NuevoDetalle != null)
            {
                nuevaVenta.Elaboraciones.Add(venta.NuevoDetalle);
            }

            var result = _ventasService.SePuedeVender(nuevaVenta);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ImporteVenta(int id)
        {
            var venta = _ventasService.Ventas().Find(id);
            return Json(venta.Importe, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize]
        public ActionResult PagarVale(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var venta = _ventasService.Ventas().Find(id);
            if (venta == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            var result = _ventasService.PagarVentaEnEfectivo(venta.Id, User.Identity.GetUserId());
            if (result)
            {
                TempData["exito"] = "Se pago el vale correctamente";
            }
            else
            {
                TempData["errro"] = "Error al pagar";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public ActionResult Propina(int ventaId, decimal propina)
        {
            var venta = _db.Set<Venta>().Find(ventaId);
            if (venta.EstadoDeVenta != EstadoDeVenta.PagadaEnEfectivo)
            {
                TempData["error"] = "No se puede cobrar propina, la venta no esta en estado pagada";
                return RedirectToAction("Index");
            }    
            _db.Set<Propina>().Add(new Propina()
            {
                VentaId = ventaId,
                Importe = propina
            });
            var result = true;
            try
            {
                _db.SaveChanges();
            }
            catch (Exception)
            {
                result = false;
            }
            if (result)
            {
                TempData["exito"] = "Se cobro la propina correctamente";
            }
            else
            {
                TempData["error"] = "Error al cobrar propina";
            }
            return RedirectToAction("Index");
        }

        public ActionResult PasarAImpreso(int id)
        {
            var venta = _db.Set<Venta>().Find(id);
            if (venta.EstadoDeVenta == EstadoDeVenta.Pendiente)
            {
                venta.EstadoDeVenta = EstadoDeVenta.Facturada;
                _db.SaveChanges();
            }
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ImprimirReporte(int id)
        {
            var venta = _db.Set<Venta>().Find(id);
            if (venta.EstadoDeVenta == EstadoDeVenta.Pendiente)
            {
                venta.EstadoDeVenta = EstadoDeVenta.Facturada;
                _db.SaveChanges();
            }
            return RedirectToAction("ValeDeVenta", "Reportes", new {Id = id});
        }


        [HttpPost]
        [Authorize]
        public ActionResult PasarAOtroDia(int ventaId, string observacion)
        {
            var venta = _ventasService.Ventas().Find(ventaId);
            if (venta == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            venta.EstadoDeVenta = EstadoDeVenta.PendienteParaOtroDia;
            venta.Observaciones = observacion;
            if (_ventasService.GuardarCambios())
            {
                TempData["exito"] = "Se paso la venta para otro dia correctamente";
            }
            else
            {
                TempData["errro"] = "Error al pasar para otro dia";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public ActionResult PorCuentaDeLaCasa(int ventaId, string observacion)
        {
            var venta = _ventasService.Ventas().Find(ventaId);
            if (venta == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }
            venta.EstadoDeVenta = EstadoDeVenta.NoCobrada;
            venta.Importe = 0;
            foreach (var menu in venta.Elaboraciones)
            {
                menu.ImporteTotal = 0;
            }
            venta.Observaciones = observacion;
            if (_ventasService.GuardarCambios())
            {
                TempData["exito"] = "La venta se puso por cuenta de la casa correctamente";
            }
            else
            {
                TempData["errro"] = "Error al pasar por cuenta de la casa";
            }
            return RedirectToAction("Index");
        }


    }
}