using WatchDog;
using WatchDog.src.Enums;
using VMtz84.Logger.Extensores;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AgregarHttpLoger();

//Start watchdog
builder.Services.AddWatchDogServices(opt =>
{
    //opt.IsAutoClear = true;
    //opt.ClearTimeSchedule = WatchDogAutoClearScheduleEnum.Monthly;
    opt.SetExternalDbConnString = builder.Configuration.GetConnectionString("WatchDog");
    opt.DbDriverOption = WatchDogDbDriverEnum.Mongo;
});
//End watchdog

builder.Logging.AddWatchDogLogger();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseHeaders();

//start watchdog
app.UseWatchDogExceptionLogger();
app.UseWatchDog(opt =>
{
    opt.WatchPageUsername = "vmartinez";
    opt.WatchPagePassword = "123456";
});
//end watchdog

app.Run();
