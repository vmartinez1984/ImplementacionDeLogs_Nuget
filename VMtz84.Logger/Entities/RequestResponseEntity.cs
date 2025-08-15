using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace VMtz84.Logger.Entities
{
    /// <summary>
    /// Clase base para la colección
    /// </summary>
    public class RequestResponseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string RequestBody { get; set; }
        public string RequestHeader { get; set; }
        public DateTime RequestDateRegistration { get; set; }
        public string Path { get; set; }
        public string Method { get; set; }
        public string ResponseBody { get; set; }
        public string ResponseHeader { get; set; }
        public int StatusCode { get; set; }
        public DateTime ResponseDateRegistration { get; set; }
        public string RequestId { get; set; }

        public string ApplicationName { get; set; }
        public string Curl { get; set; }
        public string Encodedkey { get; set; }
    }
}
