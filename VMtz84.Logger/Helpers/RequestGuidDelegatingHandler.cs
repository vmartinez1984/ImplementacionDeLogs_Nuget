using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VMtz84.Logger.Helpers
{
    public class RequestGuidDelegatingHandler : DelegatingHandler
    {
        private readonly IRequestGuidService _requestGuidService;

        public RequestGuidDelegatingHandler(IRequestGuidService requestGuidService)
        {
            _requestGuidService = requestGuidService;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Agrega el GUID como header personalizado
            request.Headers.Add("X-Request-Guid", _requestGuidService.Encodedkey);
            return base.SendAsync(request, cancellationToken);
        }
    }
}