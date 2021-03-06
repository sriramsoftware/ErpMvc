﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ComercialCore.Models;
using CompraVentaBL;
using System.Data.Entity;
using System.Net;
using AlmacenCore.Models;
using CompraVentaCore.Models;
using ContabilidadCore.Models;
using ErpMvc.Models;
using ErpMvc.Utiles;
using ErpMvc.ViewModels;
using Microsoft.AspNet.Identity;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private ProductoService _service;
        private CentroDeCostoService _centroCostoService;
        private DbContext _db;

        public ProductosController(DbContext context)
        {
            _service = new ProductoService(context);
            _centroCostoService = new CentroDeCostoService(context);
            _db = context;
        }

        public ActionResult Listado()
        {
            var productos = new List<ProductoConcretoViewModel>();
            foreach (var prod in _db.Set<ProductoConcreto>().Where(p => p.Producto.Activo).ToList())
            {
                var prodConcreto = new ProductoConcretoViewModel() { Producto = prod };
                if (_db.Set<ExistenciaCentroDeCosto>().Any(e => e.ProductoId == prod.Id))
                {
                    prodConcreto.Existencias =
                        _db.Set<ExistenciaCentroDeCosto>()
                            .Include(e => e.CentroDeCosto)
                            .Where(e => e.ProductoId == prod.Id)
                            .Select(e => new ExistenciaViewModel()
                            {
                                Lugar = e.CentroDeCosto.Nombre,
                                Cantidad = e.Cantidad
                            }).ToList();
                }
                if (_db.Set<ExistenciaAlmacen>().Any(e => e.ProductoId == prod.Id))
                {
                    var existencias = _db.Set<ExistenciaAlmacen>()
                            .Include(e => e.Almacen)
                            .Where(e => e.ProductoId == prod.Id)
                            .Select(e => new ExistenciaViewModel()
                            {
                                Lugar = e.Almacen.Descripcion,
                                Cantidad = e.ExistenciaEnAlmacen
                            }).ToList();
                    foreach (var ex in existencias)
                    {
                        prodConcreto.Existencias.Add(ex);
                    }
                }
                productos.Add(prodConcreto);
            }
            return View(productos);
        }

        public ActionResult Historial(int? id)
        {
            var producto = _db.Set<ProductoConcreto>().Find(id);
            var entradasAlmacen = _db.Set<EntradaAlmacen>().Where(e => e.ProductoId == id);
            var salidasDeAlmacen = _db.Set<DetalleSalidaAlmacen>().Where(d => d.Producto.ProductoId == id);
            var mermasDeAlmacen = _db.Set<SalidaPorMerma>().Where(d => d.ExistenciaAlmacen.Producto.ProductoId == id);
            var movimientos = _db.Set<MovimientoDeProducto>().Include(m => m.Tipo).Include(m => m.Usuario).Include(m => m.Producto).Where(m => m.ProductoId == id).ToList();

            var resumenMov = new List<DetalleMovimientoProductoViewModel>();

            resumenMov.AddRange(salidasDeAlmacen.Select(s => new DetalleMovimientoProductoViewModel()
            {
                Fecha = s.Vale.DiaContable.Fecha,
                Cantidad = s.Cantidad,
                Usuario = s.Vale.Usuario.UserName,
                Lugar = "Almacen",
                TipoDeMovimiento = "Salida de almacen",
                Unidad = s.Producto.Producto.UnidadDeMedida.Siglas,
                Detalle = "entrada a " + s.Vale.CentroDeCosto.Nombre
            }));

            resumenMov.AddRange(entradasAlmacen.Select(e => new DetalleMovimientoProductoViewModel()
            {
                Fecha = e.DiaContable.Fecha,
                Cantidad = e.Cantidad,
                Usuario = e.Usuario.UserName,
                Lugar = "Almacen",
                TipoDeMovimiento = "Entrada",
                Unidad = e.Producto.UnidadDeMedida.Siglas,
                Detalle = ""
            }));

            resumenMov.AddRange(movimientos.Select(s => new DetalleMovimientoProductoViewModel()
            {
                Fecha = s.Fecha,
                Cantidad = s.Cantidad,
                Usuario = s.Usuario.UserName,
                Lugar = s.CentroDeCosto.Nombre,
                TipoDeMovimiento = s.Tipo.Descripcion == TipoDeMovimientoConstantes.SalidaAProduccion?"Venta":s.Tipo.Descripcion,
                Unidad = s.Producto.UnidadDeMedida.Siglas,
                Detalle = ""
            }));

            resumenMov.AddRange(mermasDeAlmacen.Select(s => new DetalleMovimientoProductoViewModel()
            {
                Fecha = s.Fecha,
                Cantidad = s.Cantidad,
                Usuario = s.Usuario.UserName,
                Lugar = "Almacen",
                TipoDeMovimiento = "Merma",
                Unidad = s.UnidadDeMedida.Siglas,
                Detalle = ""
            }));

            var existencia = _db.Set<ExistenciaCentroDeCosto>().Where(e => e.ProductoId == id).Select(e => new ExistenciaViewModel() {Lugar = e.CentroDeCosto.Nombre, Cantidad = e.Cantidad}).ToList();
            existencia.Add(new ExistenciaViewModel() {Lugar = "Almacen", Cantidad = _db.Set<ExistenciaAlmacen>().Any(a => a.ProductoId == id)? _db.Set<ExistenciaAlmacen>().Where(a => a.ProductoId == id).Sum(a => a.ExistenciaEnAlmacen):0});
            
            var viewModel = new ProductoConcretoViewModel()
            {
                Producto = producto,
                Movimientos = resumenMov,
                Existencias = existencia
            };
            return View(viewModel);
        }
        
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var producto = _db.Set<ProductoConcreto>().Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            ViewBag.GrupoId = new SelectList(_db.Set<GrupoDeProducto>(), "Id", "Descripcion", producto.Producto.GrupoId);
            ViewBag.UnidadDeMedida = producto.UnidadDeMedida.Nombre;
            //ViewBag.UnidadDeMedidaId = new SelectList(_service.ListaDeUnidadesDeMedida(), "Id", "Nombre", producto.UnidadDeMedidaId);
            return View(new ProductoViewModel(producto));
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(ProductoViewModel productoViewModel)
        {
            if (ModelState.IsValid)
            {
                var producto = _db.Set<Producto>().Find(productoViewModel.ProductoId);
                var productoConcreto = _db.Set<ProductoConcreto>().Find(productoViewModel.ProductoConcretoId);
                producto.Nombre = productoViewModel.Nombre;
                producto.GrupoId = productoViewModel.GrupoId;
                //productoConcreto.Cantidad = productoViewModel.Cantidad;
                //productoConcreto.UnidadDeMedidaId = productoViewModel.UnidadDeMedidaId;
                productoConcreto.PrecioDeVenta = productoViewModel.PrecioUnitario;
                productoConcreto.ProporcionDeMerma = productoViewModel.ProporcionDeMerma;
                producto.Descripcion = productoViewModel.Descripcion;
                producto.EsInventariable = productoViewModel.EsInventariable;
                _db.Entry(producto).State = EntityState.Modified;
                _db.Entry(productoConcreto).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Listado");

            }
            ViewBag.GrupoId = new SelectList(_db.Set<GrupoDeProducto>(), "Id", "Descripcion", productoViewModel.GrupoId);
            ViewBag.UnidadDeMedidaId = new SelectList(_db.Set<UnidadDeMedida>(), "Id", "Nombre", productoViewModel.UnidadDeMedidaId);
            return View(productoViewModel);
        }

        public JsonResult ListaProductos()
        {
            var productos = _service.Productos().Where(p => p.Activo).Select(c => new { c.Id, c.Nombre });
            return Json(productos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaProductosConcretos()
        {
            var productos = _db.Set<ProductoConcreto>().Where(p => p.Producto.Activo).Select(c => new { c.Id, c.Producto.Nombre, ProductoId = c.Producto.Id, Unidad = c.UnidadDeMedida.Siglas });
            return Json(productos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaUnidadesDeMedida(int id)
        {
            var unidades = _service.UnidadesDeMismoTipoALaDeProducto(id).Select(c => new { c.Id, c.Siglas });
            return Json(unidades, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult AgregarProductoPartial()
        {
            ViewBag.GrupoId = new SelectList(_db.Set<GrupoDeProducto>(), "Id", "Descripcion");
            ViewBag.UnidadDeMedidaId = new SelectList(_db.Set<UnidadDeMedida>(), "Id", "Nombre");
            return PartialView("_AgregarProductoPartial",new ProductoViewModel() {EsInventariable = true});
        }

        [HttpPost]
        public ActionResult AgregarProductoPartial(ProductoViewModel productoViewModel)
        {
            if (ModelState.IsValid)
            {
                var producto = new Producto() { Activo = true, Nombre = productoViewModel.Nombre, GrupoId = productoViewModel.GrupoId, EsInventariable = productoViewModel.EsInventariable };
                var unidad = _db.Set<UnidadDeMedida>().Find(productoViewModel.UnidadDeMedidaId);
                _service.AgregarProducto(producto, unidad, productoViewModel.PrecioUnitario,
                    productoViewModel.Cantidad);
                return RedirectToAction("Listado");
            }
            ViewBag.GrupoId = new SelectList(_db.Set<GrupoDeProducto>(), "Id", "Descripcion");
            ViewBag.UnidadDeMedidaId = new SelectList(_db.Set<UnidadDeMedida>(), "Id", "Nombre");
            return PartialView("_AgregarProductoPartial");
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var producto = _service.Productos().Find(id);
            if (producto == null)
            {
                return HttpNotFound();
            }
            if (_db.Set<ExistenciaCentroDeCosto>().Any(e => e.Producto.ProductoId == producto.Id))
            {
                var existencia = _db.Set<ExistenciaCentroDeCosto>().SingleOrDefault(e => e.Producto.ProductoId == producto.Id);
                if (existencia.Cantidad > 0)
                {
                    TempData["error"] = "No se puede eliminar un producto con existencia";
                    return RedirectToAction("Listado");
                }
                if (_db.Set<Elaboracion>().Any(e => e.Activo && (e.Productos.Any(p => p.ProductoId == id) || e.Agregados.Any(p => p.ProductoId == id))))
                {
                    TempData["error"] = "No se puede eliminar este producto porque es usado en un menu";
                    return RedirectToAction("Listado");
                }
            }
            if (_db.Set<Elaboracion>().Any(e => e.Activo && (e.Productos.Any(p => p.ProductoId == id) || e.Agregados.Any(p => p.ProductoId == id))))
            {
                TempData["error"] = "No se puede eliminar este producto porque es usado en un menu";
                return RedirectToAction("Listado");
            }
            return View(producto);
        }

        [HttpPost]
        [ActionName("Eliminar")]
        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult EliminarConfirmado(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            _service.DesactivarProducto(id.Value);
            return RedirectToAction("Listado");
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        [DiaContable]
        public ActionResult TramitarSalida()
        {
            ViewBag.CentroDeCostoId = new SelectList(_centroCostoService.CentrosDeCosto(),"Id","Nombre");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        [DiaContable]
        public ActionResult TramitarSalida(Compra compra,int centroDeCostoId)
        {
            var usuario = User.Identity.GetUserId();
            compra.UsuarioId = usuario;
            if (ModelState.IsValid)
            {
                if (!compra.Productos.Any())
                {
                    TempData["error"] = "No se puede efectuar una merma vacia";
                    return View();
                }
                foreach (var prod in compra.Productos)
                {
                    _centroCostoService.DarSalidaPorMerma(prod.ProductoId, centroDeCostoId, prod.Cantidad, prod.UnidadDeMedidaId, User.Identity.GetUserId(),compra.Descripcion);
                }
                _db.SaveChanges();
                TempData["exito"] = "Salida registrada correctamente";
                return RedirectToAction("CentroDeCosto", "Inventario");
            }
            ViewBag.CentroDeCostoId = new SelectList(_centroCostoService.CentrosDeCosto(), "Id", "Nombre",centroDeCostoId);
            return View(compra);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        [DiaContable]
        public ActionResult EntradaPorAjuste()
        {
            ViewBag.CentroDeCostoId = new SelectList(_centroCostoService.CentrosDeCosto(), "Id", "Nombre");
            return View();
        }


        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        [DiaContable]
        public ActionResult EntradaPorAjuste(Compra compra, int centroDeCostoId)
        {
            var usuario = User.Identity.GetUserId();
            compra.UsuarioId = usuario;
            if (ModelState.IsValid)
            {
                if (!compra.Productos.Any())
                {
                    TempData["error"] = "No se puede efectuar una entrada vacia";
                    return View();
                }
                var tipoDeMovimiento =
                    _db.Set<TipoDeMovimiento>()
                        .SingleOrDefault(t => t.Descripcion == TipoDeMovimientoConstantes.EntradaPorAjuste);
                foreach (var prod in compra.Productos)
                {
                    _centroCostoService.DarEntrada(prod.ProductoId, centroDeCostoId,tipoDeMovimiento.Id, prod.Cantidad, prod.UnidadDeMedidaId, User.Identity.GetUserId());
                }
                _db.SaveChanges();
                TempData["exito"] = "Salida registrada correctamente";
                return RedirectToAction("CentroDeCosto", "Inventario");
            }
            ViewBag.CentroDeCostoId = new SelectList(_centroCostoService.CentrosDeCosto(), "Id", "Nombre", centroDeCostoId);
            return View(compra);
        }
        
        public JsonResult SePuedeDarSalida(int productoId, decimal cantidad, int unidadId)
        {
            var centroCostoId = _centroCostoService.CentrosDeCosto().FirstOrDefault().Id;
            var productoConcretoId = _db.Set<ProductoConcreto>().SingleOrDefault(p => p.ProductoId == productoId).Id;
            var result = _centroCostoService.PuedeDarSalida(productoConcretoId, centroCostoId, cantidad, unidadId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}