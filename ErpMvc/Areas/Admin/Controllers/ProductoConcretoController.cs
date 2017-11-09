using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlmacenCore.Models;
using ErpMvc.Models;

namespace ErpMvc.Areas.Admin.Controllers
{
    [Authorize(Roles = RolesMontin.Administrador)]
    public class ProductoConcretoController : Controller
    {
        private DbContext _db;

        public ProductoConcretoController(DbContext context)
        {
            _db = context;
        }

        // GET: Admin/ExistenciaAlmacen
        public ActionResult Index()
        {
            return View(_db.Set<ProductoConcreto>().ToList());
        }

        
        // GET: Admin/ExistenciaAlmacen/Edit/5
        public ActionResult Edit(int id)
        {
            var exist = _db.Set<ExistenciaAlmacen>().Find(id);
            return View(exist);
        }

        // POST: Admin/ExistenciaAlmacen/Edit/5
        [HttpPost]
        public ActionResult Edit(ExistenciaAlmacen existenciaAlmacen)
        {
            try
            {
                var exist = _db.Set<ExistenciaAlmacen>().Find(existenciaAlmacen.Id);
                exist.ExistenciaEnAlmacen = existenciaAlmacen.ExistenciaEnAlmacen;
                _db.Entry(exist).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["exito"] = "Existencia modificada correctamente";
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
