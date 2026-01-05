using AcademyIODevops.Core.Data;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Payments.API.Business;

public interface IPaymentRepository : IRepository<Payment>
{
    void Add(Payment payment);
    void AddTransaction(Transaction transaction);

    Task<bool> PaymentExists(Guid studentId, Guid courseId);
}