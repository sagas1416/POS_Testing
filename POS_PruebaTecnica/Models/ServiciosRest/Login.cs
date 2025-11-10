using System.Threading.Tasks;

namespace POS_PruebaTecnica.Models.ServiciosRest
{
    public class Login
    {
        public class RequestLogin
        {
            public string? usuario { get; set; }
            public int id { get; set; }
            public bool activo { get; set; }
            public string? Token { get; set; }
            public int? Tipo { get; set; }
            public string? Mensaje { get; set; }
        }
        internal async Task<RequestLogin> getTokenUser(string User, string pass)
        { 
            
            /*
           curl -X 'GET' \
          'https://localhost:7171/api/Login/sagas1416&123456' \
          -H 'accept: text/plain'
           */
            /*Obtener token desde api en */
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7171/");
            client.DefaultRequestHeaders.Accept.Clear();    
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
            var response = await client.GetAsync($"api/Login/{User}&{pass}");
            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadFromJsonAsync<RequestLogin>();
                return resultado!;
            }
            else
            {
                return new RequestLogin
                {
                    Token = null,
                    Mensaje = "Error en la comunicación con el servicio" + response.StatusCode+ " - " + response.ReasonPhrase
                };

            }
        }
    }
}
