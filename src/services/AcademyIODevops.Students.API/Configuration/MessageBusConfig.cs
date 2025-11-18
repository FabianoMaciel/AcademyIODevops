using System.Reflection;
using AcademyIODevops.MessageBus;
using AcademyIODevops.Core.Utils;

namespace AcademyIODevops.Students.API.Configuration
{
    internal static class MessageBusConfig
    {
        public static void AddMessageBusConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"));
        }
    }
}
