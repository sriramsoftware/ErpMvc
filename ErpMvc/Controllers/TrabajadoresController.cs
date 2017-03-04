using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CompraVentaBL;
using CompraVentaCore.Models;
using ErpMvc.Models;
using ErpMvc.ViewModels;
using HumanResourcesCore.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SeguridadCore.Models;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class TrabajadoresController : Controller
    {
        private VendedorService _vendedorService;
        private DbContext _db;
        public UserManager<Usuario> UserManager { get; private set; }

        public TrabajadoresController(DbContext context)
        {
            _db = context;
            _vendedorService = new VendedorService(context);
            UserManager = new UserManager<Usuario>(new UserStore<Usuario>(_db));
        }

        // GET: Vendedor
        public ActionResult Index()
        {
            return View(_vendedorService.Vendedores().Include(v => v.Usuario).Include(v => v.Usuario.Roles).Where(v => v.Estado == EstadoTrabajador.Activo).ToList());
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Agregar()
        {
            var roles = new List<dynamic>()
            {
              new { Nombre = RolesMontin.UsuarioAvanzado},
              new { Nombre = RolesMontin.Vendedor}
            };
            ViewBag.Roles = new SelectList(roles, "Nombre", "Nombre");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Agregar(VendedorViewModel vendedorViewModel)
        {
            vendedorViewModel.Vendedor.NoExpedienteLaboral = vendedorViewModel.Vendedor.Ci;
            vendedorViewModel.Vendedor.Direccion = new Direccion() {No = "1",Calle = "1",};
            if (_db.Set<Usuario>().Any(t => t.UserName == vendedorViewModel.UsuarioViewModel.NombreUsuario) )
            {
                TempData["error"] = "Ya existe el usuario";
                return RedirectToAction("Index");
            }
            var user = new Usuario
            {
                UserName = vendedorViewModel.UsuarioViewModel.NombreUsuario,
                Activo = true,
                Correo = vendedorViewModel.UsuarioViewModel.NombreUsuario + "@montin.com",
            };
            var result = UserManager.Create(user, vendedorViewModel.UsuarioViewModel.Contraseña);
            foreach (var rol in vendedorViewModel.Roles)
            {
                UserManager.AddToRole(user.Id, rol);
            }
            vendedorViewModel.Vendedor.UsuarioId = user.Id;
            vendedorViewModel.Vendedor.PuntoDeVentaId = _vendedorService.PuntosDeVentas().FirstOrDefault().Id;
            if (_vendedorService.AgregarVendedor(vendedorViewModel.Vendedor))
            {
               //_db.SaveChanges();
                TempData["exito"] = "Trabajador agregado correctamente";
                return RedirectToAction("Index");
            }
            var roles = new List<dynamic>()
            {
              new { Nombre = RolesMontin.UsuarioAvanzado},
              new { Nombre = RolesMontin.Vendedor}
            };
            ViewBag.Roles = new SelectList(roles, "Nombre", "Nombre",vendedorViewModel.Roles);
            return View(vendedorViewModel);
        }

        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(int id)
        {
            var vendedor = _vendedorService.Vendedores().Find(id);
            var viewModel = new VendedorViewModel(vendedor);
            var roles = new List<dynamic>()
            {
              new { Nombre = RolesMontin.UsuarioAvanzado},
              new { Nombre = RolesMontin.Vendedor}
            };
            if (vendedor.Usuario != null)
            {
                ViewBag.Roles = new SelectList(roles, "Nombre", "Nombre", vendedor.Usuario.Roles.Select(r => r.Role.Name));
            }
            else
            {
                ViewBag.Roles = new SelectList(roles, "Nombre", "Nombre");
            }
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(VendedorViewModel vendedorViewModel)
        {
            //vendedorViewModel.Vendedor.PuntoDeVentaId = vendedorViewModel.PuntoDeVentaId;
            if (_vendedorService.ModificarVendedor(vendedorViewModel.Vendedor))
            {
                //var tarjeta = _db.Set<TarjetaDeAsistencia>().Find(vendedorViewModel.Vendedor.Id);
                //if (_db.Set<TarjetaDeAsistencia>().Any(t => t.Usuario == vendedorViewModel.UsuarioViewModel.NombreUsuario && t.VendedorId != vendedorViewModel.Vendedor.Id))
                //{
                //    TempData["error"] = "El usuario ya existe";
                //    return RedirectToAction("Index");
                //}
                //if (tarjeta == null)
                //{
                //    tarjeta = new TarjetaDeAsistencia() {VendedorId = vendedorViewModel.Vendedor.Id, Usuario = vendedorViewModel.Usuario, Contraseña = vendedorViewModel.Contraseña};
                //    _db.Set<TarjetaDeAsistencia>().Add(tarjeta);
                //}
                //else
                //{
                //    tarjeta.Usuario = vendedorViewModel.Usuario;
                //    tarjeta.Contraseña = vendedorViewModel.Contraseña;
                //}
                //_db.SaveChanges();
                TempData["exito"] = "Trabajador modificado correctamente";
                return RedirectToAction("Index");
            }
            var roles = new List<dynamic>()
            {
              new { Nombre = RolesMontin.UsuarioAvanzado},
              new { Nombre = RolesMontin.Vendedor}
            };
            ViewBag.Roles = new SelectList(roles, "Nombre", "Nombre", vendedorViewModel.Roles);
            return View(vendedorViewModel);
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult Eliminar(int id)
        {

            var trabajador = _vendedorService.Vendedores().Find(id);
            return PartialView("_EliminarTrabajadorPartial", trabajador);
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        [HttpPost]
        [ActionName("Elilminar")]
        public ActionResult EliminarConfirmado(int id)
        {
            var trabajador = _vendedorService.Vendedores().Find(id);
            if (trabajador == null)
            {
                return new HttpNotFoundResult();
            }
            trabajador.Estado = EstadoTrabajador.Baja;
            if (trabajador.Usuario != null)
            {
                trabajador.Usuario.Activo = false;
            }
            _vendedorService.ModificarVendedor(trabajador);
            TempData["exito"] = "Trabajador eliminado correctamente";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}