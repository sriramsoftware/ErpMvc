using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContabilidadBL;
using ContabilidadCore.Models;
using ErpMvc.Models;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class OtrosGastosController : Controller
    {
        private DbContext _db;
        private PeriodoContableService _periodoContableService;

        public OtrosGastosController(DbContext context)
        {
            _db = context;
            _periodoContableService = new PeriodoContableService(context);
        }

        // GET: OtrosGastos
        public ActionResult Index()
        {
            return View(_db.Set<OtrosGastos>().ToList());
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Agregar()
        {
            ViewBag.ConceptoDeGastoId = new SelectList(_db.Set<ConceptoDeGasto>(), "Id", "Nombre");
            ViewBag.DiaContable = _periodoContableService.GetDiaContableActual();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Agregar(OtrosGastos otrosGastos)
        {
            otrosGastos.DiaContableId = _periodoContableService.GetDiaContableActual().Id;
            if (ModelState.IsValid)
            {
                _db.Set<OtrosGastos>().Add(otrosGastos);
                _db.SaveChanges();
                TempData["exito"] = "Gasto agregado correctamente";
                return RedirectToAction("Index");
            }
            ViewBag.ConceptoDeGastoId = new SelectList(_db.Set<ConceptoDeGasto>(), "Id", "Nombre", otrosGastos.ConceptoDeGastoId);
            return View(otrosGastos);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(int id)
        {
            var gasto = _db.Set<OtrosGastos>().Find(id);
            ViewBag.ConceptoDeGastoId = new SelectList(_db.Set<ConceptoDeGasto>(), "Id", "Nombre", gasto.ConceptoDeGastoId);
            return View(gasto);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(OtrosGastos otrosGastos)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(otrosGastos).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["exito"] = "Gasto modificado correctamente";
                return RedirectToAction("Index");
            }
            ViewBag.ConceptoDeGastoId = new SelectList(_db.Set<ConceptoDeGasto>(), "Id", "Nombre", otrosGastos.ConceptoDeGastoId);
            return View(otrosGastos);
        }
        
        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult Eliminar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var gasto = _db.Set<OtrosGastos>().Find(id);
            if (gasto == null)
            {
                return new HttpNotFoundResult();
            }
            return View(gasto);
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
            var gasto = _db.Set<OtrosGastos>().Find(id);
            if (gasto == null)
            {
                return new HttpNotFoundResult();
            }
            _db.Set<OtrosGastos>().Remove(gasto);
            _db.SaveChanges();
            TempData["exito"] = "Gasto eliminado correctamente";
            return RedirectToAction("Index");
        }

    }
}