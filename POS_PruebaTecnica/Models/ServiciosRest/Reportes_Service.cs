using POS_PruebaTecnica.Models.Reports;

namespace POS_PruebaTecnica.Models.ServiciosRest
{
    public class Reportes_Service
    {
        string baseurl = "https://localhost:7171/api/";

        public async Task<List<Model_Report>> getReport(string token,DateTime fechaIni, DateTime fechaFin, int tipo)
        {
            /*
             curl -X 'GET' \
              'https://localhost:7171/api/Report/11-09-2025,11-09-2025,2' \
              -H 'accept: text/plain' \
              -H 'Authorization: bea
             */
            var listaReportes = new List<Model_Report>();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri(baseurl);
            var response = await client.GetAsync($"Report/{fechaIni.ToString("MM-dd-yyyy")},{fechaFin.ToString("MM-dd-yyyy")},{tipo}");
            if (response.IsSuccessStatusCode)
            {
                var resultado = response.Content.ReadFromJsonAsync<List<Model_Report>>();
                listaReportes = resultado.Result!;
                return listaReportes;
            }
            else
            {
                var resultadoError = await response.Content.ReadAsStringAsync();
                /*Mandar resultado al controlador */
                listaReportes.Add(new Model_Report { Mensaje = "Error- al obtener el reporte: " + resultadoError });
                return listaReportes;
            }
        }
    }
}
