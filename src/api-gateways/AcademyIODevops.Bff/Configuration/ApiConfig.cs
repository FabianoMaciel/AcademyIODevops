using System.Diagnostics.CodeAnalysis;
using AcademyIODevops.Bff.Extensions;
using AcademyIODevops.WebAPI.Core.Configuration;

namespace AcademyIODevops.Bff.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class ApiConfig
    {
        public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers();

            services.Configure<AppServicesSettings>(configuration);

            services.AddCors(options =>
            {
                options.AddPolicy("Total",
                    builder =>
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader());
            });

            services.AddDefaultHealthCheck(configuration)
                .AddUrlGroup(new Uri($"{configuration["AppServicesSettings:CourseUrl"]}/healthz-infra"), "Courses API", tags: new[] { "infra" })
                .AddUrlGroup(new Uri($"{configuration["AppServicesSettings:StudentUrl"]}/healthz-infra"), "Students API", tags: new[] { "infra" })
                .AddUrlGroup(new Uri($"{configuration["AppServicesSettings:PaymentUrl"]}/healthz-infra"), "Payments API", tags: new[] { "infra" });
        }

        public static void UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Under certain scenarios, e.g minikube / linux environment / behind load balancer
            // https redirection could lead dev's to over complicated configuration for testing purpouses
            // In production is a good practice to keep it true
            if (app.Configuration["USE_HTTPS_REDIRECTION"] == "true")
                app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("Total");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.UseDefaultHealthcheck();
        }
    }
}