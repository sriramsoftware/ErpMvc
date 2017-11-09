using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlmacenCore.Models;
using ErpMvc.Areas.Admin.Models;
using ErpMvc.Models;

namespace ErpMvc.Areas.Admin.Controllers
{
    [Authorize(Roles = RolesMontin.Administrador)]
    public class ExistenciaCentroDeCostoController : Controller
    {
        private DbContext _db;

        public ExistenciaCentroDeCostoController(DbContext context)
        {
            _db = context;
        }

        // GET: Admin/ExistenciaAlmacen
        public ActionResult Index()
        {
            return View(_db.Set<ExistenciaCentroDeCosto>().ToList());
        }

        
        // GET: Admin/ExistenciaAlmacen/Edit/5
        public ActionResult Edit(int id, int centroId)
        {
            var exist = _db.Set<ExistenciaCentroDeCosto>().Find(id,centroId);
            return View(exist);
        }

        // POST: Admin/ExistenciaAlmacen/Edit/5
        [HttpPost]
        public ActionResult Edit(ExistenciaCentroDeCosto existenciaCentroDeCosto)
        {
            try
            {
                var exist = _db.Set<ExistenciaCentroDeCosto>().Find(existenciaCentroDeCosto.ProductoId,existenciaCentroDeCosto.CentroDeCostoId);
                exist.Cantidad = existenciaCentroDeCosto.Cantidad;
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

        [HttpPost]
        public ActionResult Borrar(int id, int centroId)
        {
            var existencia = _db.Set<ExistenciaCentroDeCosto>().Find(id,centroId);
            _db.Set<ExistenciaCentroDeCosto>().Remove(existencia);
            _db.SaveChanges();
            TempData["exito"] = "Existencia modificada correctamente";
            return RedirectToAction("Index");
        }
    }
}
