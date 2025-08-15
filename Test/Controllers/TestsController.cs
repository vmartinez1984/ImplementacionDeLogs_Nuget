using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using VMtz84.Logger.Helpers;

namespace Test.Controllers
{
    /// <summary>
    /// Controller para pruebas
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestsController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<TestsController> logger;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IRequestGuidService _requestGuidService;
        private readonly string _url;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public TestsController(
            IConfiguration configuration
            , IHttpClientFactory clientFactory
            , ILogger<TestsController> logger
            , IRequestGuidService requestGuidService
        )
        {
            this.configuration = configuration;
            this.logger = logger;
            _clientFactory = clientFactory;
            _url = configuration.GetConnectionString("OxxoMs");
            _requestGuidService = requestGuidService;
        }

        /// <summary>
        /// Obtiene el ambiente que tiene registrado en el appSettings
        /// </summary>
        /// <returns></returns>
        [HttpGet("Ambiente")]
        public IActionResult Get()
        {
            string ambiente = configuration.GetSection("Ambiente").Value;
            logger.LogInformation($"Ambiente: {ambiente}");

            return Ok(new { Ambiente = ambiente });
        }

        /// <summary>
        /// Prueba para registrar errores
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpDelete("Errores")]
        public IActionResult Delete(string error)
        {
            if (string.IsNullOrEmpty(error))
                error = Guid.NewGuid().ToString();
            throw new Exception(error);
        }

        /// <summary>
        /// Hola mundo
        /// </summary>
        /// <returns></returns>
        [HttpGet("HolaMundo")]
        public IActionResult HolaMundo(string saludo)
        {
            if (string.IsNullOrEmpty(saludo))
                saludo = "Hola mundo";

            return Ok(new { Mensaje = saludo, Fecha = DateTime.Now, Id = Guid.NewGuid() });
        }

        /// <summary>
        /// Hola mundo
        /// </summary>
        /// <returns></returns>
        [HttpPost("HolaMundo")]
        public IActionResult HolaMundo([FromBody] HolaMundoDtoIn saludo)
        {
            if (string.IsNullOrEmpty(saludo.Saludo))
                saludo.Saludo = "Hola mundo";

            return Ok(new { saludo.Saludo, Fecha = DateTime.Now, Id = Guid.NewGuid() });
        }

        [HttpPost("HttpLogger/{numero}")]
        public async Task<IActionResult> GetTarjetaAsync(int numero)
        {

            using (var client = _clientFactory.CreateClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, _url);
                request.Headers.Add("encodedkey", _requestGuidService.Encodedkey);
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


    public class HolaMundoDtoIn
    {
        public string Saludo { get; set; }
    }
}
