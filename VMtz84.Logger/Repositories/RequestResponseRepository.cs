using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Linq;
using System.Threading.Tasks;
using VMtz84.Logger.Entities;
using VMtz84.Logger.Models;

namespace VMtz84.Logger.Repositories
{
    /// <summary>
    /// Repositorio para mongoDb Donde se registrara la peticion
    /// </summary>
    public class RequestResponseRepository
    {
        private readonly IMongoCollection<RequestResponseEntity> _collection;

        /// <summary>
        /// Colocamos la cadena de conexión, inserción a la db mongo
        /// </summary>
        /// <param name="configurations"></param>
        public RequestResponseRepository(IConfiguration configurations)
        {
            MongoClient mongoClient;
            IMongoDatabase mongoDatabase;
            
            var settings = configurations.GetSection("RequestResponseMongoDb").Get<RequestResponseSettings>();
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
                //Si no existe leer serilog y si no existe arrojar Exception
                string connetionString = configurations.GetSection("Serilog:WriteTo")
                    .GetChildren()
                    .FirstOrDefault(x => x["Name"] == "MongoDB")?["Args:databaseUrl"];
                if (string.IsNullOrEmpty(connetionString))
                    throw new Exception("No hay configuración para el loggeo de Requests y Responses");
                mongoClient = new MongoClient(connetionString);
                //Obtenermos el ultimo segmento para tener el nombre de la base de datos
                mongoDatabase = mongoClient.GetDatabase(connetionString.Split('/').Last());
                _collection = mongoDatabase.GetCollection<RequestResponseEntity>("RequestsResponses");
            }           
        }

        /// <summary>
        /// Se agregan los datos de la petición
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<string> AgregarAsync(RequestResponseEntity entity)
        {
            try
            {
                await _collection.InsertOneAsync(entity);

                return entity._id;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
