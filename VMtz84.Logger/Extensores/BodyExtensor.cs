using Microsoft.AspNetCore.Http;
using System.Text;

namespace VMtz84.Logger.Extensores
{
    public static class BodyExtensor
    {       
        public static async Task<string> GetBodyAsync(this HttpRequest request)
        {
            string body;

            using (StreamReader stream = new StreamReader(request.Body))
            {
                body = await stream.ReadToEndAsync();
                byte[] bytes = Encoding.UTF8.GetBytes(body);
                request.Body = new MemoryStream(bytes);
            }

            return body;
        }
    }
}