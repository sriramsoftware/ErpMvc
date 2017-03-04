using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using CompraVentaCore.Models;
using ContabilidadBL;
using ErpMvc.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SeguridadCore.Models;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class ControlDeAsistenciaController : Controller
    {
        private ErpContext _db = new ErpContext();
        private PeriodoContableService _periodoContableService;
        public UserManager<Usuario> UserManager { get; private set; }

        public ControlDeAsistenciaController()
        {
            _periodoContableService = new PeriodoContableService(_db);
            UserManager = new UserManager<Usuario>(new UserStore<Usuario>(_db));
        }

        
        // GET: ControlDeAsistencia
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Firmar(string nombreUsuario, string contraseña)
        {
            var usuario = UserManager.Find(nombreUsuario, contraseña);
            if (usuario == null)
            {
                TempData["error"] = "Nombre de usuario o contraseña incorrecta";
                return RedirectToAction("Index");
            }
            var vendedor = _db.Set<Vendedor>().SingleOrDefault(v => v.UsuarioId == usuario.Id);
            if (vendedor == null || usuario.Roles.Any(r => r.Role.Name == RolesMontin.Administrador))
            {
                TempData["error"] = "Este usuario no registra la asistencia";
                return RedirectToAction("Index");
            }
            var diaContable = _periodoContableService.GetDiaContableActual();
           
            var asistencia = _db.Asistencias.SingleOrDefault(a => a.DiaContableId == diaContable.Id && a.VendedorId == vendedor.Id);
            if (asistencia == null)
            {
                asistencia = new Asistencia() {VendedorId = vendedor.Id, DiaContableId = diaContable.Id};
                _db.Set<Asistencia>().Add(asistencia);
            }

            if (asistencia.Entrada == null)
            {
                asistencia.Entrada = DateTime.Now;
            }
            else if (asistencia.Salida == null)
            {
                asistencia.Salida = DateTime.Now;
            }
            _db.SaveChanges();
            TempData["exito"] = "Asistencia registrada correctamente";
            return RedirectToAction("Index");
        }

        public PartialViewResult ListadoAsistencia()
        {
            var diaContable = _periodoContableService.GetDiaContableActual();
            var asistencias = _db.Asistencias.Where(a => a.DiaContableId == diaContable.Id).ToList();
            return PartialView("_ListadoAsistenciaPartial",asistencias);
        }
    }
}