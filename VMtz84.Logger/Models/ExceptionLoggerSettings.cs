namespace VMtz84.Logger.Models
{
    internal class LoggerSettings
    {
        public string ConnectionString { get; set; }
        
        public string MongoDbName { get; set; }
        
        public string ApplicationName { get; set; }

        public string CollectionNameRequests { get; set; }

        public string CollectionNameExceptions { get; set; }

        public string CollectionNameHttpLogger { get; set; }

        public bool ShowErrores { get; set; } = false;

        public string ErrorMessage { get; set; }

        public Dictionary<string, string> Headers { get; set; }
    }
}
