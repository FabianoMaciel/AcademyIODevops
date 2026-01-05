using AcademyIODevops.Courses.API.Data;
using AcademyIODevops.WebAPI.Core.DatabaseFlavor;
using System.Diagnostics.CodeAnalysis;
using static AcademyIODevops.WebAPI.Core.DatabaseFlavor.ProviderConfiguration;

namespace AcademyIODevops.Courses.API.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class AddEF
    {
        public static IServiceCollection AddContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigureProviderForContext<CoursesContext>(DetectDatabase(configuration), "AcademyIODevops.Courses.API");

            services.AddMemoryCache()
                .AddDataProtection();

            return services;
        }
    }
}
