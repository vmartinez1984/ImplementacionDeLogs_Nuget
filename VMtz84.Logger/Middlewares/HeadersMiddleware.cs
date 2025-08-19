using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Text;
using VMtz84.Logger.Entities;
using VMtz84.Logger.Extensores;
using VMtz84.Logger.Helpers;
using VMtz84.Logger.Models;
namespace VMtz84.Logger.Middlewares
{
    /// <summary>
    /// Agregar header personalizados
    /// </summary>
    public class HeadersMiddleware
    {
        private RequestDelegate _next;

        private readonly IConfiguration _configuration;
        private readonly LoggerSettings _loggerSettings;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="next"></param>
        /// <param name="configuration"></param>
        public HeadersMiddleware(
            RequestDelegate next,
            IConfiguration configuration
        )
        {
            _next = next;
            _configuration = configuration;
            // Agregaremos los header que se coloquen en el appsettings
            _loggerSettings = configuration.GetSection("LoggerMongoDb").Get<LoggerSettings>();
        }

        /// <summary>
        /// Agregamos headers personalizados
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, IRequestGuidService requestGuidService)
        {
            if (context.Request.Headers.TryGetValue("encodedkey", out StringValues encodedkey))
            {
                if (requestGuidService.Encodedkey != encodedkey)
                    requestGuidService.SetEncodedkey(encodedkey);
            }
            else
            {
                context.Request.Headers.Add("encodedkey", requestGuidService.Encodedkey);
            }
            //Agregar los headers
            foreach (var item in _loggerSettings.Headers)
            {
                if (!context.Response.Headers.TryGetValue(item.Key, out StringValues header))
                    context.Response.Headers.Add(item.Key, item.Value);
            }

            // Call the next middleware
            await _next(context);
        }
    }
}
