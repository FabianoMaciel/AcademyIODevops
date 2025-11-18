using AcademyIODevops.Payments.API.Data;
using AcademyIODevops.WebAPI.Core.DatabaseFlavor;
using static AcademyIODevops.WebAPI.Core.DatabaseFlavor.ProviderConfiguration;

namespace AcademyIODevops.Payments.API.Configuration
{
    public static class AddEF
    {
        public static IServiceCollection AddContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigureProviderForContext<PaymentsContext>(DetectDatabase(configuration), "AcademyIODevops.Payments.API");

            services.AddMemoryCache()
                .AddDataProtection();

            return services;
        }
    }
}
