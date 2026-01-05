using AcademyIODevops.Core.Interfaces.Services;
using AcademyIODevops.Core.Notifications;
using AcademyIODevops.Students.API.Application.Commands;
using AcademyIODevops.Students.API.Application.Queries;
using AcademyIODevops.Students.API.Data.Repository;
using AcademyIODevops.Students.API.Models;
using AcademyIODevops.Students.API.Services;
using AcademyIODevops.WebAPI.Core.User;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Students.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IRegistrationRepository, RegistrationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<INotifier, Notifier>();

            services.AddScoped<IRegistrationQuery, RegistrationQuery>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AddRegistrationCommand>());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<AddUserCommand>());
            services.AddHostedService<UserRegisteredIntegrationHandler>();
            services.AddHttpContextAccessor();
            services.AddScoped<IAspNetUser, AspNetUser>();

            return services;
        }
    }
}
