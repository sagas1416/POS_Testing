using POS_PruebaTecnica.Models.Productos;
namespace POS_PruebaTecnica.Models.ServiciosRest
{
    public class Producto_Service
    {
        string baseurl = "https://localhost:7171/api/";
        public async Task<List<Model_Productos>> GetProductos(string token)
        { 
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            client.BaseAddress = new Uri(baseurl+ "Productos");
            var response = await client.GetAsync("Productos");
            if (response.IsSuccessStatusCode)
            {
                var resultado = await response.Content.ReadFromJsonAsync<List<Model_Productos>>();
                return resultado!;
            }
            else
            {
                return new List<Model_Productos>();
            }

        }
        public async Task<Model_Productos> GetProducto(string token, int id)
        { 
            var producto = new Model_Productos();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            client.BaseAddress = new Uri(baseurl);
            var response = await client.GetAsync("Productos/"+id);
            if (response.IsSuccessStatusCode)
            {
                producto = await response.Content.ReadFromJsonAsync<Model_Productos>();
                return producto!;
            }
            else
            {
                var resultadoError = await response.Content.ReadAsStringAsync();
                /*Mandar resultado al controlador */
                producto.Mensaje = "Error- al obtener el producto: " + resultadoError;
                return producto;
            }

        }
        public async Task<Model_Productos> GetProductoCode(string token, string code)
        { 
            var producto = new Model_Productos();
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            client.BaseAddress = new Uri(baseurl);
            var response = await client.GetAsync("Productos/"+code+",1");
            if (response.IsSuccessStatusCode)
            {
                producto = await response.Content.ReadFromJsonAsync<Model_Productos>();
                return producto!;
            }
            else
            {
                var resultadoError = await response.Content.ReadAsStringAsync();
                /*Mandar resultado al controlador */
                producto.Mensaje = "Error- al obtener el producto: " + resultadoError;
                return producto;
            }

        }
        //nuevo producto 
        public async Task<bool> CrearProducto(Model_Productos producto, string token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri(baseurl + "Productos");
            var response = await client.PostAsJsonAsync("Productos", producto);
            return response.IsSuccessStatusCode;
        }

        internal async Task<string> ActualizarProducto(Model_Productos producto, string? token)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/plain"));
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            client.BaseAddress = new Uri(baseurl);
            var response = client.PutAsJsonAsync("Productos/" + producto.nkey, producto);
            if (response.Result.IsSuccessStatusCode)
            {
                return "exito- Producto actualizado exitosamente";
            }
            else
            {
                var resultadoError = await response.Result.Content.ReadAsStringAsync();
                return "Error- al actualizar el producto: " + resultadoError;
            }
        }
    }
}
