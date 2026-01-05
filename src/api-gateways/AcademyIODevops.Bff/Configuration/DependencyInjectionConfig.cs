using AcademyIODevops.Bff.Extensions;
using AcademyIODevops.Bff.Services;
using AcademyIODevops.WebAPI.Core.Extensions;
using AcademyIODevops.WebAPI.Core.User;
using Polly;
using System.Diagnostics.CodeAnalysis;


namespace AcademyIODevops.Bff.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAspNetUser, AspNetUser>();

            services.AddTransient<HttpClientAuthorizationDelegatingHandler>();

            services.AddHttpClient<IPaymentService, PaymentService>();
            services.AddHttpClient<IStudentService, StudentService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AllowSelfSignedCertificate()
                .AddPolicyHandler(PollyExtensions.Retry())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));

            services.AddHttpClient<ICourseService, CourseService>()
                .AddHttpMessageHandler<HttpClientAuthorizationDelegatingHandler>()
                .AllowSelfSignedCertificate()
                .AddPolicyHandler(PollyExtensions.Retry())
                .AddTransientHttpErrorPolicy(
                    p => p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)));
        }
    }
}
