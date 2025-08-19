# Implementaciones de logger

Este nuget contiene 3 componentes principales RequestResponseMiddleware, ExceptionMiddleware y HttpLoger. A continuación se describe en que consiste cada componente
Estas Implementaciones usan MongoDB como base de datos y esta construida en C#.

## HttpLogger
El http loger intercepta la petición que se envia a traves del HppClientFactory, para agregar en el log

En el program agregue
```bash
builder.Services.AgregarHttpLoger();
```

Cuando haga la petición mediante el factory, agregue en el contructor de su servicio
```bash
private readonly IRequestGuidService _requestGuidService;

public Servicio(IRequestGuidService requestGuidService)
{
    _requestGuidService = requestGuidService
}
```

Agregue en el factory
```bash
using VMtz84.Logger.Extensores;

request.AgregarHeaderDeEncodedKey(_requestGuidService);
```

Para que quede como el siguiente fragmento
```bash
 using (var client = _clientFactory.CreateClient())
{
    var request = new HttpRequestMessage(HttpMethod.Get, _url);
    request.Headers.Add("encodedkey", _requestGuidService.Encodedkey);
    var response = await client.SendAsync(request);
    if (response.IsSuccessStatusCode)
    {
        JObject jObject;

        jObject = JObject.Parse(await response.Content.ReadAsStringAsync());

        return Ok(new { Tarjeta = jObject["numeroDeTarjeta"].ToString() });
    }
    else
        throw new Exception(await response.Content.ReadAsStringAsync());
}
```

Asi el mismo guid queda registrado en los Middleware y pueda hacer la realación

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

```bash
<PackageReference Include="MongoDB.Driver" Version="3.0.0" />
```

Quitelo limpie la solución, compile y pruebe de nuevo

### Todo
- Agregar controller test a peticón de configuración
- Simplificar la confiración del appsettings
- Headers personalizados en el appsetiings