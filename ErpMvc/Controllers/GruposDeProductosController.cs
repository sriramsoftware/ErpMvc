using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using ComercialCore.Models;
using CompraVentaBL;
using ContabilidadCore.Models;
using ErpMvc.Models;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class GruposDeProductosController : Controller
    {
        private ProductoService _productoService;
        private DbContext _db;


        public GruposDeProductosController(DbContext context)
        {
            _productoService = new ProductoService(context);
            _db = context;
        }

        // GET: GruposDeProductos

        public ActionResult Listado()
        {
            return View(_productoService.GruposDeProductos().Include(g => g.Clasificacion).ToList());
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Agregar()
        {
            ViewBag.ClasificacionId = new SelectList(_productoService.ClasificacionesDeProductos(), "Id", "Descripcion");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Agregar(GrupoDeProducto grupoDeProducto)
        {
            if (ModelState.IsValid)
            {
                if (_productoService.AgregarGrupo(grupoDeProducto))
                {
                    TempData["exito"] = "Grupo de producto agregado correctamente!";
                    return RedirectToAction("Listado");
                }
            }
            ViewBag.ClasificacionId = new SelectList(_productoService.ClasificacionesDeProductos(), "Id", "Descripcion", grupoDeProducto.ClasificacionId);
            return View();
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var grupo = _productoService.GruposDeProductos().Find(id);
            if (grupo == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClasificacionId = new SelectList(_productoService.ClasificacionesDeProductos(), "Id", "Descripcion",grupo.ClasificacionId);
            return View(grupo);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(GrupoDeProducto grupoDeProducto)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(grupoDeProducto).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["exito"] = "Grupo de producto modificado correctamente!";
                return RedirectToAction("Listado");

            }
            ViewBag.ClasificacionId = new SelectList(_productoService.ClasificacionesDeProductos(), "Id", "Descripcion", grupoDeProducto.ClasificacionId);
            return View();
        }

        //[Authorize(Roles = RolesMontin.Administrador)]
        //public ActionResult Eliminar(int id)
        //{
        //    return PartialView("_EliminarTrabajadorPartial");
        //}

        //[Authorize(Roles = RolesMontin.Administrador)]
        //[HttpPost]
        //public ActionResult EliminarConfirmado(int id)
        //{
        //    return RedirectToAction("Index");
        //}

    }
}