namespace VMtz84.Logger.Models
{
    internal class RequestResponseSettings
    {
        public string ConnectionString { get; set; }
        public string MongoDbName { get; set; }
        public string MongoDbVersion { get; set; }
        public string CollectionName { get; set; }
        public string ApplicationName { get; set; }
    }
}
