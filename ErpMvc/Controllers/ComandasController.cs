﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CompraVentaBL;
using CompraVentaCore.Models;
using ContabilidadBL;
using ErpMvc.Models;
using ErpMvc.Utiles;
using ErpMvc.ViewModels;
using HumanResourcesCore.Models;
using Microsoft.AspNet.Identity;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class ComandasController : Controller
    {
        private DbContext _db;
        private VentasService _ventasService;
        private PeriodoContableService _periodoContableService;

        public ComandasController(DbContext context)
        {
            _db = context;
            _ventasService = new VentasService(context);
            _periodoContableService = new PeriodoContableService(context);
        }


        public PartialViewResult ListaDeComandasPartial(int id)
        {
            var comandas = new List<Comanda>();
            if (User.IsInRole(RolesMontin.Vendedor))
            {
                var usuarioId = User.Identity.GetUserId();
                var vendedor = _ventasService.Vendedores().SingleOrDefault(v => v.UsuarioId == usuarioId);
                comandas = _db.Set<Comanda>().Where(v => v.DiaContableId == id && v.VendedorId == vendedor.Id && v.VentaId == null).ToList().OrderByDescending(v => v.Fecha).ToList();
            }
            else
            {
                comandas = _db.Set<Comanda>().Where(v => v.DiaContableId == id && v.VentaId == null).ToList().OrderByDescending(v => v.Fecha).ToList();
            }
            return PartialView("_ListaDeComandasPartial", comandas);
        }

        [DiaContable]
        public ActionResult Index()
        {
            var diaContable = _periodoContableService.GetDiaContableActual();
            ViewBag.DiaContable = diaContable;
            ViewBag.CantidadDeVentas = _ventasService.Ventas().Count(v => v.DiaContableId == diaContable.Id);
            if (User.IsInRole(RolesMontin.Vendedor))
            {
                var usuarioId = User.Identity.GetUserId();
                var vendedor = _ventasService.Vendedores().SingleOrDefault(v => v.UsuarioId == usuarioId);
                if (vendedor == null)
                {
                    TempData["error"] = "No existe vendedor asociado a la cuenta, consulte al administrador";
                }
            }
            return View();
        }

        public PartialViewResult DetalleDeComanda()
        {
            return PartialView("_DetalleDeComandaPartial");
        }


        [DiaContable]
        public ActionResult GenerarVenta(int id)
        {
            var comanda = _db.Set<Comanda>().Find(id);
            if (comanda.VentaId != null)
            {
                TempData["error"] = "Ya esta comanda genero una cuenta";
            }
            var venta = new Venta()
            {
                Fecha = DateTime.Now,
                DiaContableId = comanda.DiaContableId,
                EstadoDeVenta = EstadoDeVenta.Pendiente,
                PuntoDeVentaId = comanda.PuntoDeVentaId,
                CantidadPersonas = comanda.CantidadPersonas,
                UsuarioId = comanda.UsuarioId,
                VendedorId = comanda.VendedorId,
                Elaboraciones = comanda.Detalles.Select(d => new DetalleDeVenta()
                {
                    ElaboracionId = d.ElaboracionId,
                    Cantidad = d.Cantidad,
                    Agregados = d.Agregados.Select(a => new AgregadosVendidos() { AgregadoId = a.AgregadoId, Cantidad = a.Cantidad }).ToList(),
                    ImporteTotal = (d.Elaboracion.PrecioDeVenta * d.Cantidad) + (d.Agregados.Sum(a => a.Cantidad * a.Agregado.Precio) * d.Cantidad),
                    Costo = (d.Elaboracion.Costo * d.Cantidad) + (d.Agregados.Sum(a => a.Cantidad * a.Agregado.Costo) * d.Cantidad)
                }).ToList()

            };
            venta.Importe = venta.Elaboraciones.Sum(e => e.ImporteTotal);
            comanda.Venta = venta;
            if (_ventasService.Vender(venta, User.Identity.GetUserId()))
            {
                TempData["exito"] = "Venta generada correctamente";
            }
            else
            {
                TempData["error"] = "Error al generar comanda";
                return View("ProductosNecesarios", ProductosNecesarios(venta));
            }
            return RedirectToAction("Index", "Ventas");
        }

        private ICollection<ProductosNecesariosViewModel> ProductosNecesarios(Venta venta)
        {
            var result = new List<ProductosNecesariosViewModel>();
            foreach (var detalle in venta.Elaboraciones)
            {
                if (!_ventasService.SePuedeVender(detalle.ElaboracionId, (int)detalle.Cantidad, detalle.Elaboracion.CentroDeCostoId))
                {
                    foreach (var prod in detalle.Elaboracion.Productos)
                    {
                        result.Add(new ProductosNecesariosViewModel
                        {
                            Producto = prod.Producto.Nombre,
                            Cantidad = prod.Cantidad * detalle.Cantidad,
                            Unidad = prod.UnidadDeMedida.Siglas,
                            Menus = detalle.Elaboracion.Nombre
                        });
                    }
                    foreach (var agreg in detalle.Agregados)
                    {
                        result.Add(new ProductosNecesariosViewModel
                        {
                            Producto = agreg.Agregado.Producto.Nombre,
                            Cantidad = agreg.Cantidad * detalle.Cantidad * agreg.Agregado.Cantidad,
                            Unidad = agreg.Agregado.UnidadDeMedida.Siglas,
                            Menus = detalle.Elaboracion.Nombre
                        });
                    }
                }
            }
            var data = result.GroupBy(r => r.Producto).Select(g => new ProductosNecesariosViewModel()
            {
                Producto = g.Key,
                Cantidad = g.Sum(p => p.Cantidad),
                Unidad = g.FirstOrDefault().Unidad,
                Menus = String.Join(",",g.Select(r => r.Menus))

            }).ToList();
            return data;
        }



        [DiaContable]
        public ActionResult NuevaComanda()
        {
            ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre");
            ViewBag.VendedorId = new SelectList(_ventasService.Vendedores().Where(v => v.Estado == EstadoTrabajador.Activo), "Id", "NombreCompleto");
            return View();
        }


        [HttpPost]
        [DiaContable]
        public ActionResult NuevaComanda(Comanda comanda)
        {
            var usuarioId = User.Identity.GetUserId();
            if (User.IsInRole(RolesMontin.Vendedor) || User.IsInRole(RolesMontin.CapitanDeSalon))
            {

                var vendedor = _ventasService.Vendedores().SingleOrDefault(v => v.UsuarioId == usuarioId);
                if (vendedor == null)
                {
                    TempData["error"] = "No existe vendedor asociado a la cuenta, consulte al administrador";
                    return RedirectToAction("Index");
                }
                comanda.VendedorId = vendedor.Id;
            }
            comanda.UsuarioId = usuarioId;
            if (!comanda.Detalles.Any())
            {
                TempData["error"] = "No se puede crear una comanda vacia";
                ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre");
                ViewBag.VendedorId = new SelectList(_ventasService.Vendedores(), "Id", "NombreCompleto");
                return View();
            }
            var ordenes = new List<Orden>();
            foreach (var detalle in comanda.Detalles)
            {
                foreach (var orden in detalle.Ordenes)
                {
                    if (!ordenes.Any(o => o.Numero == orden.Orden.Numero))
                    {
                        ordenes.Add(new Orden() { Numero = orden.Orden.Numero, Comensal = orden.Orden.Comensal });
                    }
                    var ord = ordenes.SingleOrDefault(o => o.Numero == orden.Orden.Numero);
                    orden.Orden = ord;
                    var idAnotaciones = orden.Anotaciones.Select(a => a.Id).ToList();
                    orden.Anotaciones.Clear();
                    foreach (var id in idAnotaciones)
                    {
                        var anot = _db.Set<Anotacion>().Find(id);
                        orden.Anotaciones.Add(anot);
                    }
                }
            }
            for (int i = 1; i <= comanda.CantidadPersonas; i++)
            {
                if (!ordenes.Any(o => o.Numero == i))
                {
                    ordenes.Add(new Orden() { Numero = i, Comensal = Comensal.Hombre });
                }
            }
            comanda.Comensales = ordenes;

            comanda.DiaContableId = _periodoContableService.GetDiaContableActual().Id;
            comanda.Fecha = DateTime.Now;
            _db.Set<Comanda>().Add(comanda);
            try
            {
                _db.SaveChanges();
                TempData["exito"] = "Comanda agregada correctamente";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["error"] = "Error al crear la comanda";
            }

            ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre");
            ViewBag.VendedorId = new SelectList(_ventasService.Vendedores(), "Id", "NombreCompleto");
            return View();
        }


        public PartialViewResult MenusEnComandaPartial(int id)
        {
            return PartialView("_MenusEnComandaPartial", _db.Set<Comanda>().Find(id).Detalles);
        }

        [DiaContable]
        public ActionResult Editar(int id)
        {
            var comanda = _db.Set<Comanda>().Find(id);
            if (!User.IsInRole(RolesMontin.Administrador) && (comanda.EstadoDeVenta == EstadoDeVenta.Facturada || comanda.EstadoDeVenta == EstadoDeVenta.PagadaEnEfectivo || comanda.EstadoDeVenta == EstadoDeVenta.PagadaPorTarjeta))
            {
                TempData["error"] = "Usted no puede editar una comanda impresa o pagada";
                return RedirectToAction("Index");
            }

            ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre", comanda.PuntoDeVentaId);
            ViewBag.VendedorId = new SelectList(_ventasService.Vendedores(), "Id", "NombreCompleto", comanda.VendedorId);
            return View(comanda);
        }

        [HttpPost]
        [DiaContable]
        public ActionResult Editar(Comanda comanda)
        {
            if (!User.IsInRole(RolesMontin.Administrador) && (comanda.EstadoDeVenta == EstadoDeVenta.Facturada || comanda.EstadoDeVenta == EstadoDeVenta.PagadaEnEfectivo || comanda.EstadoDeVenta == EstadoDeVenta.PagadaPorTarjeta))
            {
                TempData["error"] = "Usted no puede editar una venta impresa o pagada";
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                var result = true;
                comanda.Comensales = null;
                comanda.Detalles = null;
                _db.Entry(comanda).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["exito"] = "Comanda editada correctamente";
                return RedirectToAction("Index");
            }
            ViewBag.PuntoDeVentaId = new SelectList(_ventasService.PuntosDeVentas(), "Id", "Nombre", comanda.PuntoDeVentaId);
            ViewBag.VendedorId = new SelectList(_ventasService.Vendedores(), "Id", "NombreCompleto", comanda.VendedorId);
            return View(comanda);
        }

        [HttpPost]
        [DiaContable]
        public ActionResult EliminarComanda(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var comanda = _db.Set<Comanda>().Find(id);
            if (!User.IsInRole(RolesMontin.Administrador) && comanda.VentaId != null)
            {
                TempData["error"] = "Usted no puede eliminar una comanda asosiada a una venta";
                return RedirectToAction("Index");
            }
            var detalles = comanda.Detalles.ToList();
            foreach (var detalle in detalles)
            {
                var ordenes = detalle.Ordenes.ToList();
                _db.Set<DetalleDeComanda>().Remove(detalle);
                foreach (var orden in ordenes)
                {
                    _db.Set<OrdenPorDetalle>().Remove(orden);
                }
            }

            var ords = comanda.Comensales.ToList();
            foreach (var orden in ords)
            {
                _db.Set<Orden>().Remove(orden);
            }

            _db.Set<Comanda>().Remove(comanda);
            _db.SaveChanges();
            TempData["exito"] = "Comanda eliminada correctamente";
            return RedirectToAction("Index");
        }



        public JsonResult DisminuirMenu(int id)
        {
            var detalle = _db.Set<DetalleDeComanda>().Find(id);
            detalle.Cantidad--;
            if (detalle.Cantidad < 0)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            _db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        public JsonResult DisminuirAgregado(int detalleId, int agregadoId)
        {
            var detalle = _db.Set<DetalleDeComanda>().Find(detalleId);
            var agregado =
                    detalle.Agregados.SingleOrDefault(
                        a => a.DetalleDeComandaId == detalleId && a.AgregadoId == agregadoId);
            agregado.Cantidad--;
            if (agregado.Cantidad == 0)
            {
                detalle.Agregados.Remove(agregado);
            }
            _db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AumentarAgregado(int detalleId, int agregadoId)
        {
            var detalle = _db.Set<DetalleDeComanda>().Find(detalleId);
            if (detalle.Agregados.Any(a => a.DetalleDeComandaId == detalleId && a.AgregadoId == agregadoId))
            {
                var agregado =
                    detalle.Agregados.SingleOrDefault(
                        a => a.DetalleDeComandaId == detalleId && a.AgregadoId == agregadoId);
                agregado.Cantidad++;
            }
            else
            {
                detalle.Agregados.Add(new AgregadoDeComanda() { AgregadoId = agregadoId, Cantidad = 1, DetalleDeComandaId = detalleId });
            }
            _db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AumentarMenu(int id)
        {
            var detalle = _db.Set<DetalleDeComanda>().Find(id);
            detalle.Cantidad++;
            _db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AgregarMenuAVenta(DetalleDeComanda detalle)
        {
            detalle.Elaboracion = null;
            var comanda = _db.Set<Comanda>().Find(detalle.ComandaId);
            comanda.Detalles.Add(detalle);
            _db.SaveChanges();
            return Json(new { Result = true, DetalleId = detalle.Id }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarDetalle(int id)
        {
            var detalle = _db.Set<DetalleDeComanda>().Find(id);
            _db.Set<DetalleDeComanda>().Remove(detalle);
            _db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarOrdenEnDetalle(int id)
        {
            var detalle = _db.Set<OrdenPorDetalle>().Find(id);
            _db.Set<OrdenPorDetalle>().Remove(detalle);
            _db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AgregarOrdenEnDetalle(int ordenId, int detalleId)
        {
            var detalle = _db.Set<DetalleDeComanda>().Find(detalleId);
            var ordenDetalle = new OrdenPorDetalle() { OrdenId = ordenId };
            detalle.Ordenes.Add(ordenDetalle);
            _db.SaveChanges();
            return Json(new { Result = true, Id = ordenDetalle.Id }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarAnotacion(int anotacionId, int ordenId)
        {
            var detalle = _db.Set<OrdenPorDetalle>().Find(ordenId);
            var anotacion = _db.Set<Anotacion>().Find(anotacionId);
            detalle.Anotaciones.Remove(anotacion);
            _db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AgregarAnotacion(int anotacionId, int ordenId)
        {
            var detalle = _db.Set<OrdenPorDetalle>().Find(ordenId);
            var anotacion = _db.Set<Anotacion>().Find(anotacionId);
            detalle.Anotaciones.Add(anotacion);
            _db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
    }
}