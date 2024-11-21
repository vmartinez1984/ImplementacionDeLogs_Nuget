using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using VMtz84.Logger.Entities;
using VMtz84.Logger.Extensores;
using VMtz84.Logger.Repositories;


namespace VMtz84.Logger.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly bool _mostrarError;
        private readonly ExceptionRepository _repository;

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
            _repository = new ExceptionRepository(configuration);
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
                RequestResponseEntity requestDtoIn = await AnalizeRequest(context);
                string eventId = Guid.NewGuid().ToString();
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                _ = _repository.AgregarAsync(new ExceptionEntity
                {
                    Exception = exception.StackTrace,
                    MessageTemplate = exception.Message,
                    EventId = eventId,
                    Application = string.Empty,
                    ConnectionId = context.Connection.Id,
                    RequestId = context.TraceIdentifier,
                    RequestPath = context.Request.Path,
                    Timestamp = DateTime.Now,
                    Body = requestDtoIn.RequestBody,
                    Header = requestDtoIn.RequestHeader,
                    Path = requestDtoIn.Path,
                    Method = requestDtoIn.Method,
                    Curl = requestDtoIn.Curl,
                });
                if (_mostrarError)
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        Mensaje = exception.Message,
                        Source = exception.Source,
                        StackTrace = exception.StackTrace,
                        InnerException = exception.InnerException,
                        EventId = eventId,
                        RequestId = context.TraceIdentifier,
                        Date = DateTime.UtcNow
                    }));
                else
                    await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                    {
                        Mensaje = "El minge ya esta reiniciado el servidor",
                        Id = eventId,
                        RequestId = context.TraceIdentifier,
                        Date = DateTime.UtcNow
                    }));
            }
        }

        /// <summary>
        /// Aqui extraemos los datos
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task<RequestResponseEntity> AnalizeRequest(HttpContext context)
        {
            RequestResponseEntity requestDtoIn;

            using (StreamReader stream = new StreamReader(context.Request.Body))
            {
                string path;
                string queryString;
                string header;
                string body;
                string method;

                path = context.Request.Path;
                queryString = context.Request.QueryString.Value;
                header = JsonConvert.SerializeObject(context.Request.Headers).Replace("[", string.Empty).Replace("]", string.Empty);
                method = context.Request.Method;
                body = await stream.ReadToEndAsync();
                requestDtoIn = new RequestResponseEntity
                {
                    RequestBody = body,
                    RequestHeader = header,
                    RequestDateRegistration = DateTime.Now,
                    Path = path + queryString,
                    Method = method,
                    Curl = context.BuildCurlCommand()
                };

                byte[] bytes = Encoding.UTF8.GetBytes(body);
                context.Request.Body = new MemoryStream(bytes);
            }

            return requestDtoIn;
        }
    }//end class
}