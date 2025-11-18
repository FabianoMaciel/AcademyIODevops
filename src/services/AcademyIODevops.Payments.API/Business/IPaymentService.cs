using AcademyIODevops.Core.DomainObjects.DTOs;

namespace AcademyIODevops.Payments.API.Business;

public interface IPaymentService
{
    Task<bool> MakePaymentCourse(PaymentCourse paymentCourse);
}