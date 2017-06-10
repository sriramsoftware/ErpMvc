using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ErpMvc.Models;
using ErpMvc.Utiles;

namespace ErpMvc
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            UnityConfig.RegisterComponents();
            //GlobalFilters.Filters.Add(new LicenciaAttribute());
            //GlobalFilters.Filters.Add(new DiaContableAttribute(new ErpContext()));
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
