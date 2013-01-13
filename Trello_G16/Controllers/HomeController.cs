using System.Web.Mvc;

namespace Etapa2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Bem Vindo!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
