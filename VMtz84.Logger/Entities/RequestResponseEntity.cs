using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

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
        public string RequestBody { get; internal set; }
        public string RequestHeader { get; internal set; }
        public DateTime RequestDateRegistration { get; internal set; }
        public string Path { get; internal set; }
        public string Method { get; internal set; }
        public string ResponseBody { get; internal set; }
        public string ResponseHeader { get; internal set; }
        public int StatusCode { get; internal set; }
        public DateTime ResponseDateRegistration { get; set; }
        public string RequestId { get; set; }

        public string ApplicationName { get; set; }
        public string Curl { get; internal set; }
    }
}
