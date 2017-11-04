using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompraVentaBL;
using CompraVentaCore.Models;
using ContabilidadCore.Models;
using ErpMvc.Models;

namespace ErpMvc.Controllers
{
    [Authorize(Roles = RolesMontin.Administrador)]
    public class PuntoDeVentaController : Controller
    {
        private DbContext _db;

        public PuntoDeVentaController(DbContext context)
        {
            _db = context;
        }


        // GET: Puntos de ventas
        public ActionResult Index()
        {
            return View(_db.Set<PuntoDeVenta>().ToList());
        }

        public ActionResult Agregar()
        {
            ViewBag.CentroDeCostoId = new SelectList(_db.Set<CentroDeCosto>(),"Id","Nombre");
            return View();
        }

        [HttpPost]
        public ActionResult Agregar(PuntoDeVenta puntoDeVenta)
        {
            if (ModelState.IsValid)
            {
                _db.Set<PuntoDeVenta>().Add(puntoDeVenta);
                _db.SaveChanges();
                TempData["exito"] = "Punto de venta agregado correctamente";
                return RedirectToAction("Index");
            }
            ViewBag.CentroDeCostoId = new SelectList(_db.Set<PuntoDeVenta>(), "Id", "Nombre", puntoDeVenta.CentroDeCostoId);
            return View(puntoDeVenta);
        }

        public ActionResult Editar(int id)
        {
            var punto = _db.Set<PuntoDeVenta>().Find(id);
            ViewBag.CentroDeCostoId = new SelectList(_db.Set<CentroDeCosto>(), "Id", "Nombre", punto.CentroDeCostoId);
            return View(punto);
        }

        [HttpPost]
        public ActionResult Editar(PuntoDeVenta puntoDeVenta)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(puntoDeVenta).State = EntityState.Modified;
                _db.SaveChanges();
                TempData["exito"] = "Punto de venta modificado correctamente";
                return RedirectToAction("Index");
            }
            ViewBag.CentroDeCostoId = new SelectList(_db.Set<CentroDeCosto>(), "Id", "Nombre", puntoDeVenta.CentroDeCostoId);
            return View(puntoDeVenta);
        }

    }
}