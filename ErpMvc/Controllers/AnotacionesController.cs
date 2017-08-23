using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ContabilidadBL;
using ContabilidadCore.Models;
using ErpMvc.Models;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class AnotacionesController : Controller
    {
        private DbContext _db;
        private PeriodoContableService _periodoContableService;

        public AnotacionesController(DbContext context)
        {
            _db = context;
            _periodoContableService = new PeriodoContableService(context);
        }

        // GET: OtrosGastos
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Index()
        {
            return View(_db.Set<Anotacion>());
        }

        public JsonResult Data()
        {
            var anotaciones = _db.Set<Anotacion>();
            return Json(anotaciones.Select(a => new {Id = a.Id, Abreviatura = a.Abreviatura, Descripcion = a.Descripcion}).ToList(), JsonRequestBehavior.AllowGet);
        }

    [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Agregar(Anotacion anotacion)
        {
            if (ModelState.IsValid)
            {
                _db.Set<Anotacion>().Add(anotacion);
                _db.SaveChanges();
                TempData["exito"] = "Anotacion agregada correctamente";
                return RedirectToAction("Index");
            }
            return View(anotacion);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(int id)
        {
            var anotacion = _db.Set<Anotacion>().Find(id);
            return View(anotacion);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(Anotacion anotacion)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(anotacion).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["exito"] = "Anotacion modificada correctamente";
                return RedirectToAction("Index");
            }
            return View(anotacion);
        }

        
        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult Eliminar(int id)
        {
            var anotacion = _db.Set<Anotacion>().Find(id);
            if (anotacion == null)
            {
                return new HttpNotFoundResult();
            }
            return View(anotacion);
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        [HttpPost]
        public ActionResult EliminarConfirmado(int id)
        {
            var anotacion = _db.Set<Anotacion>().Find(id);
            if (anotacion == null)
            {
                return new HttpNotFoundResult();
            }
            _db.Set<Anotacion>().Remove(anotacion);
            _db.SaveChanges();
            TempData["exito"] = "Anotacion eliminada correctamente";
            return RedirectToAction("Index");
        }

    }
}