# Implementaciones de logger

Este nuget contiene 3 componentes principales RequestResponseMiddleware, ExceptionMiddleware y HttpLoger. A continuación se describe en que consiste cada componente
Estas Implementaciones usan MongoDB como base de datos y esta construida en C#.
## RequestsResponsesMiddleware

Registra las peticiones de entrada y salida al servicio, para configurar es necesario agregar en el appsettings el segmento:

```bash
"RequestResponseMongoDb": {
    "ConnectionString": "mongodb://root:123456@192.168.1.142:27017/ClientesMs?ssl=false&authSource=admin",
    "MongoDbName": "ClientesLog",
    "CollectionName": "Peticiones",
    "ApplicationName": "ClientesMs"
}
```

Si no encuentra ninguna de las configuraciones arrojara una excepción.

En caso de que no lo agregue y haya configurado serilog con mongo tomara los datos de serilog mongo y la coleccion la colocara como RequestsResponses, en el Program.cs agregue el middleware
```bash
app.UseMiddleware<RequestResponseMiddleware>();
```

## RequestResponseMiddleware

Este middleware es para registrar las entradas y salidas al servicio, este tiene un dependencia de serilog por lo que habra que configurar en el appsettings.json

```bash
"Serilog": {
    "Using": [],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      //{
      //  "Nota":"Descomente este segmento para que escriba el log en txt",
      //  "Name": "File",
      //  "Args": {
      //    "path": "logs/log-.txt",
      //    "rollingInterval": "Day",
      //    "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
      //  }
      //},
      {
        "Name": "MongoDB",
        "Args": {
          "databaseUrl": "mongodb://root:123456@192.168.1.142:27017/ClientesMs?ssl=false&authSource=admin",
          "collectionName": "logs",
          "cappedMaxSizeMb": 1024,
          "cappedMaxDocuments": 100000
        }
      }
    ],
    "Enrich": [ "FromLogContext" ],
    "Properties": {
      "Application": "ClientesMs"
    }
  }
 ```
  y en en el Program.cs agrega el segmento como sigue:

```bash
  // Configura Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)  // Lee configuración desde appsettings.json
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

// Reemplaza el logger predeterminado por Serilog
builder.Host.UseSerilog();
//Muestra el error de serilog
//SelfLog.Enable(Console.Error);
```
Completo queda como el siguiente codigo Program.cs

```bash
using Serilog;
using VMtz84.Logger.Loggers;
using VMtz84.Logger.Middlewares;
var builder = WebApplication.CreateBuilder(args);
// Configura Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)  // Lee configuración desde appsettings.json
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

// Reemplaza el logger predeterminado por Serilog
builder.Host.UseSerilog();
//Muestra el error de serilog
//SelfLog.Enable(Console.Error);

// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

//Middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

// Asegúrate de cerrar el logger al final del programa
Log.CloseAndFlush();
```bash

