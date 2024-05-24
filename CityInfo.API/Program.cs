using Microsoft.AspNetCore.StaticFiles;
using Serilog;


Log.Logger = new LoggerConfiguration()
.MinimumLevel.Information()
.WriteTo.Console()
.WriteTo.File("logs/myapp.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddControllers(options=>{
    options.ReturnHttpNotAcceptable = true; // Return 406 Not Acceptable for not supported foramt
}).AddXmlDataContractSerializerFormatters();

builder.Services.AddProblemDetails(); //Global Handling error use with  app.UseExceptionHandler(); In PRODUCTION to making a friendly error msg

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<FileExtensionContentTypeProvider>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

