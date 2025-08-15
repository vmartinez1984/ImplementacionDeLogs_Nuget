

namespace VMtz84.Logger.Helpers
{
    public interface IRequestGuidService
    {
        string Encodedkey { get; }
    }

    public class RequestGuidService : IRequestGuidService
    {
        private static RequestGuidService _instance;

        public RequestGuidService()
        {
            Encodedkey = Guid.NewGuid().ToString();
        }

        public string Encodedkey { get; } 

        public static RequestGuidService GetInstance()
        {
            if (_instance == null)
            {
                _instance = new RequestGuidService();                
            }
            return _instance;
        }
    }

}
