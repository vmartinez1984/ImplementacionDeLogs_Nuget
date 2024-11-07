using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly string _url;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _url = configuration.GetConnectionString("Utilidades");
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpDelete("Errores/{error}")]
        public IActionResult Delete(string error) { 

            throw new Exception(error);
        }

        [HttpGet("Tarjetas")]
        public async Task<IActionResult> ObtenerNumeroDeTarjeta()
        {
            using var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get,_url + "TarjetasDeDebito");
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                JObject jsonObject;

                jsonObject = JObject.Parse(await response.Content.ReadAsStringAsync());

                return Ok(new { Tarjeta = jsonObject["numeroDeTarjeta"].ToString() });
            }
            else
                throw new Exception(await response.Content.ReadAsStringAsync());
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion([FromBody]InicioDeSesion inicioDeSesion)
        {
            using var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://unam-banco.vmartinez84.xyz/api/Clientes/InicioDeSesiones");
            request.Content = new StringContent(JsonConvert.SerializeObject(inicioDeSesion), null, "application/json");
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                JObject jsonObject;

                jsonObject = JObject.Parse(await response.Content.ReadAsStringAsync());

                return Ok(new { Token = jsonObject["token"].ToString() });
            }
            else
                throw new Exception(await response.Content.ReadAsStringAsync());
        }
    }

    public class InicioDeSesion
    {
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
    }
}
