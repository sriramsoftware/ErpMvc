using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using ErpMvc.Controllers;
using ErpMvc.Models;
using LicenciaCore;
using VerificadorDeLicencia;

namespace ErpMvc.Utiles
{
    public class LicenciaAttribute:ActionFilterAttribute
    {
         private ErpContext _db = new ErpContext();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller is LicenciaController)
            {

            }
            else
            {
                var licencia = _db.Licencias.FirstOrDefault();
                if (licencia == null)
                {
                    var routeValues = new RouteValueDictionary();
                    routeValues.Add("controller", "Licencia");
                    routeValues.Add("action", "Index");

                    filterContext.Result = new RedirectToRouteResult(routeValues);
                }
                else
                {
                    var cl = new ComprobadorDeLicencia();
                    var lic = new Licencia();
                    lic.Suscriptor = licencia.Suscriptor;
                    lic.Aplicacion = licencia.Aplicacion;
                    lic.FechaDeVencimiento = licencia.FechaDeVencimiento;
                    lic.LicenceHash = licencia.Hash;
                    if (cl.Verificar(lic, DateTime.Now))
                    {
                        var routeValues = new RouteValueDictionary();
                        routeValues.Add("controller", "Inicio");
                        routeValues.Add("action", "Index");

                        filterContext.Result = new RedirectToRouteResult(routeValues);
                    }
                    else
                    {
                        var routeValues = new RouteValueDictionary();
                        routeValues.Add("controller", "Licencia");
                        routeValues.Add("action", "Index");

                        filterContext.Result = new RedirectToRouteResult(routeValues);

                    }
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}