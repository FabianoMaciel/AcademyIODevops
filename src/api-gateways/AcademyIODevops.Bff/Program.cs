using AcademyIODevops.Bff.Configuration;
using AcademyIODevops.Bff.Extensions;
using AcademyIODevops.WebAPI.Core.Identity;
using Serilog;
using System.Diagnostics.CodeAnalysis;

[assembly: ExcludeFromCodeCoverage]

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSerilog(new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger());

#region Configure Services


builder.Services.AddApiConfiguration(builder.Configuration);

builder.Services.AddJwtConfiguration(builder.Configuration);

builder.Services.AddSwaggerConfiguration();

builder.Services.RegisterServices();

builder.Services.Configure<AppServicesSettings>(builder.Configuration.GetSection("AppServicesSettings"));
//builder.Services.AddMessageBusConfiguration(builder.Configuration);

//builder.Services.ConfigureGrpcServices(builder.Configuration);


var app = builder.Build();
#endregion

#region Configure Pipeline


app.UseSwaggerConfiguration();

app.UseApiConfiguration(app.Environment);

app.Run();

#endregion