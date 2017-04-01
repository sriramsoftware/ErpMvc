using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ErpMvc.Controllers
{
    public class AgregadosController : Controller
    {
        // GET: Agregados
        public ActionResult Index()
        {
            return View();
        }

        // GET: Agregados/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Agregados/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Agregados/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Agregados/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Agregados/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Agregados/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Agregados/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
