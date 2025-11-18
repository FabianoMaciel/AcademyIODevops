using AcademyIODevops.Students.API.Data;
using AcademyIODevops.WebAPI.Core.DatabaseFlavor;
using static AcademyIODevops.WebAPI.Core.DatabaseFlavor.ProviderConfiguration;

namespace AcademyIODevops.Students.API.Configuration
{
    public static class AddEF
    {
        public static IServiceCollection AddContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigureProviderForContext<StudentsContext>(DetectDatabase(configuration), "AcademyIODevops.Students.API");

            services.AddMemoryCache()
                .AddDataProtection();

            return services;
        }
    }
}
