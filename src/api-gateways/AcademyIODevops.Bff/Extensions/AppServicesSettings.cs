using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Bff.Extensions
{
    [ExcludeFromCodeCoverage]

    public class AppServicesSettings
    {
        public string CourseUrl { get; set; }
        public string StudentUrl { get; set; }
        public string PaymentUrl { get; set; }
    }
}
