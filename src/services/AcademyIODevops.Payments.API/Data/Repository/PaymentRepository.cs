using Microsoft.EntityFrameworkCore;
using AcademyIODevops.Payments.API.Business;
using AcademyIODevops.Core.Data;

namespace AcademyIODevops.Payments.API.Data.Repository;

public class PaymentRepository(PaymentsContext context) : IPaymentRepository
{
    private readonly DbSet<Payment> _dbSet = context.Set<Payment>();
    public IUnitOfWork UnitOfWork => context;
    public void Add(Payment payment)
    {
        _dbSet.Add(payment);
    }

    public void AddTransaction(Transaction transaction)
    {
        context.Set<Transaction>().Add(transaction);
    }

    public async Task<bool> PaymentExists(Guid studentId, Guid courseId)
    {
        return await _dbSet.AnyAsync(x => x.CourseId == courseId && x.StudentId == studentId);
    }

    public void Dispose()
    {
        context.Dispose();
    }
}