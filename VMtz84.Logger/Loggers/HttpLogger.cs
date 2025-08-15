using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http.Logging;
using Microsoft.Extensions.Logging;
using VMtz84.Logger.Entities;
using VMtz84.Logger.Extensores;
using VMtz84.Logger.Helpers;
using VMtz84.Logger.Repositories;

namespace VMtz84.Logger.Loggers
{
    public class HttpLogger : IHttpClientLogger
    {
        private readonly ILogger<HttpLogger> _logger;
        private readonly IRequestGuidService _requestGuidService;
        private readonly HttpLoggerRepository _repository;

        public HttpLogger(ILogger<HttpLogger> logger, IConfiguration configuration, IRequestGuidService requestGuidService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._requestGuidService = requestGuidService;
            _repository = new HttpLoggerRepository(configuration);
        }

        public object? LogRequestStart(HttpRequestMessage request)
        {            
            return null;
        }

        public void LogRequestStop(
            object? context, HttpRequestMessage request, HttpResponseMessage response, TimeSpan elapsed)
        {            
            HttpLoggerEntity httpLoggerEntity;            
            string encodedkey = request.Headers.Where(x => x.Key == "encodedkey").FirstOrDefault().Value == null ? string.Empty: request.Headers.Where(x => x.Key == "encodedkey").FirstOrDefault().Value.FirstOrDefault();

            httpLoggerEntity = new HttpLoggerEntity
            {
                Encodedkey = encodedkey,                
                FechaDeRegistro = DateTime.Now,
                RequestBody = request.Content is null ? null : request.Content?.ReadAsStringAsync().Result,
                RequestUrl = request.RequestUri.ToString(),
                RequestHeaders = request.Headers.ToString(),
                ResponseBody = response.Content is null ? null : response.Content?.ReadAsStringAsync().Result,
                ResponseHeaders = response.Headers.ToString(),
                StatusCode = (int)response.StatusCode,
                TiempoDeRespuesta = elapsed.TotalMilliseconds,                
                Curl = request.GenerateCurlCommand()
            };            
            Task.Run(async () =>
            {                
                await _repository.AgregarAsync(httpLoggerEntity);
            });
        }

        public void LogRequestFailed(
            object? context,
            HttpRequestMessage request,
            HttpResponseMessage? response,
            Exception exception,
            TimeSpan elapsed)
        {
            _logger.LogError(
                exception,
                "Request towards '{Request.Host}{Request.Path}' failed after {Response.ElapsedMilliseconds}ms",
                request.RequestUri?.GetComponents(UriComponents.SchemeAndServer, UriFormat.Unescaped),
                request.RequestUri!.PathAndQuery,
                elapsed.TotalMilliseconds.ToString("F1"));
        }
    }
}