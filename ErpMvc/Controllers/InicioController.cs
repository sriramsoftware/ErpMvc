using System.Data.Entity;
using System.Web.Mvc;
using ContabilidadBL;

namespace ErpMvc.Controllers
{
    public class InicioController : Controller
    {
        private PeriodoContableService _periodoContableService ;

        public InicioController(DbContext context)
        {
            _periodoContableService = new PeriodoContableService(context);
        }
        //private ErpContext db = new ErpContext();        
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.DiaContable = _periodoContableService.GetDiaContableActual();
                return View();
            }
            return RedirectToAction("Autenticarse", "Seguridad");

        }
    }
}