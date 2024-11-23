using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VMtz84.Logger.Entities;
using VMtz84.Logger.Repositories;
using VMtz84.Logger.Extensores;

namespace VMtz84.Logger.Loggers
{
    public class HttpLogger : IHttpClientLogger
    {
        private readonly ILogger<HttpLogger> _logger;
        private readonly HttpLoggerRepository _repository;

        public HttpLogger(ILogger<HttpLogger> logger, IConfiguration configuration)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            httpLoggerEntity = new HttpLoggerEntity
            {
                FechaDeRegistro = DateTime.Now,
                RequestBody = request.Content is null ? null : request.Content.ReadAsStringAsync().Result,
                RequestUrl = request.RequestUri.ToString(),
                RequestHeaders = request.Content is null ? null : request.Content?.Headers.ToString().Trim(),
                ResponseBody = response.Content is null ? null : response.Content?.ReadAsStringAsync().Result,
                ResponseHeaders = response.Content is null || response.Content?.Headers is null ? null : response.Content?.Headers?.ToString().Trim(),
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
