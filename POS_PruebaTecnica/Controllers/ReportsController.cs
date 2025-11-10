using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using POS_PruebaTecnica.Models.Reports;
using POS_PruebaTecnica.Models.ServiciosRest;
using System.Threading.Tasks;

namespace POS_PruebaTecnica.Controllers
{
    public class ReportsController : Controller
    {
        private readonly Reportes_Service _service;
        private readonly GenerarExcel _excel;
        public ReportsController(Reportes_Service service,GenerarExcel excel)
        {
            _service = service;
            _excel = excel;
        }
        public IActionResult Index()
        {

            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");
            if (HttpContext.Session.GetString("Mensaje") != null)
            {
                ViewBag.Mensaje = HttpContext.Session.GetString("Mensaje");
                HttpContext.Session.Remove("Mensaje");
            }
            return View();
        }

        public IActionResult ExportPDF(DateTime fechaIni, DateTime fechaFin, int tipo)
        {


            return View();
        }

        public async Task<IActionResult> ExportExcel(DateTime fechaIni, DateTime fechaFin, int tipo)
        {
            if (HttpContext.Session.GetString("Usuario") == null)
                return RedirectToAction("Index", "Login");
            try
            {
                string token = HttpContext.Session.GetString("Token") ?? "";
                var reportes = await _service.getReport(token, fechaIni, fechaFin, tipo);
                if (reportes[0].Mensaje == null)
                {
                    byte[] fileContents = Array.Empty<byte>();
                    if (tipo == 1)
                        fileContents = await _excel.GenerarExcelDetallado(reportes);
                    else
                        fileContents = await _excel.GenerarExcelConsolidado(reportes);

                    return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte.xlsx");
                }
                else
                {
                    HttpContext.Session.SetString("Mensaje", "Error-" + reportes[0].Mensaje);
                    return RedirectToAction(nameof(Index));

                }
            }
            catch (Exception ex)
            {

                HttpContext.Session.SetString("Mensaje", "Error-" + ex.Message);
                return RedirectToAction(nameof(Index));
            }

        }
    }
}
