using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ErpMvc.Models;
using ErpMvc.ViewModels;
using LicenciaCore;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using SeguridadCore.Models;
using SeguridadCore.Utiles;
using SeguridadCore.ViewModels;
using VerificadorDeLicencia;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class SeguridadController : Controller
    {
        private readonly DbContext _db;
        public UserManager<Usuario> UserManager { get; private set; }

        public SeguridadController(DbContext context)
        {
            _db = context;
            UserManager = new UserManager<Usuario>(new UserStore<Usuario>(_db));
        }
        
        //
        // GET: /Usuario/Autenticarse
        [AllowAnonymous]
        public ActionResult Autenticarse(string returnUrl)
        {
            var licencia = _db.Set<LicenciaInfo>().SingleOrDefault();
            if (licencia != null)
            {
                var cl = new ComprobadorDeLicencia();
                var lic = new Licencia();
                lic.Suscriptor = licencia.Suscriptor;
                lic.Aplicacion = licencia.Aplicacion;
                lic.FechaDeVencimiento = licencia.FechaDeVencimiento;
                lic.LicenceHash = licencia.Hash;
                if (!cl.Verificar(lic, DateTime.Now))
                {
                    _db.Set<LicenciaInfo>().RemoveRange(_db.Set<LicenciaInfo>().ToList());
                    _db.SaveChanges();
                    return RedirectToAction("Index", "Licencia");
                }
            }
            else
            {
                return RedirectToAction("Index", "Licencia");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Usuario/Autenticarse
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Autenticarse(AutenticarseViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindAsync(model.NombreUsuario, model.Contraseña);
                if (user != null)
                {
                    if (!user.Activo)
                    {
                        ModelState.AddModelError("", "El usuario esta deshabilitado.");
                        return View(model);
                    }
                    await SignInAsync(user, false);
                    return RedirectToLocal(returnUrl);
                }
                ModelState.AddModelError("", "Contraseña o nombre de usuario incorrecto.");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Usuario/CambiarContraseña
        [Authorize(Roles = RolesMontin.Administrador + "," + RolesMontin.UsuarioAvanzado + "," + RolesMontin.Vendedor)]
        public ActionResult CambiarContraseña(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ExitoCambioContraseña ? "Su contraseña ha sido cambiada correctamente."
                : message == ManageMessageId.ExitoCrearContraseña ? "Su contraseña ha sido definida."
                : message == ManageMessageId.Error ? "Ha ocurrido un error."
                : "";
            ViewBag.HasLocalPassword = HasPassword();
            ViewBag.ReturnUrl = Url.Action("CambiarContraseña");
            return View();
        }

        //
        // POST: /Usuario/CambiarContraseña
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesMontin.Administrador + "," + RolesMontin.UsuarioAvanzado + "," + RolesMontin.Vendedor)]
        public async Task<ActionResult> CambiarContraseña(CambiarContraseñaViewModel model)
        {
            bool hasPassword = HasPassword();
            ViewBag.HasLocalPassword = hasPassword;
            ViewBag.ReturnUrl = Url.Action("CambiarContraseña");
            if (hasPassword)
            {
                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.ContraseñaActual, model.ContraseñaNueva);
                    if (result.Succeeded)
                    {
                        TempData["exito"] = ManageMessageId.ExitoCambioContraseña;
                        return RedirectToAction("CambiarContraseña");
                    }
                    AddErrors(result);
                }
            }
            else
            {
                // User does not have a password so remove any validation errors caused by a missing OldPassword field
                ModelState state = ModelState["ContraseñaActual"];
                if (state != null)
                {
                    state.Errors.Clear();
                }

                if (ModelState.IsValid)
                {
                    IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.ContraseñaNueva);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("CambiarContraseña", new { Message = ManageMessageId.ExitoCrearContraseña });
                    }
                    AddErrors(result);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult ResetearContraseña(string usuarioId)
        {
            var data = new ResetearContraseñaViewModel() {Usuarioid = usuarioId};
            return View(data);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesMontin.Administrador)]
        public async Task<ActionResult> ResetearContraseña(ResetearContraseñaViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Set<Usuario>().Find(model.Usuarioid);
                user.PasswordHash = "AGo6bjEybjpmoFh0EwbHXayf+5ZdYtt4a5LYYa8tTkd412v9yCkZ0VhgbQcgrCVdEg==";
                _db.Entry(user).State = EntityState.Modified;
                _db.SaveChanges();
                IdentityResult result = await UserManager.ChangePasswordAsync(user.Id, "123456", model.ContraseñaNueva);
                if (result.Succeeded)
                {
                    TempData["exito"] = "Contraseña reseteada correctamente";
                    return RedirectToAction("Index","Trabajadores");
                }
                else
                {
                    AddErrors(result);
                }
            }
            return View(model);
        }

        //
        // POST: /Usuario/CerrarSesion
        [HttpPost]
        public ActionResult CerrarSesion()
        {
            _db.Set<LogDeAcceso>().Add(new LogDeAcceso(){UsuarioId = User.Identity.GetUserId(), Fecha = DateTime.Now, TipoDeAcceso = TipoDeAcceso.CerrarSesion});
            AuthenticationManager.SignOut();
            _db.SaveChanges();
            return RedirectToAction("Index", "Inicio");
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult ListaUsuario()
        {
            var usuarios = _db.Set<Usuario>().Where(u => u.Activo).ToList();
            return View(usuarios);
        }

        
        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult CrearUsuario()
        {
            var roles = new List<dynamic>()
            {
              new { Nombre = RolesMontin.Administrador},
              new { Nombre = RolesMontin.UsuarioAvanzado},
              new { Nombre = RolesMontin.Vendedor},
              new { Nombre = RolesMontin.CapitanDeSalon}
            };
            ViewBag.Roles = new SelectList(roles, "Nombre", "Nombre");
            return View();
        }

        //
        // POST: /Usuario/CerrarSesion
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesMontin.Administrador)]
        public async Task<ActionResult> CrearUsuario([Bind(Include = "Id,NombreUsuario,Roles,CorreoElectronico,Contraseña,ConfirmarContraseña")]UsuarioViewModel usuarioViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new Usuario
                {
                    UserName = usuarioViewModel.NombreUsuario,
                    Activo = true,
                    Correo = usuarioViewModel.CorreoElectronico,
                };
                var result = await UserManager.CreateAsync(user, usuarioViewModel.Contraseña);
                foreach (var rol in usuarioViewModel.Roles)
                {
                    await UserManager.AddToRoleAsync(user.Id, rol);
                }
                TempData["exito"] = ManageMessageId.ExitoCrearUsuario;
                return RedirectToAction("ListaUsuario");
            }
            var roles = new List<dynamic>()
            {
              new { Nombre = RolesMontin.Administrador},
              new { Nombre = RolesMontin.UsuarioAvanzado},
              new { Nombre = RolesMontin.Vendedor},
              new { Nombre = RolesMontin.CapitanDeSalon}
            };
            ViewBag.Roles = new MultiSelectList(roles, "Nombre", "Nombre", usuarioViewModel.Roles);
            return View(usuarioViewModel);
        }
        
        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult Eliminar(string id)
        {

            var usuario = UserManager.FindById(id);
            return PartialView("_EliminarUsuarioPartial", usuario);
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        [HttpPost]
        public ActionResult EliminarConfirmado(string id)
        {
            var usuario = UserManager.FindById(id);
            if (usuario == null)
            {
                return new HttpNotFoundResult();
            }
            usuario.Activo = !usuario.Activo;
            UserManager.Update(usuario);
            TempData["exito"] = "Usuario eliminado correctamente";
            return RedirectToAction("ListaUsuario");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing && UserManager != null)
            {
                UserManager.Dispose();
                _db.Dispose();
                UserManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private async Task SignInAsync(Usuario user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, identity);
            _db.Set<LogDeAcceso>().Add(new LogDeAcceso() { UsuarioId = user.Id, Fecha = DateTime.Now , TipoDeAcceso = TipoDeAcceso.Autenticar});
            _db.SaveChanges();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            ExitoCambioContraseña,
            ExitoCrearContraseña,
            ExitoCrearUsuario,
            ExitoEditarUsuario,
            Error,
            NoModificable
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Inicio");
        }

        #endregion
    }
}