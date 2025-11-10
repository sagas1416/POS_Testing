using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using POS_PruebaTecnica.Models.Productos;
using POS_PruebaTecnica.Models.ServiciosRest;

namespace POS_PruebaTecnica.Controllers
{
    public class ProductosController : Controller
    {
        private Producto_Service _service;
        public ProductosController(Producto_Service service)
        {
            _service = service;
        }
        // GET: ProductosController
        public async Task<ActionResult> Index()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            if (HttpContext.Session.GetString("Mensaje") != null)
            {
                ViewBag.Mensaje = HttpContext.Session.GetString("Mensaje");
                HttpContext.Session.Remove("Mensaje");
            }
            var token = HttpContext.Session.GetString("Token");
            var productos = await _service.GetProductos(token! );
            return View(productos);
        }

        // GET: ProductosController/Details/5
        public ActionResult Details(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");
            return View();
        }

        // GET: ProductosController/Create
        public ActionResult Create()
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");
            return View();
        }

        // POST: ProductosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Model_Productos producto)
        {
            try
            {
                if (HttpContext.Session.GetString("Usuario") == null)
                    return RedirectToAction("Index", "Login");
                var token = HttpContext.Session.GetString("Token");
                await _service.CrearProducto(producto, token);
                HttpContext.Session.SetString("Mensaje", "Producto creado correctamente");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProductosController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            try
            {
                if (HttpContext.Session.GetString("Mensaje") != null)
                {
                    ViewBag.Mensaje = HttpContext.Session.GetString("Mensaje");
                    HttpContext.Session.Remove("Mensaje");
                }
                var token = HttpContext.Session.GetString("Token")??"";
                var producto =  await _service.GetProducto(token, id);
                if(producto.nkey ==0)
                {
                    HttpContext.Session.SetString("Mensaje", "Error- Producto no encontrado");
                    return RedirectToAction(nameof(Index));
                }
                return View(producto);
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("Mensaje", "Error-" + ex.Message);
                return View();
            }
           
        }

        // POST: ProductosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Model_Productos producto)
        {

            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");
            try
            {
                var token = HttpContext.Session.GetString("Token");
                string mensaje = await _service.ActualizarProducto(producto, token);
                HttpContext.Session.SetString("Mensaje", mensaje);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                HttpContext.Session.SetString("Mensaje", "Error-" + ex.Message);
                return View();
            }
        }

        // GET: ProductosController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
