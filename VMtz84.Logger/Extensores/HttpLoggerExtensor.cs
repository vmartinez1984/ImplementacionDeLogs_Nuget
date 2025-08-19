using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using VMtz84.Logger.Helpers;
using VMtz84.Logger.Loggers;
using VMtz84.Logger.Middlewares;

namespace VMtz84.Logger.Extensores
{
    public static class HttpLoggerExtensor
    {
        /// <summary>
        /// Se agrega el HttpLogger y el servicio de Guid para poder hacer la relación
        /// con el Resquest Response y la excepción
        /// </summary>
        /// <param name="services"></param>
        public static void AgregarHttpLoger(this IServiceCollection services)
        {
            //HttpLogger
            services.AddScoped<HttpLogger>();
            //Misma instancia durante toda la petición HTTP, pero diferente para cada petición.
            services.AddScoped<IRequestGuidService, RequestGuidService>();
                        
            //HttpClientFactory
            services.AddHttpClient(string.Empty, client => { })
                .RemoveAllLoggers().AddLogger<HttpLogger>()           
               ;
        }


        public static IApplicationBuilder UseRequestResponseExceptionYHeaders(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<HeadersMiddleware>();
            builder.UseMiddleware<ExceptionMiddleware>();
            builder.UseMiddleware<HeadersMiddleware>();

            return builder;
        }

        public static IApplicationBuilder UseRequestResponse(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<HeadersMiddleware>();            

            return builder;
        }

        public static IApplicationBuilder UseYException(this IApplicationBuilder builder)
        {            
            builder.UseMiddleware<ExceptionMiddleware>();

            return builder;
        }

        public static IApplicationBuilder UseHeaders(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<HeadersMiddleware>();

            return builder;
        }
    }
}
