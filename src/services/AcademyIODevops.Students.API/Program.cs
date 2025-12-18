using AcademyIODevops.WebAPI.Core.Configuration;
using AcademyIODevops.WebAPI.Core.Identity;
using AcademyIODevops.Students.API.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogger(builder.Configuration);
builder.Services.AddApiCoreConfiguration(builder.Configuration);
builder.Services.AddJwtConfiguration(builder.Configuration);
builder.Services.AddContext(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddServices();
builder.Services.AddSwaggerConfiguration();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMessageBusConfiguration(builder.Configuration);
var app = builder.Build();

app.UseSwaggerSetup();
app.UseApiCoreConfiguration(app.Environment);
app.UseDbMigrationHelperAsync();

app.Run();

