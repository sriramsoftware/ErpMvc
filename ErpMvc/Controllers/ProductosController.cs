using System;
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
using ErpMvc.Models;
using ErpMvc.ViewModels;
using Microsoft.AspNet.Identity;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class ProductosController : Controller
    {
        private ProductoService _service;
        private DbContext _db;

        public ProductosController(DbContext context)
        {
            _service = new ProductoService(context);
            _db = context;
        }

        public ActionResult Listado()
        {
            var productos = new List<ProductoConcretoViewModel>();
            foreach (var prod in _db.Set<ProductoConcreto>().Where(p => p.Producto.Activo).ToList())
            {
                if (_db.Set<ExistenciaCentroDeCosto>().Any(e => e.ProductoId == prod.Id))
                {
                    productos.Add(new ProductoConcretoViewModel() {Producto = prod, Existencia = _db.Set<ExistenciaCentroDeCosto>().SingleOrDefault(e => e.ProductoId == prod.Id) });
                }
                else
                {
                    productos.Add(new ProductoConcretoViewModel()
                    {
                        Producto = prod,
                        Existencia = new ExistenciaCentroDeCosto() { Cantidad = 0, Producto = prod, ProductoId = prod.Id}
                    });
                }
            }
            //productos.AddRange(_db.Set<ProductoConcreto>().Where(p=> p.));
            return View(productos);
        }

        public ActionResult Historial(int? id)
        {
            var producto = _db.Set<ProductoConcreto>().Find(id);
            var movimientos = _db.Set<MovimientoDeProducto>().Include(m => m.Tipo).Include(m => m.Usuario).Include(m => m.Producto).Where(m => m.ProductoId == id).ToList();
            var existencia = _db.Set<ExistenciaCentroDeCosto>().SingleOrDefault(e => e.ProductoId == id);
            if (existencia == null)
            {
                existencia = new ExistenciaCentroDeCosto() {Cantidad = 0};
            }
            var viewModel = new ProductoConcretoViewModel()
            {
                Producto = producto,
                Movimientos = movimientos,
                Existencia = existencia
            };
            return View(viewModel);
        }

        // GET: Productos
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Agregar()
        {
            ViewBag.GrupoId = new SelectList(_service.GruposDeProductos(),"Id", "Descripcion");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Agregar(Producto producto)
        {
            if (ModelState.IsValid)
            {
                if (_service.AgregarProducto(producto))
                {
                    TempData["exito"] = "Producto agregado correctamente!";
                    return RedirectToAction("Listado"); 
                }
            }
            ViewBag.GrupoId = new SelectList(_service.GruposDeProductos(), "Id", "Descripcion",producto.GrupoId);
            return View(producto);
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
            ViewBag.GrupoId = new SelectList(_service.GruposDeProductos(), "Id", "Descripcion",producto.Producto.GrupoId);
            ViewBag.UnidadDeMedidaId = new SelectList(_service.ListaDeUnidadesDeMedida(), "Id", "Nombre",producto.UnidadDeMedidaId);
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
                productoConcreto.Cantidad = productoViewModel.Cantidad;
                productoConcreto.UnidadDeMedidaId = productoViewModel.UnidadDeMedidaId;
                producto.Descripcion = productoViewModel.Descripcion;
                producto.EsInventariable = productoViewModel.EsInventariable;
                _db.Entry(producto).State = EntityState.Modified;
                _db.Entry(productoConcreto).State = EntityState.Modified;
                _db.SaveChanges();
                    return RedirectToAction("Listado");
                
            }
            ViewBag.GrupoId = new SelectList(_service.GruposDeProductos(), "Id", "Descripcion", productoViewModel.GrupoId);
            ViewBag.UnidadDeMedidaId = new SelectList(_service.ListaDeUnidadesDeMedida(), "Id", "Nombre", productoViewModel.UnidadDeMedidaId);
            return View(productoViewModel);
        }

        public JsonResult ListaProductos()
        {
            var productos = _service.Productos().Where(p => p.Activo).Select(c => new { c.Id, c.Nombre });
            return Json(productos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListaUnidadesDeMedida(int id)
        {
            var unidades = _service.UnidadesDeMismoTipoALaDeProducto(id).Select(c => new { c.Id, c.Nombre });
            return Json(unidades, JsonRequestBehavior.AllowGet);
        }

        public PartialViewResult AgregarProductoPartial()
        {
            ViewBag.GrupoId = new SelectList(_service.GruposDeProductos(), "Id", "Descripcion");
            ViewBag.UnidadDeMedidaId = new SelectList(_service.ListaDeUnidadesDeMedida(), "Id", "Nombre");
            return PartialView("_AgregarProductoPartial");
        }

        [HttpPost]
        public ActionResult AgregarProductoPartial(ProductoViewModel productoViewModel)
        {
            if (ModelState.IsValid)
            {
                var producto = new Producto() {Activo = true, Nombre = productoViewModel.Nombre, GrupoId = productoViewModel.GrupoId, EsInventariable = productoViewModel.EsInventariable};
                var unidad = _service.ListaDeUnidadesDeMedida().Find(productoViewModel.UnidadDeMedidaId);
                _service.AgregarProducto(producto, unidad, productoViewModel.PrecioUnitario,
                    productoViewModel.Cantidad);
                return RedirectToAction("Listado");
            }
            ViewBag.GrupoId = new SelectList(_service.GruposDeProductos(), "Id", "Descripcion");
            ViewBag.UnidadDeMedidaId = new SelectList(_service.ListaDeUnidadesDeMedida(), "Id", "Nombre");
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
            _service.EliminarProducto(id.Value);
            return RedirectToAction("Listado");
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult TramitarSalida()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult TramitarSalida(Compra compra)
        {
            var usuario = User.Identity.GetUserId();
            compra.UsuarioId = usuario;
            if (ModelState.IsValid)
            {
                //if (_comprasService.ComprarYPasarACentroDeCosto(compra, usuario))
                //{

                    return RedirectToAction("Index", "Inicio");
                //}
            }
            return View(compra);
        }

    }
}