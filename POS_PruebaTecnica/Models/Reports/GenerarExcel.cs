using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace POS_PruebaTecnica.Models.Reports
{
    public class GenerarExcel
    {
        public async Task<byte[]> GenerarExcelDetallado(List<Model_Report> reports)
        {
            var datos = reports;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Reporte Detallado");

            // Encabezados
            var headers = new string[]
            {
                "Fecha", "id", "tipo", "cantidad",
                "descripcion", "precio", "montoNeto", "montoIVA",
                "montoGrav"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }

            // Datos
            int row = 2;
            foreach (var item in datos)
            {
                worksheet.Cells[row, 1].Value = item.fecha.ToString("dd/MM/yyyy");
                worksheet.Cells[row, 2].Value = item.id;
                worksheet.Cells[row, 3].Value = item.tipo;
                worksheet.Cells[row, 4].Value = item.cantidad;
                worksheet.Cells[row, 5].Value = item.descripcion;
                worksheet.Cells[row, 6].Value = item.precio;
                worksheet.Cells[row, 7].Value = item.montoNeto;
                worksheet.Cells[row, 8].Value = item.montoIVA;
                worksheet.Cells[row, 9].Value = item.montoGrav; 
                row++;
            }

            worksheet.Cells.AutoFitColumns();

            // Exportar como archivo descargable
            var stream = new MemoryStream();
            await package.SaveAsAsync(stream);
            stream.Position = 0; 
            return stream.ToArray();
        } 
        public async Task<byte[]> GenerarExcelConsolidado(List<Model_Report> reports)
        {
            var datos = reports;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Reporte Consolidado");

            // Encabezados
            var headers = new string[]
            {
                "Fecha", "tipo", "montoNeto", "montoIVA","montoGrav"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }

            // Datos
            int row = 2;
            foreach (var item in datos)
            {
                worksheet.Cells[row, 1].Value = item.fecha.ToString("dd/MM/yyyy"); 
                worksheet.Cells[row, 3].Value = item.tipo;   
                worksheet.Cells[row, 7].Value = item.montoNeto;
                worksheet.Cells[row, 8].Value = item.montoIVA;
                worksheet.Cells[row, 9].Value = item.montoGrav; 
                row++;
            }

            worksheet.Cells.AutoFitColumns();

            // Exportar como archivo descargable
            var stream = new MemoryStream();
            await package.SaveAsAsync(stream);
            stream.Position = 0; 
            return stream.ToArray();
        } 
    }
}
