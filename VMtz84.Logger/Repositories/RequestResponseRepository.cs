using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Xml.Linq;
using VMtz84.Logger.Entities;
using VMtz84.Logger.Helpers;
using VMtz84.Logger.Models;

namespace VMtz84.Logger.Repositories
{
    /// <summary>
    /// Repositorio para mongoDb Donde se registrara la peticion
    /// </summary>
    public class RequestResponseRepository
    {
        private readonly IMongoCollection<RequestResponseEntity> _collection;
        private readonly string _name;        

        /// <summary>
        /// Colocamos la cadena de conexión, inserción a la db mongo
        /// </summary>
        /// <param name="configurations"></param>
        public RequestResponseRepository(IConfiguration configurations)
        {
            MongoClient mongoClient;
            IMongoDatabase mongoDatabase;

            var settings = configurations.GetSection("RequestResponseMongoDb").Get<RequestResponseSettings>();
            _name = settings.ApplicationName;
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
                _collection = mongoDatabase.GetCollection<RequestResponseEntity>(settings.CollectionName);
            }
            else
            {
                //Si no existe arrojar Exception
                throw new Exception("No esta agregado en el appsettings.js el segmento ExceptionLoggerMongoDb");
            }
            
        }

        /// <summary>
        /// Se agregan los datos de la petición
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task AgregarAsync(RequestResponseEntity entity)
        {
            try
            {
                entity.ApplicationName = _name;                
                await _collection.InsertOneAsync(entity);
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
            }
        }
    }
}
