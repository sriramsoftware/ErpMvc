using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ComercialCore.Models;
using CompraVentaBL;
using ContabilidadCore.Models;
using ErpMvc.Models;

namespace ErpMvc.Controllers
{
    [Authorize(Roles = RolesMontin.Administrador)]
    public class ClasificacionesDeProductosController : Controller
    {
        private DbContext _db;
        private ProductoService _productoService;

        public ClasificacionesDeProductosController(DbContext context)
        {
            _db = context;
            _productoService = new ProductoService(context);
        }

        // GET: ClasificacionesDeProductos
        public ActionResult Listado()
        {
            return View(_db.Set<ClasificacionDeProducto>().ToList());
        }

        public ActionResult Agregar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Agregar(ClasificacionDeProducto clasificacionDeProducto)
        {
            if (ModelState.IsValid)
            {
                _db.Set<ClasificacionDeProducto>().Add(clasificacionDeProducto);
                _db.SaveChanges();
                return RedirectToAction("Listado");
            }
            return View();
        }


        public ActionResult Editar(int id)
        {
            var clasif = _db.Set<ClasificacionDeProducto>().Find(id);
            return View(clasif);
        }

        [HttpPost]
        public ActionResult Editar(CentroDeCosto centroDeCosto)
        {
            //if (_centroDeCostoService.ModificarCentroDeCosto(centroDeCosto))
            //{
            //    TempData["exito"] = "Centro de costo modificado correctamente";
            //    return RedirectToAction("Index");
            //}
            return View(centroDeCosto);
        }


        public ActionResult Eliminar(int id)
        {
            return PartialView("_EliminarTrabajadorPartial");
        }

        [HttpPost]
        [ActionName("Eliminar")]
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