using Microsoft.AspNetCore.Mvc;

namespace POS_PruebaTecnica.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");
            return View();
        }
    }
}
