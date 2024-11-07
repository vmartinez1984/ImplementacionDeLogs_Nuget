using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;


namespace VMtz84.Logger.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionMiddleware> logger;
        private readonly bool _mostrarError;

        /// <summary>
        /// Se incializan los servicios
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IConfiguration configuration
        )
        {
            this.next = next;
            this.logger = logger;
            _mostrarError = false;
            bool.TryParse(configuration.GetSection("MostrarErrores").Value, out _mostrarError);
        }

        /// <summary>
        /// Aqui se cachan las excepciones y se registran el la base de datos que este configurada en el serilog
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                EventId eventId = new EventId(0, Guid.NewGuid().ToString());
                logger.LogError(eventId, exception, exception.Message);
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                if (_mostrarError)
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        Mensaje = exception.Message,
                        Source = exception.Source,
                        StackTrace = exception.StackTrace,
                        InnerException = exception.InnerException,
                        EventId = eventId.Name,
                        RequestId = context.TraceIdentifier,
                        Date = DateTime.UtcNow
                    }));
                else
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        Mensaje = "El minge ya esta reiniciado el servidor",
                        Id = eventId.Name,
                        RequestId = context.TraceIdentifier,
                        Date = DateTime.UtcNow
                    }));
            }
        }

    }//end class
}