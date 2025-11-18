using AcademyIODevops.Core.Data;

namespace AcademyIODevops.Payments.API.Business;

public interface IPaymentRepository : IRepository<Payment>
{
    void Add(Payment payment);
    void AddTransaction(Transaction transaction);

    Task<bool> PaymentExists(Guid studentId, Guid courseId);
}