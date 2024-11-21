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

En el Program.cs agregue el middleware
```bash
app.UseMiddleware<RequestResponseMiddleware>();
```

## ExceptionMiddleware

Este middleware es para registrar las excepciones, este tiene un dependencia de serilog por lo que habra que configurar en el appsettings.json

```bash
"ExceptionLoggerMongoDb": {
    "ConnectionString": "mongodb://root:123456@localhost:27017/Logs?ssl=false&authSource=admin",
    "MongoDbName": "LogsDev",
    "CollectionName": "ExceptionLogger",
    "ApplicationName": "TestDev"
}
 ```
  y en en el Program.cs agrega el segmento como sigue:

```bash
app.UseMiddleware<ExceptionMiddleware>();
```
Completo queda como el siguiente codigo Program.cs

```bash
using VMtz84.Logger.Loggers;
using VMtz84.Logger.Middlewares;

var builder = WebApplication.CreateBuilder(args);

//HttpLogger
builder.Services.AddScoped<HttpLogger>();

//HttpClientFactory
builder.Services.AddHttpClient(string.Empty, client => { }).RemoveAllLoggers().AddLogger<HttpLogger>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<RequestResponseMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
```

### Nota
En Ocaciones no escribe el log, verifique y que no este el nugget

<PackageReference Include="MongoDB.Driver" Version="3.0.0" />

Quitelo limpie la solución, compile y pruebe de nuevo

### Todo
- Agregar controller test a peticón de configuración
