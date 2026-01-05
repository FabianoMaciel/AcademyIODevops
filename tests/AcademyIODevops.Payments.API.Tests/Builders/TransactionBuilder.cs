using AcademyIODevops.Payments.API.Business;
using Bogus;

namespace AcademyIODevops.Payments.API.Tests.Builders;

public class TransactionBuilder
{
    private readonly Transaction _transaction;
    private static readonly Faker _faker = new();

    public TransactionBuilder()
    {
        _transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            RegistrationId = Guid.NewGuid(),
            PaymentId = Guid.NewGuid(),
            Total = _faker.Random.Double(100, 5000),
            StatusTransaction = StatusTransaction.Accept,
            CreatedDate = DateTime.Now
        };
    }

    public TransactionBuilder WithId(Guid id)
    {
        _transaction.Id = id;
        return this;
    }

    public TransactionBuilder WithRegistrationId(Guid registrationId)
    {
        _transaction.RegistrationId = registrationId;
        return this;
    }

    public TransactionBuilder WithPaymentId(Guid paymentId)
    {
        _transaction.PaymentId = paymentId;
        return this;
    }

    public TransactionBuilder WithTotal(double total)
    {
        _transaction.Total = total;
        return this;
    }

    public TransactionBuilder WithStatus(StatusTransaction status)
    {
        _transaction.StatusTransaction = status;
        return this;
    }

    public TransactionBuilder AsAccepted()
    {
        _transaction.StatusTransaction = StatusTransaction.Accept;
        return this;
    }

    public TransactionBuilder AsDeclined()
    {
        _transaction.StatusTransaction = StatusTransaction.Declined;
        return this;
    }

    public TransactionBuilder WithPayment(Payment payment)
    {
        _transaction.Payment = payment;
        _transaction.PaymentId = payment.Id;
        return this;
    }

    public Transaction Build() => _transaction;

    public static List<Transaction> BuildMany(int count)
    {
        var transactions = new List<Transaction>();
        for (int i = 0; i < count; i++)
        {
            transactions.Add(new TransactionBuilder().Build());
        }
        return transactions;
    }

    public static List<Transaction> BuildWithDifferentStatuses()
    {
        return new List<Transaction>
        {
            new TransactionBuilder().AsAccepted().Build(),
            new TransactionBuilder().AsDeclined().Build(),
            new TransactionBuilder().AsAccepted().Build(),
            new TransactionBuilder().AsDeclined().Build()
        };
    }
}
