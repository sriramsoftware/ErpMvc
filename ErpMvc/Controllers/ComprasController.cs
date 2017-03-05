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
using Microsoft.AspNet.Identity;

namespace ErpMvc.Controllers
{
    [Authorize]
    public class ComprasController : Controller
    {
        private ComprasService _comprasService;


        public ComprasController(ComprasService comprasService)
        {
            _comprasService = comprasService;
        }

        // GET: Compra
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult TramitarCompra()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult TramitarCompra(Compra compra)
        {
            var usuario = User.Identity.GetUserId();
            compra.UsuarioId = usuario;
            if (ModelState.IsValid)
            {
                if (!compra.Productos.Any())
                {
                    TempData["error"] = "No se puede efectuar una compra vacia";
                    return View();
                }
                if (_comprasService.ComprarYPasarACentroDeCosto(compra,usuario))
                {

                    return RedirectToAction("Index", "Inicio");
                }
            }
            return View(compra);
        }



        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(int id)
        {
            //var centro = _centroDeCostoService.CentrosDeCosto().Find(id);
            return View();
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(Compra compra)
        {
            //if (_centroDeCostoService.ModificarCentroDeCosto(centroDeCosto))
            //{
            //    TempData["exito"] = "Centro de costo modificado correctamente";
            //    return RedirectToAction("Index");
            //}
            return View(compra);
        }


        [Authorize(Roles = RolesMontin.Administrador)]
        public ActionResult Eliminar(int id)
        {
            return PartialView("_EliminarTrabajadorPartial");
        }

        [Authorize(Roles = RolesMontin.Administrador)]
        [HttpPost]
        public ActionResult EliminarConfirmado(int id)
        {
            //var trabajador = _vendedorService.Vendedores().Find(id);
            //if (trabajador == null)
            //{
            //    return new HttpNotFoundResult();
            //}
            //trabajador.Estado = EstadoTrabajador.Baja;
            //if (trabajador.Usuario != null)
            //{
            //    trabajador.Usuario.Activo = false;
            //}
            //_vendedorService.ModificarVendedor(trabajador);
            //TempData["exito"] = "Trabajador eliminado correctamente";
            return RedirectToAction("Index");
        }

    }
}