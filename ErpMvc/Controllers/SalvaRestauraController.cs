using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ErpMvc.Controllers
{
    public class SalvaRestauraController : Controller
    {
        // GET: SalvaRestaura
        public ActionResult Index()
        {
            //Console.WriteLine("mysqldump -u root -p amelia > c.sql\n\r");
            Process.Start(@"C:\xampp\mysql\bin\mysqldump.exe", "-u root -p amelia > c.sql");
            TempData["exito"] = "Salva correcta";
            return View();
        }
    }
}