using AcademyIODevops.Auth.API.Data;
using AcademyIODevops.WebAPI.Core.DatabaseFlavor;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using static AcademyIODevops.WebAPI.Core.DatabaseFlavor.ProviderConfiguration;


namespace AcademyIODevops.Auth.API.Configuration
{
    public static class IdentityConfig
    {
        public static IServiceCollection AddIdentityConfiguration(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.ConfigureProviderForContext<ApplicationDbContext>(DetectDatabase(configuration), "AcademyIODevops.Auth.API");

            services.AddMemoryCache()
                .AddDataProtection();

            services.AddDefaultIdentity<IdentityUser<Guid>>()
             .AddRoles<IdentityRole<Guid>>()
             .AddEntityFrameworkStores<ApplicationDbContext>()
             .AddSignInManager()
             .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
             .AddDefaultTokenProviders();            

            return services;
        }
    }
}
