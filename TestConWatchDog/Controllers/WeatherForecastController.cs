using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VMtz84.Logger.Helpers;
using VMtz84.Logger.Extensores;

namespace TestConWatchDog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IRequestGuidService _requestGuidService;
        private readonly string _url;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger, 
            IConfiguration configuration, 
            IHttpClientFactory clientFactory,
            IRequestGuidService requestGuidService
        )
        {
            _logger = logger;
            _clientFactory = clientFactory;
            _url = configuration.GetConnectionString("OxxoMs");
            _requestGuidService = requestGuidService;
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

        [HttpPost("HttpLogger/{numero}")]
        public async Task<IActionResult> GetTarjetaAsync(int numero)
        {

            using (var client = _clientFactory.CreateClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, _url);
                request.AgregarHeaderDeEncodedKey(_requestGuidService);
                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    JObject jObject;

                    jObject = JObject.Parse(await response.Content.ReadAsStringAsync());

                    return Ok(new { Tarjeta = jObject["numeroDeTarjeta"].ToString() });
                }
                else
                    throw new Exception(await response.Content.ReadAsStringAsync());
            }
        }
    }
}
