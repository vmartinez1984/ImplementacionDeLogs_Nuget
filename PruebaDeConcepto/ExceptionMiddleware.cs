using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace PruebaDeConcepto.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly bool _mostrarError;

        /// <summary>
        /// Se incializan los servicios
        /// </summary>
        /// <param name="next"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public ExceptionMiddleware(
            RequestDelegate next,
            IConfiguration configuration
        )
        {
            this.next = next;
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
                await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                {
                    Mensaje = exception.Message,
                    Source = exception.Source,
                    StackTrace = exception.StackTrace,
                    InnerException = exception.InnerException,                    
                    RequestId = context.TraceIdentifier,
                    Date = DateTime.UtcNow
                }));
            }
        }


    }//end class
}