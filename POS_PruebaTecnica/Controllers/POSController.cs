using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using POS_PruebaTecnica.Models.POS;
using POS_PruebaTecnica.Models.Productos;
using POS_PruebaTecnica.Models.ServiciosRest;
using System.Threading.Tasks;

namespace POS_PruebaTecnica.Controllers
{
    public class POSController : Controller
    {
        private POS_Service _service;
        private Producto_Service _serviceProducto;
        public POSController(POS_Service service,Producto_Service producto)
        {
            _service = service;
            _serviceProducto = producto;
        }
        // GET: POSController
        public ActionResult Index()
        {

            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            if (HttpContext.Session.GetString("Mensaje") != null)
            {
                ViewBag.Mensaje = HttpContext.Session.GetString("Mensaje");
                HttpContext.Session.Remove("Mensaje");
            }
            try
            {

                var token = HttpContext.Session.GetString("Token");
                var ventas = _service.GetVentasHeader(token!);
                return View(ventas);
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error-" + ex.Message;
            }
            return View();
        }

        // GET: POSController/Details/5
        public async Task<ActionResult> Details(string id)
        {

            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");

            if (HttpContext.Session.GetString("Mensaje") != null)
            {
                ViewBag.Mensaje = HttpContext.Session.GetString("Mensaje");
                HttpContext.Session.Remove("Mensaje");
            }
            try
            {
                var token = HttpContext.Session.GetString("Token");
                var ventas = await _service.getVenta(token!, id);
                if (ventas.Mensaje != null)
                { 
                    HttpContext.Session.SetString("Mensaje", ventas.Mensaje);
                    return RedirectToAction(nameof(Index));
                }
                return View(ventas);
            }
            catch (Exception ex)
            {
                ViewBag.Mensaje = "Error-" + ex.Message;
            }
            return View();
        }

        // GET: POSController/Create
        public async Task<ActionResult> Create()
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
                var Productos = await _serviceProducto.GetProductos(HttpContext.Session.GetString("Token") ?? ""); 
                ViewBag.Productos = new SelectList(Productos,"nkey","descripcion");
                if (HttpContext.Session.GetString("IdVentaActual") == null)
                {
                    var idUser = HttpContext.Session.GetInt32("IdUser"); 
                    string id = Guid.NewGuid().ToString().ToUpper();
                    /*crear la instacion de model_Venta*/
                    var venta = new Model_Venta();
                    venta.id = id;
                    HttpContext.Session.SetString("IdVentaActual", venta.id);
                    venta.idTienda = 1; //valor por defecto
                    venta.idTerminal = 1; //valor por defecto
                    venta.fechahora = DateTime.Now;
                    venta.idUser = idUser ?? 0; //valor por defecto
                    /*Los valores monto se agregan en detalle*/
                    HttpContext.Session.SetString("Venta", System.Text.Json.JsonSerializer.Serialize(venta));
                    var sales = new Sales();
                    if (HttpContext.Session.GetString("ProductoSeleccionado") != null)
                    {
                        var detalle = System.Text.Json.JsonSerializer.Deserialize<List<Transaccion_Detalle>>(HttpContext.Session.GetString("ProductoSeleccionado")!);
                        sales.detalle = detalle;
                    }
                    sales.cabecera = venta;
                    return View(sales);
                }
                else
                {
                    var sales = new Sales();
                    var venta = System.Text.Json.JsonSerializer.Deserialize<Model_Venta>(HttpContext.Session.GetString("Venta")!); 

                    if (HttpContext.Session.GetString("ProductoSeleccionado") != null)
                    {
                        var det = HttpContext.Session.GetString("ProductoSeleccionado");
                        var detalle = System.Text.Json.JsonSerializer.Deserialize<List<Transaccion_Detalle>>(det); 
                        sales.detalle = detalle;
                    }
                    sales.cabecera= venta;
                    return View(sales);
                }

            }
            catch (Exception ex)
            {
                HttpContext.Session.Remove("IdVentaActual");
                HttpContext.Session.Remove("Venta");
                ViewBag.Mensaje = "Error-" + ex.Message; 
            }
           
