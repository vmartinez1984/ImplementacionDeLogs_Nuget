using Microsoft.Extensions.DependencyInjection;
using VMtz84.Logger.Helpers;
using VMtz84.Logger.Loggers;

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

            //services.AddScoped<RequestGuidDelegatingHandler>();
            //HttpClientFactory
            services.AddHttpClient(string.Empty, client => { })
                .RemoveAllLoggers().AddLogger<HttpLogger>()
            //    .AddHttpMessageHandler<RequestGuidDelegatingHandler>()
            //    .Services.AddScoped<IRequestGuidService, RequestGuidService>();
               ;
        }
    }
}
