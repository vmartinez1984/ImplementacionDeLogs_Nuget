using System;

namespace VMtz84.Logger.Entities
{     
    public class ExceptionEntity
    {
        public DateTime Timestamp { get; set; }      
        public string MessageTemplate { get; set; }        
        public string Exception { get; set; }   
        public string EventId { get; set; }        
        public string RequestId { get; set; }
        public string RequestPath { get; set; }
        public string ConnectionId { get; set; }
        public string Application { get; set; }
        public string Body { get; internal set; }
        public string Header { get; internal set; }
        public string Path { get; internal set; }
        public string Method { get; internal set; }
        public string Curl { get; internal set; }
    }
}