using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using VMtz84.Logger.Entities;
using VMtz84.Logger.Helpers;
using VMtz84.Logger.Models;

namespace VMtz84.Logger.Repositories
{
    internal class ExceptionRepository
    {
        private readonly IMongoCollection<ExceptionEntity> _collection;
        private readonly string _applicationName;        

        public ExceptionRepository(IConfiguration configurations)
        {
            MongoClient mongoClient;
            IMongoDatabase mongoDatabase;
            
            var settings = configurations.GetSection("LoggerMongoDb").Get<LoggerSettings>();
            if (
                settings != null &&
                (
                    !string.IsNullOrEmpty(settings.ConnectionString)
                    && !string.IsNullOrEmpty(settings.MongoDbName)
                    && !string.IsNullOrEmpty(settings.CollectionNameHttpLogger)
                )
            )
            {
                mongoClient = new MongoClient(settings.ConnectionString);
                mongoDatabase = mongoClient.GetDatabase(settings.MongoDbName);
                _collection = mongoDatabase.GetCollection<ExceptionEntity>(settings.CollectionNameExceptions);
                _applicationName = settings.ApplicationName;
            }
            else
            {
                //Si no existe arrojar Exception
                throw new Exception("No esta agregado en el appsettings.js la configuracón LoggerMongoDb");
            }            
        }

        /// <summary>
        /// Se agregan los datos de la petición
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task AgregarAsync(ExceptionEntity entity)
        {
            try
            {
                entity.Application = _applicationName;                
                await _collection.InsertOneAsync(entity);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }
        }
    }
}
