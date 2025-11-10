using Microsoft.AspNetCore.Mvc;
using POS_PruebaTecnica.Models.ServiciosRest;
using System.Threading.Tasks;

namespace POS_PruebaTecnica.Controllers
{
    public class LoginController : Controller
    {
        private Login _getTokenUser;
        public LoginController(Login login)
        {
            _getTokenUser = login;

        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Index(string usuario, string password)
        {
            try
            { 
                var resultado = await _getTokenUser.getTokenUser(usuario, password);
                if (resultado.Token != null)
                {
                    HttpContext.Session.SetString("Token", resultado.Token!);
                    HttpContext.Session.SetString("Usuario", resultado.usuario!);
                    HttpContext.Session.SetInt32("IdUser", resultado.id);
                    HttpContext.Session.SetInt32("Permiso", resultado.Tipo ?? 0);
                    if (resultado.Tipo == 1)
                        return RedirectToAction("Index", "Dashboard");
                    else if (resultado.Tipo == 0)
                        return RedirectToAction("Index", "POS");
                    else
                        ViewBag.Mensaje = "Error- No tienes acceso a ningun modulo";
                   
                }
                else
                {
                    ViewBag.Mensaje = resultado.Mensaje;
                }
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error-"+ ex.Message +" "+ ex.InnerException?.Message;
            }
            return View();
        }

        public IActionResult Cerrar()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Login");
            
        }
    }
}
