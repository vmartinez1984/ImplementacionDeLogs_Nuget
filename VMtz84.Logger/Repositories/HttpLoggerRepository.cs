using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using VMtz84.Logger.Entities;
using VMtz84.Logger.Models;

namespace VMtz84.Logger.Repositories
{
    public class HttpLoggerRepository
    {
        private readonly IMongoCollection<HttpLoggerEntity> _collection;
        public HttpLoggerRepository(IConfiguration configurations)
        {
            MongoClient mongoClient;
            IMongoDatabase mongoDatabase;

            var settings = configurations.GetSection("HttpLoggerMongoDb").Get<HttpLoggerSettings>();
            if (
                settings != null &&
                (
                    !string.IsNullOrEmpty(settings.ConnectionString)
                    && !string.IsNullOrEmpty(settings.MongoDbName)
                    && !string.IsNullOrEmpty(settings.CollectionName)
                )
            )
            {
                mongoClient = new MongoClient(settings.ConnectionString);
                mongoDatabase = mongoClient.GetDatabase(settings.MongoDbName);
                _collection = mongoDatabase.GetCollection<HttpLoggerEntity>(settings.CollectionName);
            }
            else
            {
                //Si no existe arrojar Exception
                throw new Exception("No esta agregado en el appsettings.js el segmento HttpLoggerMongoDb");
            }
        }

        public async Task AgregarAsync(HttpLoggerEntity entity)
        {
            try
            {
                await _collection.InsertOneAsync(entity);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }
        }
    }
}
