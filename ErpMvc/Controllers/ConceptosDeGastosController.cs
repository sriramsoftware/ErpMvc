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
    public class ConceptosDeGastosController : Controller
    {
        private DbContext _db;
        private PeriodoContableService _periodoContableService;

        public ConceptosDeGastosController(DbContext context)
        {
            _db = context;
            _periodoContableService = new PeriodoContableService(context);
        }

        // GET: OtrosGastos
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Index()
        {
            return View(_db.Set<ConceptoDeGasto>());
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Agregar(ConceptoDeGasto conceptoDeGasto)
        {
            if (ModelState.IsValid)
            {
                _db.Set<ConceptoDeGasto>().Add(conceptoDeGasto);
                _db.SaveChanges();
                TempData["exito"] = "Concepto agregado correctamente";
                return RedirectToAction("Index");
            }
            return View(conceptoDeGasto);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(int id)
        {
            var concepto = _db.Set<ConceptoDeGasto>().Find(id);
            return View(concepto);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(ConceptoDeGasto conceptoDeGasto)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(conceptoDeGasto).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["exito"] = "Concepto modificado correctamente";
                return RedirectToAction("Index");
            }
            return View(conceptoDeGasto);
        }

        
        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult Eliminar(int id)
        {
            return PartialView("_EliminarTrabajadorPartial");
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        [HttpPost]
        public ActionResult EliminarConfirmado(int id)
        {
            //var trabajador = _vendedorService.Vendedores().Find(id);
            //if (trabajador == null)
            //{
            //    return new HttpNotFoundResult();
            //}
            //trabajador.Estado = EstadoTrabajador.Baja;
            //if (trabajador.Usuario != null)
            //{
            //    trabajador.Usuario.Activo = false;
            //}
            //_vendedorService.ModificarVendedor(trabajador);
            //TempData["exito"] = "Trabajador eliminado correctamente";
            return RedirectToAction("Index");
        }

    }
}