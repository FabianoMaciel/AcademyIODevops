using System.Reflection;
using AcademyIODevops.MessageBus;
using AcademyIODevops.Core.Utils;
using AcademyIODevops.Payments.API.Services;

namespace AcademyIODevops.Payments.API.Configuration
{
    internal static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"));
            services.AddHostedService<PaymentRequestedIntegrationHandler>();
        }
    }
}
