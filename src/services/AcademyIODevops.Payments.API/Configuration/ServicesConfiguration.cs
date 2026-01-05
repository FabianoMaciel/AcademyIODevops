using AcademyIODevops.Core.Interfaces.Services;
using AcademyIODevops.Core.Messages.IntegrationCommands;
using AcademyIODevops.Core.Notifications;
using AcademyIODevops.Payments.API.Application.Query;
using AcademyIODevops.Payments.API.Business;
using AcademyIODevops.Payments.API.Data.Repository;
using AcademyIODevops.WebAPI.Core.User;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Payments.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class ServicesConfiguration
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<INotifier, Notifier>();

            services.AddScoped<IPaymentQuery, PaymentQuery>();

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<MakePaymentCourseCommand>());

            services.AddHttpContextAccessor();
            services.AddScoped<IAspNetUser, AspNetUser>();

            return services;
        }
    }
}
