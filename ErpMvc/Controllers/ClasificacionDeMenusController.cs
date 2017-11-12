using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;
using ComercialCore.Models;
using CompraVentaBL;
using CompraVentaCore.Models;
using ContabilidadCore.Models;
using ErpMvc.Models;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class ClasificacionDeMenusController : Controller
    {
        private DbContext _db;


        public ClasificacionDeMenusController(DbContext context)
        {
            _db = context;
        }

        // GET: GruposDeProductos

        public ActionResult Listado()
        {
            return View(_db.Set<Clasificacion>().ToList());
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Agregar(Clasificacion clasificacion)
        {
            if (ModelState.IsValid)
            {
                _db.Set<Clasificacion>().Add(clasificacion);
                _db.SaveChanges();
                TempData["exito"] = "Clasificacion de menu agregada correctamente!";
                return RedirectToAction("Listado");
            }
            return View();
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var clasificacion = _db.Set<Clasificacion>().Find(id);
            if (clasificacion == null)
            {
                return HttpNotFound();
            }
            return View(clasificacion);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(Clasificacion clasificacion)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(clasificacion).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["exito"] = "Clasificacion de menu modificada correctamente!";
                return RedirectToAction("Listado");
            }
            return View();
        }
    }
}