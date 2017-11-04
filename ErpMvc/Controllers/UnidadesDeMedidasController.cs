using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ComercialCore.Models;
using CompraVentaBL;
using ErpMvc.Models;

namespace ErpMvc.Controllers
{
    [Authorize(Roles = RolesMontin.Administrador)]
    public class UnidadesDeMedidasController : Controller
    {
        private DbContext _db;
        private ProductoService _productoService;

        public UnidadesDeMedidasController(DbContext context)
        {
            _db = context;
            _productoService = new ProductoService(context);
        }

        // GET: UnidadesDeMedidas
        public ActionResult Listado()
        {
            return View(_db.Set<UnidadDeMedida>().Include(u => u.TipoDeUnidadDeMedida));
        }

        public ActionResult Agregar()
        {
            ViewBag.TipoDeUnidadDeMedidaId = new SelectList(_db.Set<TipoDeUnidadDeMedida>(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Agregar(UnidadDeMedida unidadDeMedida)
        {
            if (ModelState.IsValid)
            {
                _db.Set<UnidadDeMedida>().Add(unidadDeMedida);
                _db.SaveChanges();
                TempData["exito"] = "Unidad agregada correctamente!";
                return RedirectToAction("Listado");
            }
            ViewBag.TipoDeUnidadDeMedidaId = new SelectList(_db.Set<TipoDeUnidadDeMedida>(), "Id", "Name",unidadDeMedida.TipoDeUnidadDeMedidaId);
            return View(unidadDeMedida);
        }

        public ActionResult Editar(int id)
        {
            var unidad = _db.Set<UnidadDeMedida>().Find(id);
            ViewBag.TipoDeUnidadDeMedidaId = new SelectList(_db.Set<TipoDeUnidadDeMedida>(), "Id", "Name", unidad.TipoDeUnidadDeMedidaId);
            return View(unidad);
        }

        [HttpPost]
        public ActionResult Editar(UnidadDeMedida unidadDeMedida)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(unidadDeMedida).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["exito"] = "Unidad modificada correctamente!";
                return RedirectToAction("Listado");
            }
            ViewBag.TipoDeUnidadDeMedidaId = new SelectList(_db.Set<TipoDeUnidadDeMedida>(), "Id", "Name", unidadDeMedida.TipoDeUnidadDeMedidaId);
            return View(unidadDeMedida);
        }
    }
}