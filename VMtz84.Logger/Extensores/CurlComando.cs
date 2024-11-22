using Microsoft.AspNetCore.Http;
using System.Text;

namespace VMtz84.Logger.Extensores
{
    public static class CurlComando
    {
        public static string BuildCurlCommand(this HttpContext context)
        {
            var request = context.Request;
            var curlCommand = new StringBuilder("curl");

            // Agrega el método HTTP
            curlCommand.Append($" -X {request.Method}");

            // Agrega las cabeceras
            foreach (var header in request.Headers)
            {
                curlCommand.Append($" -H \"{header.Key}: {header.Value}\"");
            }

            // Agrega el URL
            var url = $"{request.Scheme}://{request.Host}{request.Path}{request.QueryString}";
            curlCommand.Append($" \"{url}\"");

            // Agrega el cuerpo de la solicitud (si existe y es un método que permite cuerpo, como POST o PUT)
            if (request.ContentLength > 0 && (request.Method == "POST" || request.Method == "PUT" || request.Method == "PATCH"))
            {
                request.Body.Position = 0;
                using var reader = new StreamReader(request.Body);
                var body = reader.ReadToEnd();
                curlCommand.Append($" -d \"{body}\"");
            }

            return curlCommand.ToString();
        }

        public static string GenerateCurlCommand(this HttpRequestMessage request)
        {
            var curlCommand = new StringBuilder("curl");

            // Agregar el método HTTP
            curlCommand.Append($" -X '{request.Method}' \\\n");

            // Agregar la URL de la solicitud
            curlCommand.Append($"'{request.RequestUri}' \\\n");

            // Agregar las cabeceras
            //foreach (var header in request.Headers)
            //{
            //    curlCommand.Append($" -H \"{header.Key}: {string.Join(", ", header.Value)}\"");
            //}

            if (request.Content != null)
            {
                // Agregar las cabeceras del contenido (por ejemplo, Content-Type)
                foreach (var header in request.Content.Headers)
                {
                    if (header.Key != "Content-Length")
                        curlCommand.Append($" -H '{header.Key}: {string.Join(", ", header.Value)}' \\\n");
                }

                // Agregar el contenido del cuerpo
                var content = request.Content.ReadAsStringAsync().Result;
                if (!string.IsNullOrEmpty(content))
                {
                    curlCommand.Append($" -d '{content}'");
                }
            }

            return curlCommand.ToString();
        }
    }
}
