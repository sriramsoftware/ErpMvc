using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using ErpMvc.Models;
using SeguridadCore.Utiles;
using VerificadorDeLicencia;

namespace ErpMvc.Controllers
{
    //[Authorize(Roles = RolesMontin.Administrador)]
    public class LicenciaController : Controller
    {
        private ErpContext _db = new ErpContext();
        // GET: Licencia
        public ActionResult Index()
        {
            var licencia = _db.Licencias.SingleOrDefault();
            return View(licencia);
        }

        [HttpPost]
        public ActionResult Nueva(HttpPostedFileBase licencia)
        {
            var path = HostingEnvironment.ApplicationPhysicalPath;
            licencia.SaveAs(path + "\\licencia.lic");

            var lic = CargarLicencia.CargarDeFichero(path + "\\licencia.lic");

            var cl = new ComprobadorDeLicencia();
            if (cl.Verificar(lic, DateTime.Now))
            {
                var licenciaInfo = new LicenciaInfo()
                {
                    Suscriptor = lic.Suscriptor,
                    Aplicacion = lic.Aplicacion,
                    FechaDeVencimiento = lic.FechaDeVencimiento,
                    Hash = lic.LicenceHash
                };

                _db.Licencias.RemoveRange(_db.Licencias.ToList());
                _db.Licencias.Add(licenciaInfo);
                _db.SaveChanges();
                TempData["exito"] = "Licencia agregada correctamente";
            }
            else
            {
                TempData["error"] = "Licencia no valida";
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult Quitar()
        {
            _db.Licencias.RemoveRange(_db.Licencias.ToList());
            _db.SaveChanges();
            TempData["exito"] = "Licencia eliminada correctamente";
            return RedirectToAction("Index");
        }
    }
}