            return View();
        }
        [HttpPost] 
        public async Task<ActionResult> addItem(string idVenta,int idProduct, string? codItem, decimal cantidad)
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                var producto = new Model_Productos();
                if(codItem==null)
                    producto = await _serviceProducto.GetProducto(token!, idProduct);
                else
                    producto = await _serviceProducto.GetProductoCode(token!, codItem);

                if (producto.Mensaje != null)
                {
                    HttpContext.Session.SetString("Mensaje", producto.Mensaje);
                    return RedirectToAction(nameof(Create));
                }
                if (HttpContext.Session.GetString("ProductoSeleccionado") != null)
                {
                    var detalleAnterior = System.Text.Json.JsonSerializer.Deserialize<List<Transaccion_Detalle>>(HttpContext.Session.GetString("ProductoSeleccionado")!);
                    detalleAnterior!.Add(new Transaccion_Detalle
                    {
                        nkey = detalleAnterior.Count + 1,
                        idVenta = idVenta.ToString(),
                        idProducto = producto.nkey,
                        descripcion = producto.descripcion,
                        cantidad = cantidad,
                        precio = producto.precio,
                        precioNeto = Math.Round(producto.precio/1.13m,5), //suponiendo que no hay descuentos
                        montoIVA = Math.Round((producto.precio * cantidad) * 0.13M,5), //IVA 13%
                        subTotal = Math.Round((producto.precio * cantidad),5)
                    });
                    //Actualizar montos en la venta
                    var venta = System.Text.Json.JsonSerializer.Deserialize<Model_Venta>(HttpContext.Session.GetString("Venta")!);
                    venta!.montoNeto += Math.Round(producto.precio / 1.13m * cantidad, 5);
                    venta.montoIVA += Math.Round((producto.precio * cantidad) * 0.13M, 5);
                    venta.montoGrav += Math.Round((producto.precio * cantidad), 5);
                    HttpContext.Session.SetString("Venta", System.Text.Json.JsonSerializer.Serialize(venta));
                    HttpContext.Session.SetString("ProductoSeleccionado", System.Text.Json.JsonSerializer.Serialize(detalleAnterior));
                }
                else
                {
                    //Primer item agregado
                    var ListDetalle = new List<Transaccion_Detalle>();
                    ListDetalle.Add(new Transaccion_Detalle
                    {
                        nkey = 1,
                        idVenta = idVenta.ToString(),
                        idProducto = idProduct,
                        descripcion = producto.descripcion,
                        cantidad = cantidad,
                        precio = producto.precio,
                        precioNeto = Math.Round(producto.precio / 1.13m, 5), //suponiendo que no hay descuentos
                        montoIVA = Math.Round((producto.precio * cantidad) * 0.13M, 5), //IVA 13%
                        subTotal = Math.Round((producto.precio * cantidad), 5)
                    });
                    //Actualizar montos en la venta
                    var venta = System.Text.Json.JsonSerializer.Deserialize<Model_Venta>(HttpContext.Session.GetString("Venta")!);
                    venta!.montoNeto += Math.Round(producto.precio / 1.13m * cantidad,5);
                    venta.montoIVA += Math.Round((producto.precio * cantidad) * 0.13M,5);
                    venta.montoGrav += Math.Round((producto.precio * cantidad),5);
                    HttpContext.Session.SetString("Venta", System.Text.Json.JsonSerializer.Serialize(venta));
                    HttpContext.Session.SetString("ProductoSeleccionado", System.Text.Json.JsonSerializer.Serialize(ListDetalle));
                }
                   
               // HttpContext.Session.SetString("Mensaje", "Item agregado correctamente"); 
                return RedirectToAction(nameof(Create));
            }
            catch(Exception ex)
            {
                HttpContext.Session.SetString("Mensaje", "Error-" + ex.Message); 
                return RedirectToAction(nameof(Create));
            }
        }
        [HttpGet]
        public async Task<ActionResult> DeleteItem(int id)
        {

            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");
            try
            {
                if (HttpContext.Session.GetString("Venta") != null)
                {
                    var venta = System.Text.Json.JsonSerializer.Deserialize<Model_Venta>(HttpContext.Session.GetString("Venta")!);
                    var detalle = System.Text.Json.JsonSerializer.Deserialize<List<Transaccion_Detalle>>(HttpContext.Session.GetString("ProductoSeleccionado")!);
                    var sales = new Sales(); 
                    var itemEliminar = detalle!.Where(x => x.nkey == id).FirstOrDefault();
                    detalle.Remove(itemEliminar!);
                    HttpContext.Session.SetString("ProductoSeleccionado", System.Text.Json.JsonSerializer.Serialize(detalle));
                    //Actualizar montos en la venta
                    venta!.montoNeto += Math.Round(detalle.Sum(x=>x.subTotal/1.13m), 2);
                    venta.montoIVA += Math.Round((detalle.Sum(x => x.subTotal * 0.13m)) * 0.13M, 2);
                    venta.montoGrav += Math.Round((detalle.Sum(x => x.subTotal)), 2);
                }
                return RedirectToAction(nameof(Create));
            }
            catch (Exception ex)
            {
                HttpContext.Session.SetString("Mensaje", "Error- al intentar eliminar la linea "+ ex.Message);
                return RedirectToAction(nameof(Create));
            }
        }
        // POST: POSController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(string idVenta)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");
            try
            {
                if (HttpContext.Session.GetString("Venta") != null)
                {
                    var venta = System.Text.Json.JsonSerializer.Deserialize<Model_Venta>(HttpContext.Session.GetString("Venta")!);
                    var detalle = System.Text.Json.JsonSerializer.Deserialize<List<Transaccion_Detalle>>(HttpContext.Session.GetString("ProductoSeleccionado")!);
                    var sales = new Sales();
                    sales.cabecera = venta!;
                    detalle.ForEach(x => x.fecha = DateTime.Now);
                    detalle.ForEach(x => x.nkey =0);
                    sales.detalle = detalle!;
                    var token = HttpContext.Session.GetString("Token");
                    //Llamar al servicio para guardar la venta
                    var result = await _service.PostVenta(token!, sales);
                    if (result.Mensaje != null)
                    {
                        HttpContext.Session.SetString("Mensaje", result.Mensaje);
                        return RedirectToAction(nameof(Create));
                    }
                    HttpContext.Session.SetString("Mensaje", "Venta creada correctamente");
                    /*Borrar todos las sessiones de venta generada*/
                    HttpContext.Session.Remove("IdVentaActual");
                    HttpContext.Session.Remove("Venta");
                    HttpContext.Session.Remove("ProductoSeleccionado");
                }
                else
                    HttpContext.Session.SetString("Mensaje", "No hay que cobrar");
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
          
        // GET: POSController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: POSController/Delete/5
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
