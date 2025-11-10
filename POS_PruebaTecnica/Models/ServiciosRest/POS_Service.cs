using Microsoft.VisualBasic;
using NuGet.Common;
using POS_PruebaTecnica.Models.POS;

namespace POS_PruebaTecnica.Models.ServiciosRest
{
    public class POS_Service
    {
        string baseurl = "https://localhost:7171/api/";

        public List<Sales> GetVentasHeader(string Token)
        {
           var model = new List<Sales>();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            client.BaseAddress = new Uri(baseurl);
            var response =  client.GetAsync("SalesPOS");
            if (response.Result.IsSuccessStatusCode)
            {
                var resultado = response.Result.Content.ReadFromJsonAsync< List<Sales>>();
                model = resultado.Result!;
                return model;
            }
            else
            {
                var resultadoError = response.Result.Content.ReadAsStringAsync();
                /*Mandar resultado al controlador */
                model[0].Mensaje = "Error- al obtener el header de la venta: " + resultadoError.Result;
                return model;
            }
             

        } 
        internal async Task<Sales> getVenta(string Token, string id)
        {
            var model = new Sales();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            client.BaseAddress = new Uri(baseurl);
            var response = client.GetAsync("SalesPOS/"+id);
            if (response.Result.IsSuccessStatusCode)
            {
                var resultado = response.Result.Content.ReadFromJsonAsync<Sales>();
                model = resultado.Result!;
                return model;
            }
            else
            {
                var resultadoError = response.Result.Content.ReadAsStringAsync();
                /*Mandar resultado al controlador */
                model.Mensaje = "Error- al obtener la venta: " + resultadoError.Result;
                return model;
            }
        }
        internal async Task<Sales> PostVenta(string Token, Sales venta)
        {
            var model = new Sales();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            client.BaseAddress = new Uri(baseurl);
            var response = await client.PostAsJsonAsync("SalesPOS", venta);
            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadFromJsonAsync<Sales>();
                model = resultado!;
                return model;
            }
            else
            {
                var resultadoError = await response.Content.ReadAsStringAsync();
                /*Mandar resultado al controlador */
                model.Mensaje = "Error- al guardar la venta: " + resultadoError;
                return model;
            }
        }
    }
}
