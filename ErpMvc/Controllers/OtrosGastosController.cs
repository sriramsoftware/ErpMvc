﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ContabilidadBL;
using ContabilidadCore.Models;
using ErpMvc.Models;
using Microsoft.AspNet.Identity;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class OtrosGastosController : Controller
    {
        private DbContext _db;
        private PeriodoContableService _periodoContableService;
        private SubmayorService _submayorService;
        private CuentasServices _cuentasServices;

        public OtrosGastosController(DbContext context)
        {
            _db = context;
            _periodoContableService = new PeriodoContableService(context);
            _submayorService = new SubmayorService(context);
            _cuentasServices = new CuentasServices(context);
        }

        // GET: OtrosGastos
        public ActionResult Index()
        {
            var diaContableId = _periodoContableService.GetDiaContableActual().Id;
            return View(_db.Set<OtrosGastos>().Where(g => g.DiaContableId == diaContableId).ToList());
        }


        public ActionResult Historico()
        {
            return View(_db.Set<OtrosGastos>().OrderBy(g => g.DiaContableId).ToList());
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
                var cuentaCaja = _cuentasServices.FindCuentaByNombre("Caja");
                var cuentaGasto = _cuentasServices.FindCuentaByNombre("Gastos");
                var concepto = _db.Set<ConceptoDeGasto>().Find(otrosGastos.ConceptoDeGastoId);
                string detalle = "Pago de: " + concepto.Nombre;
                if (_submayorService.AgregarOperacion(cuentaCaja.Id, cuentaGasto.Id, otrosGastos.Importe, DateTime.Now,
                    detalle, User.Identity.GetUserId()))
                {
                    TempData["exito"] = "Gasto agregado correctamente";
                }
                else
                {
                    TempData["error"] = "Error al agregar el gasto";
                }
                return RedirectToAction("Index");
            }
            ViewBag.DiaContable = _periodoContableService.GetDiaContableActual();
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
                var entry = _db.Entry(otrosGastos);
                entry.State = EntityState.Modified;
                var importeAnterior = (decimal)entry.GetDatabaseValues()["Importe"];

                var cuentaCaja = _cuentasServices.FindCuentaByNombre("Caja");
                var cuentaGasto = _cuentasServices.FindCuentaByNombre("Gastos");
                var concepto = _db.Set<ConceptoDeGasto>().Find(otrosGastos.ConceptoDeGastoId);

                string detalle = "Pago de: " + concepto.Nombre;
                _db.Set<Asiento>().Add(_submayorService.CrearAsientoContable(cuentaGasto.Id, cuentaCaja.Id, importeAnterior, DateTime.Now,
                    "Ajuste por error en gasto " + otrosGastos.Id + " de " + concepto.Nombre, User.Identity.GetUserId()));
                _db.Set<Asiento>().Add(_submayorService.CrearAsientoContable(cuentaCaja.Id, cuentaGasto.Id, otrosGastos.Importe, DateTime.Now,
                    detalle, User.Identity.GetUserId()));
                if (_submayorService.GuardarCambios())
                {
                    TempData["exito"] = "Gasto modificado correctamente";
                }
                else
                {
                    TempData["error"] = "Error al modificar el gasto";
                }
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
            var cuentaCaja = _cuentasServices.FindCuentaByNombre("Caja");
            var cuentaGasto = _cuentasServices.FindCuentaByNombre("Gastos");
            var concepto = _db.Set<ConceptoDeGasto>().Find(gasto.ConceptoDeGastoId);

            if (_submayorService.AgregarOperacion(cuentaGasto.Id, cuentaCaja.Id, gasto.Importe, DateTime.Now,
                "Ajuste por error en gasto " + gasto.Id + " de " + concepto.Nombre, User.Identity.GetUserId()))
            {
                TempData["exito"] = "Gasto eliminado correctamente";
            }
            else
            {
                TempData["error"] = "No se pudo eliminar el gasto";
            }
            return RedirectToAction("Index");
        }

    }
}