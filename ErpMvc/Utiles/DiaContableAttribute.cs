using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using ContabilidadBL;
using ErpMvc.Controllers;
using ErpMvc.Models;
using LicenciaCore;
using VerificadorDeLicencia;

namespace ErpMvc.Utiles
{
    public class DiaContableAttribute : ActionFilterAttribute
    {
        private DbContext _db;
        private PeriodoContableService _periodoContableService;

        public DiaContableAttribute()
        {
            _db = new ErpContext();
            _periodoContableService = new PeriodoContableService(_db);
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!(filterContext.Controller is PeriodoContableController))
            {
                if (_periodoContableService.GetDiaContableActual() == null)
                {
                    var routeValues = new RouteValueDictionary();
                    routeValues.Add("controller", "PeriodoContable");
                    routeValues.Add("action", "AbrirDia");

                    filterContext.Result = new RedirectToRouteResult(routeValues);
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}