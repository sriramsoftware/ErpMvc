﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CajaCore.Models;
using CompraVentaCore.Models;
using ContabilidadBL;
using ContabilidadCore.Models;
using ErpMvc.Models;
using ErpMvc.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SeguridadCore.Models;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class PeriodoContableController : Controller
    {
        private PeriodoContableService _service;
        private DbContext _db;

        public PeriodoContableController(DbContext context)
        {
            _service = new PeriodoContableService(context);
            _db = context;
        }
        // GET: PeriodoContable
        public PartialViewResult DiaContable()
        {
            var diaContable = _service.GetDiaContableActual();
            return PartialView("_DiaContablePartial", diaContable);
        }

        public JsonResult DiaContableData()
        {
            var diaContable = _service.GetDiaContableActual();
            return Json(diaContable.Fecha.ToShortDateString(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult ResumenEfectivoData()
        {
            //var diaAnterior = _db.Set<DiaContable>().OrderBy(d => d.Fecha).Skip(1).FirstOrDefault();
            var dia = _service.GetDiaContableActual();
            var resumen = new
            {
                EfectivoAnterior = _db.Set<Caja>().SingleOrDefault().Efectivo.Sum(e => e.Cantidad * e.Denominacion.Valor),
                Ventas = _db.Set<Venta>().Where(v => v.DiaContableId == dia.Id).Sum(v => v.Importe),
                Depositos = 0,
                Extracciones = 0,
            };
            return Json(resumen, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DenominacionesData()
        {
            var billetes = _db.Set<DenominacionDeMoneda>().Where(d => d.Billete).GroupBy(d => d.Valor).OrderByDescending(d => d.Key).Select(d => new DenominacionViewModel()
            {
                Valor = d.Key,
                CantidadCup = 0.00m,
                CantidadCuc = 0.00m,
                Cup = d.Any(e => e.Moneda.Sigla == "CUP"),
                Cuc = d.Any(e => e.Moneda.Sigla == "CUC")
            });

            var monedas = _db.Set<DenominacionDeMoneda>().Where(d => !d.Billete).GroupBy(d => d.Valor).OrderByDescending(d => d.Key).Select(d => new DenominacionViewModel()
            {
                Valor = d.Key,
                CantidadCup = 0.00m,
                CantidadCuc = 0.00m,
                Cup = d.Any(e => e.Moneda.Sigla == "CUP"),
                Cuc = d.Any(e => e.Moneda.Sigla == "CUC")
            });
            
            var denominaciones = new DesgloceEfectivoViewModel()
            {
                Billetes = billetes.ToList(),
                Monedas = monedas.ToList(),
            };
            return Json(denominaciones, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CerrarPeriodo(DesgloceEfectivoViewModel desgloceEfectivoViewModel)
        {

            return RedirectToAction("Index", "Inicio");
        }

        public ActionResult CerrarPeriodo()
        {
            return View();
        }


        public PartialViewResult DesgloseEfectivo()
        {
            return PartialView("_DesgloseEfectivoPartial");
        }

        public PartialViewResult ResumenDeEfectivo()
        {
            return PartialView("_ResumenDeEfectivoPartial");
        }

        public ActionResult AbrirDia()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AbrirDia(DateTime? fecha)
        {
            if (fecha != null)
            {
                _service.CerrarDiaContable();
                _service.EmpezarDiaContable(fecha.Value);
                TempData["exito"] = "Dia abierto correctamente, registre la laentrada de los trabajadores";
                return RedirectToAction("Index", "ControlDeAsistencia");
            }
            return View();
        }
    }
}