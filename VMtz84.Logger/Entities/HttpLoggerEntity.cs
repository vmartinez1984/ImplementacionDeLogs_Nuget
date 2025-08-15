using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;

namespace VMtz84.Logger.Entities
{
    public class HttpLoggerEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string RequestHeaders { get; set; }
        public string RequestUrl { get; set; }
        public string RequestBody { get; set; }

        public string ResponseBody { get; set; }
        public string ResponseHeaders { get; set; }

        public int StatusCode { get; set; }

        public DateTime FechaDeRegistro { get; set; }
        public double TiempoDeRespuesta { get;  set; }
        public string Curl { get; internal set; }
        public string ApplicationName { get; set; }
        public string Encodedkey { get; set; }
    }
}
