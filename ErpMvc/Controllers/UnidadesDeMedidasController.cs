using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using ComercialCore.Models;
using CompraVentaBL;
using ErpMvc.Models;

namespace ErpMvc.Controllers
{
    [Authorize(Roles = RolesMontin.Administrador)]
    public class UnidadesDeMedidasController : Controller
    {
        private ProductoService _productoService;

        public UnidadesDeMedidasController(ProductoService service)
        {
            _productoService = service;
        }

        // GET: UnidadesDeMedidas
        public ActionResult Listado()
        {
            return View(_productoService.ListaDeUnidadesDeMedida().Include(u => u.TipoDeUnidadDeMedida));
        }

        public ActionResult Agregar()
        {
            ViewBag.TipoDeUnidadDeMedidaId = new SelectList(_productoService.ListaTipoDeUnidadDeMedidas(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Agregar(UnidadDeMedida unidadDeMedida)
        {
            if (ModelState.IsValid)
            {
                _productoService.AgregarUnidadDeMedida(unidadDeMedida);
                TempData["exito"] = "Unidad agregada correctamente!";
                return RedirectToAction("Listado");
            }
            ViewBag.TipoDeUnidadDeMedidaId = new SelectList(_productoService.ListaTipoDeUnidadDeMedidas(), "Id", "Name",unidadDeMedida.TipoDeUnidadDeMedidaId);
            return View(unidadDeMedida);
        }

        public ActionResult Editar(int id)
        {
            var unidad = _productoService.ListaDeUnidadesDeMedida().Find(id);
            ViewBag.TipoDeUnidadDeMedidaId = new SelectList(_productoService.ListaTipoDeUnidadDeMedidas(), "Id", "Name", unidad.TipoDeUnidadDeMedidaId);
            return View(unidad);
        }

        [HttpPost]
        public ActionResult Editar(UnidadDeMedida unidadDeMedida)
        {
            if (ModelState.IsValid)
            {
                _productoService.ModificarUnidadDeMedida(unidadDeMedida);
                TempData["exito"] = "Unidad modificada correctamente!";
                return RedirectToAction("Listado");
            }
            ViewBag.TipoDeUnidadDeMedidaId = new SelectList(_productoService.ListaTipoDeUnidadDeMedidas(), "Id", "Name", unidadDeMedida.TipoDeUnidadDeMedidaId);
            return View(unidadDeMedida);
        }

        //public ActionResult Eliminar(int id)
        //{
        //    var producto = _productoService.ListaDeUnidadesDeMedida().Find(id);
        //    return View(producto);
        //}

        //[HttpPost]
        //[ActionName("Eliminar")]
        //public ActionResult ComfirmarEliminar(int id)
        //{
            
        //    if (_productoService.EliminarUnidadDeMedida(id) > 0)
        //    {
        //        TempData["exito"] = "Unidad eliminada correctamente!";
        //        return RedirectToAction("Listado");
        //    }
        //    TempData["error"] = "No se pudo eliminar la unidad de medida!";
        //    return RedirectToAction("Listado");
        //}
    }
}