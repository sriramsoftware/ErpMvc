using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ComercialCore.Models;
using CompraVentaBL;
using CompraVentaCore.Models;
using ContabilidadBL;
using ContabilidadCore.Models;
using ErpMvc.Models;
using ErpMvc.Utiles;
using Microsoft.AspNet.Identity;

namespace ErpMvc.Controllers
{
    [Authorize]
    [DiaContable]
    public class ComprasController : Controller
    {
        private ComprasService _comprasService;
        private PeriodoContableService _periodoContableService;
        private CuentasServices _cuentasServices;
        private DbContext _db;


        public ComprasController(DbContext context)
        {
            _db = context;
            _comprasService = new ComprasService(context);
            _periodoContableService = new PeriodoContableService(context);
            _cuentasServices = new CuentasServices(context);
        }

        public ActionResult Index(int id = 0)
        {
            if (id == -1)
            {
                TempData["error"] = "El dia seleccionado no existe!";
                id = 0;
            }
            var diaContable = id == 0
                ? _periodoContableService.GetDiaContableActual()
                : _periodoContableService.BuscarDiaContable(id);
            ViewBag.DiaContable = diaContable;
            return View();
        }

        [HttpPost]
        public ActionResult BuscarCompras(DateTime fecha)
        {
            return RedirectToAction("ListaDeComprasPartial", "Compras", new {fecha = fecha});
        }

        public PartialViewResult ListaDeComprasPartial(DateTime fecha)
        {
            //todo: cambiar aqui y en compras la verificacion de la fecha para no cargar todos los registros
            if (User.IsInRole(RolesMontin.Administrador))
            {
                var compras = _db.Set<Compra>().ToList().Where(v => v.Fecha.Date == fecha.Date).ToList().OrderByDescending(v => v.Fecha);
                return PartialView("_ListaDeComprasPartial", compras);
            }
            var comprasSel = _db.Set<SeleccionCompra>().ToList().Where(v => v.Compra.Fecha.Date == fecha.Date).ToList().OrderByDescending(v => v.Compra.Fecha);
            return PartialView("_ListaDeComprasPartial", comprasSel.Select(c => c.Compra));
        }

        public PartialViewResult ProductosComprados(int id)
        {
            var detalleDeCompra = _db.Set<Compra>().Find(id).Productos;
            return PartialView("_ProductosCompradosPartial", detalleDeCompra);
        }

        // GET: Compra
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult TramitarCompra()
        {
            return View();
        }

        public JsonResult Entidades()
        {
            return Json(_db.Set<Entidad>().Select(e => new {e.Id, e.Nombre}), JsonRequestBehavior.AllowGet);
        }


        public JsonResult AgregarEntidad(string nombre)
        {
            var entidad = new Entidad()
            {
                Nombre = nombre,
                CodigoReup = "0",
                CtaBancariaCuc = "0",
                CtaBancariaMn = "0",
                Direccion = "ninguna",
                Nit = "00000000000",
            };
            _db.Set<Entidad>().Add(entidad);
            _db.SaveChanges();
            return Json(_db.Set<Entidad>().Select(e => new { e.Id, e.Nombre }), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult TramitarCompra(Compra compra, bool esPorCaja)
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
                bool result = true;
                if (esPorCaja)
                {
                    var cta = _cuentasServices.FindCuentaByNombre("Caja");
                    var totalCompra = compra.Productos.Sum(p => p.ImporteTotal);
                    if (cta.Disponibilidad.Saldo - totalCompra < 0)
                    {
                        TempData["error"] = "No se puede pagar por caja porque no existe esta cantidad de efectivo";
                        result = false;
                    }
                }
                if (result)
                {
                    if (_comprasService.ComprarYDarEntradaAAlmacen(compra, esPorCaja, usuario))
                    {
                        TempData["exito"] = "Compra agregada correctamente";
                        
                    } 
                }
                return RedirectToAction("Index", "Compras");
            }
            return RedirectToAction("Index", "Compras");
        }



        [Authorize(Roles = RolesMontin.UsuarioAvanzado + "," + RolesMontin.Administrador)]
        public ActionResult Editar(int id)
        {
            var compra = _db.Set<Compra>().Find(id);
            return View(compra);
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

        [HttpPost]
        public JsonResult AgregarDetalle(DetalleDeCompra detalle)
        {
            if (_comprasService.AgregarDetalleACompra(detalle))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarDetalle(int id)
        {
            if (_comprasService.EliminarDetalleDeCompra(id))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
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