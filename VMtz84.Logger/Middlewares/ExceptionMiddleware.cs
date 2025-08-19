using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using VMtz84.Logger.Entities;
using VMtz84.Logger.Extensores;
using VMtz84.Logger.Helpers;
using VMtz84.Logger.Repositories;


namespace VMtz84.Logger.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly bool _mostrarError;
        private readonly string _mensajeDeError;
        private readonly IConfiguration _configuration;

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
            _mensajeDeError = configuration.GetSection("MensajeDeError").Value;
            _configuration = configuration;
        }

        /// <summary>
        /// Aqui se cachan las excepciones y se registran el la base de datos que este configurada en el serilog
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, IRequestGuidService requestGuidService)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                var _repository = new ExceptionRepository(_configuration);
                string encodedkey = requestGuidService.Encodedkey;                
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                _ = _repository.AgregarAsync(new ExceptionEntity
                {
                    Encodedkey = encodedkey,
                    Exception = exception.StackTrace,
                    MessageTemplate = exception.Message,                    
                    Application = string.Empty,
                    ConnectionId = context.Connection.Id,
                    RequestId = context.TraceIdentifier,
                    RequestPath = context.Request.Path,
                    Timestamp = DateTime.Now,
                    Body = await context.Request.GetBodyAsync(),
                    Header = JsonConvert.SerializeObject(context.Request.Headers).Replace("[", string.Empty).Replace("]", string.Empty),
                    Path = context.Request.Path,
                    Method = context.Request.Method,
                    Curl = context.BuildCurlCommand()
                });
                if (_mostrarError)
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        Mensaje = exception.Message,
                        Source = exception.Source,
                        StackTrace = exception.StackTrace,
                        InnerException = exception.InnerException,
                        EventId = encodedkey,
                        RequestId = context.TraceIdentifier,
                        Date = DateTime.UtcNow
                    }));
                else
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        Mensaje = string.IsNullOrEmpty(_mensajeDeError) ? "El minge ya esta reiniciado el servidor" : _mensajeDeError,
                        Encodedkey = encodedkey,                        
                        Date = DateTime.UtcNow
                    }));
            }
        }


    }//end class
}