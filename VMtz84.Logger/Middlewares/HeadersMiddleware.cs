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
                context.Request.Headers.Add("encodedkey", encodedkey);
                requestGuidService.SetEncodedkey(encodedkey);
            }
            else
            {
                context.Request.Headers.Add("encodedkey", requestGuidService.Encodedkey);
            }

            // Call the next middleware
            await _next(context);
        }       
    }
}
