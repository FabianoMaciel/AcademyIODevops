using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Courses.API.Models.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class PaymentViewModel
    {
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string CardExpirationDate { get; set; }
        public string CardCVV { get; set; }
    }
}
