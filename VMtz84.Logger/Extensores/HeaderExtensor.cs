using VMtz84.Logger.Helpers;

namespace VMtz84.Logger.Extensores
{
    public static class HeaderExtensor
    {
        public static HttpRequestMessage AgregarHeaderDeEncodedKey(this HttpRequestMessage request, IRequestGuidService _requestGuidService)
        {
            request.Headers.Add("encodedkey", _requestGuidService.Encodedkey);

            return request;
        }
    }
}
