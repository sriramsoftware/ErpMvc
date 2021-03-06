﻿using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompraVentaBL;
using CompraVentaCore.Models;
using System.Data.Entity;
using System.Net;
using ContabilidadCore.Models;
using ErpMvc.Models;
using ErpMvc.ViewModels;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class MenusController : Controller
    {
        private DbContext _db;
        private ElaboracionService _elaboracionService;
        private ProductoService _productoService;

        public MenusController(DbContext context)
        {
            _db = context;
            _elaboracionService = new ElaboracionService(context);
            _productoService = new ProductoService(context);
        }
        // GET: Menus
        public ActionResult Listado()
        {
            ViewBag.Porcientos = _db.Set<PorcientoMenu>().ToList();
            //ViewBag.ClasificacionId = new SelectList(_db.Set<Clasificacion>(), "Id", "Nombre");
            return View(_elaboracionService.Elaboraciones().Include(e => e.Agregados).Where(e => e.Activo).ToList());
        }

        public ActionResult BajoPrecioDeCosto()
        {
            ViewBag.Porcientos = _db.Set<PorcientoMenu>().ToList();
            return View("Listado",_elaboracionService.Elaboraciones().Include(e => e.Agregados).Where(e => e.Activo && e.Costo > e.PrecioDeVenta).ToList());
        }
        public ActionResult BajoPrecioConIndice()
        {
            ViewBag.Porcientos = _db.Set<PorcientoMenu>().ToList();
            return View("Listado",_elaboracionService.Elaboraciones().Include(e => e.Agregados).Where(e => e.Activo && e.Costo * e.IndiceEsperado > e.PrecioDeVenta).ToList());
        }

        public JsonResult Menu(int id)
        {
            var elaboracion = _elaboracionService.Elaboraciones().Find(id);
            var porCiento = _db.Set<PorcientoMenu>().Find(elaboracion.Id);
            var e = new 
            {
                Id = elaboracion.Id,
                Costo = elaboracion.Costo,
                Nombre = elaboracion.Nombre,
                PrecioDeVenta = elaboracion.PrecioDeVenta,
                CostoPlanificado = elaboracion.CostoPlanificado,
                IndiceEsperado = elaboracion.IndiceEsperado,
                Presentacion = elaboracion.Presentacion,
                Preparacion = elaboracion.Preparacion,
                PorCiento = porCiento.SeCalcula,
                CentroDeCostoId = elaboracion.CentroDeCostoId,
                ClasificacionId = elaboracion.ClasificacionId
            };
            return Json(e, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AgregadosData(int id)
        {
            var elaboracion = _elaboracionService.Elaboraciones().Find(id);
            var agregados = elaboracion.Agregados.Select(a => new {Id = a.Id, Nombre = a.Producto.Nombre, Costo = a.Costo, Precio = a.Precio, AgregadoId = a.Id}).ToList();
            return Json(agregados, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult AgregarMenu()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult AgregarMenu(Elaboracion elaboracion, bool? porCiento)
        {
            ModelState.Remove("Id");
            ModelState.Remove("PrecioDeVenta");
            ModelState.Remove("Costo");
            elaboracion.Activo = true;
            if (ModelState.IsValid)
            {
                _elaboracionService.AgregarElaboracion(elaboracion);
                _db.Set<PorcientoMenu>().Add(new PorcientoMenu() {ElaboracioId = elaboracion.Id,SeCalcula = (porCiento.HasValue?porCiento.Value:false)});
                _db.SaveChanges();
                return RedirectToAction("Listado");
            }
            return View(elaboracion);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult AgregarImagen(HttpPostedFileBase file, int id)
        {
            if (file != null)
            {
                file.SaveAs(Server.MapPath("~/Content/uploads/productos/" + id + file.FileName.Substring(file.FileName.LastIndexOf("."))));
            }
            return RedirectToAction("Listado");
        }

        public PartialViewResult AgregarMenuPartial()
        {
            ViewBag.CentroDeCostoId = new SelectList(_db.Set<CentroDeCosto>(), "Id", "Nombre");
            ViewBag.ClasificacionId = new SelectList(_db.Set<Clasificacion>(), "Id", "Nombre");
            return PartialView("_AgregarMenuPartial");
        }

        [HttpPost]
        public JsonResult ExisteMenu(string nombre)
        {
            var menu = _elaboracionService.Elaboraciones().FirstOrDefault(e => e.Nombre == nombre);
            return Json(menu, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult Editar(Elaboracion elaboracion,bool? porCiento)
        {
            if (ModelState.IsValid)
            {
                elaboracion.Activo = true;
                _elaboracionService.ModificarElaboracion(elaboracion);
                if (_db.Set<PorcientoMenu>().Any(p => p.ElaboracioId == elaboracion.Id))
                {
                    var porCientoMenu = _db.Set<PorcientoMenu>().SingleOrDefault(p => p.ElaboracioId == elaboracion.Id);
                    porCientoMenu.SeCalcula = (porCiento.HasValue ? porCiento.Value : false);
                    _db.SaveChanges();
                }
                else
                {
                    _db.Set<PorcientoMenu>().Add(new PorcientoMenu() {ElaboracioId = elaboracion.Id, SeCalcula = (porCiento.HasValue ? porCiento.Value : false) });
                    _db.SaveChanges();
                }
            }
            return RedirectToAction("Listado");
        }

        public ActionResult FichaDeCosto(int id)
        {
            var elab = _elaboracionService.Elaboraciones().Find(id);
            return View(elab);
        }

        public ActionResult Agregados(int id)
        {
            var elab = _elaboracionService.Elaboraciones().Find(id);
            return View(elab);
        }

        public PartialViewResult FichaDeCostoResumen(int id)
        {
            var elab = _elaboracionService.Elaboraciones().Include(e => e.Productos).SingleOrDefault(e => e.Id == id);
            var costos = new Dictionary<int, decimal>();
            foreach (var detalle in elab.Productos)
            {
                var costo = _productoService.GetPrecioUnitarioDeProducto(detalle.ProductoId, detalle.UnidadDeMedidaId);
                costos.Add(detalle.ProductoId, costo);
            }
            ViewBag.Costos = costos;
            return PartialView("_TablaFichaDeCostoPartial", elab);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.Administrador)]
        public JsonResult AgregarProductoAMenu(int menuId, int productoId, int unidadId, decimal cantidad)
        {
            _elaboracionService.AgregarProductoAElaboracion(menuId, productoId, cantidad, unidadId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.Administrador)]
        public JsonResult AgregarAgregadoAMenu(int menuId, int productoId, int unidadId, decimal cantidad, decimal precio)
        {
            _elaboracionService.AgregarAgregadoAElaboracion(menuId, productoId, cantidad, unidadId, precio);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProductosEnMenu(int menuId)
        {
            var elaboracion = _elaboracionService.Elaboraciones().Find(menuId);
            var productos = new List<dynamic>();
            foreach (var prod in elaboracion.Productos)
            {
                productos.Add(new { Producto = prod.Producto.Nombre, ProductoId = prod.ProductoId, Unidad = prod.UnidadDeMedida.Siglas, Cantidad = prod.Cantidad });
            }

            return Json(productos, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AgregadosEnMenu(int menuId)
        {
            var elaboracion = _elaboracionService.Elaboraciones().Find(menuId);
            var productos = new List<dynamic>();
            foreach (var prod in elaboracion.Agregados)
            {
                productos.Add(new { Producto = prod.Producto.Nombre, ProductoId = prod.ProductoId, Unidad = prod.UnidadDeMedida.Siglas, Cantidad = prod.Cantidad , Precio = prod.Precio});
            }

            return Json(productos, JsonRequestBehavior.AllowGet);
        }


        public JsonResult Menus()
        {
            var menus = _elaboracionService.Elaboraciones().Where(e => e.Activo && e.Productos.Any()).ToList();
            var menusLista = new List<MenuViewModel>();
            foreach (var menu in menus)
            {
                menusLista.Add(new MenuViewModel() { Id = menu.Id, Nombre = menu.Nombre, Precio = menu.PrecioDeVenta,CentroDeCostoId = menu.CentroDeCostoId });
            }

            return Json(menusLista.OrderBy(m => m.Nombre), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public JsonResult QuitarProductoDeMenu(int menuId, int productoId)
        {
            _elaboracionService.QuitarProductoDeElaboracion(menuId, productoId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public JsonResult QuitarAgregadoDeMenu(int menuId, int productoId)
        {
            _elaboracionService.QuitarAgregadoDeElaboracion(menuId, productoId);
            return Json(true, JsonRequestBehavior.AllowGet);
        }


        public JsonResult PrecioDeVentaDeMenu(int id)
        {
            var menu = _elaboracionService.Elaboraciones().Find(id);
            return Json(menu.PrecioDeVenta, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var menu = _elaboracionService.Elaboraciones().Find(id);
            if (menu == null)
            {
                return HttpNotFound();
            }
            return View(menu);
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        [HttpPost]
        [ActionName("Eliminar")]
        public ActionResult EliminarConfirmado(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (_elaboracionService.InactivarElaboracion(id.Value) > -1)
            {
                TempData["exito"] = "Menu eliminado correctamente";
                return RedirectToAction("Listado");
            }

            TempData["error"] = "No se pudo eliminar el menu.";
            return RedirectToAction("Listado");
        }

    }
}