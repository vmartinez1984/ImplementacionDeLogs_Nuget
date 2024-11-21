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