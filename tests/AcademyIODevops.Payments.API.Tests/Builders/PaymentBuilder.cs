using AcademyIODevops.Payments.API.Business;
using Bogus;

namespace AcademyIODevops.Payments.API.Tests.Builders;

public class PaymentBuilder
{
    private readonly Payment _payment;
    private static readonly Faker _faker = new();

    public PaymentBuilder()
    {
        _payment = new Payment
        {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Value = _faker.Random.Double(100, 5000),
            CardName = _faker.Name.FullName(),
            CardNumber = _faker.Finance.CreditCardNumber(),
            CardExpirationDate = _faker.Date.Future(2).ToString("MM/yy"),
            CardCVV = _faker.Finance.CreditCardCvv(),
            CreatedDate = DateTime.Now
        };
    }

    public PaymentBuilder WithId(Guid id)
    {
        _payment.Id = id;
        return this;
    }

    public PaymentBuilder WithCourseId(Guid courseId)
    {
        _payment.CourseId = courseId;
        return this;
    }

    public PaymentBuilder WithStudentId(Guid studentId)
    {
        _payment.StudentId = studentId;
        return this;
    }

    public PaymentBuilder WithValue(double value)
    {
        _payment.Value = value;
        return this;
    }

    public PaymentBuilder WithCardName(string cardName)
    {
        _payment.CardName = cardName;
        return this;
    }

    public PaymentBuilder WithCardNumber(string cardNumber)
    {
        _payment.CardNumber = cardNumber;
        return this;
    }

    public PaymentBuilder WithCardExpirationDate(string expirationDate)
    {
        _payment.CardExpirationDate = expirationDate;
        return this;
    }

    public PaymentBuilder WithCardCVV(string cvv)
    {
        _payment.CardCVV = cvv;
        return this;
    }

    public PaymentBuilder WithTransaction(Transaction transaction)
    {
        _payment.Transaction = transaction;
        return this;
    }

    public PaymentBuilder WithInvalidValue()
    {
        _payment.Value = -100;
        return this;
    }

    public PaymentBuilder WithEmptyCardName()
    {
        _payment.CardName = string.Empty;
        return this;
    }

    public PaymentBuilder WithInvalidCardNumber()
    {
        _payment.CardNumber = "1234";
        return this;
    }

    public PaymentBuilder WithExpiredCard()
    {
        _payment.CardExpirationDate = "01/20";
        return this;
    }

    public Payment Build() => _payment;

    public static List<Payment> BuildMany(int count)
    {
        var payments = new List<Payment>();
        for (int i = 0; i < count; i++)
        {
            payments.Add(new PaymentBuilder().Build());
        }
        return payments;
    }

    public static List<Payment> BuildDiversePayments()
    {
        return new List<Payment>
        {
            new PaymentBuilder().WithValue(500).Build(),
            new PaymentBuilder().WithValue(1500).Build(),
            new PaymentBuilder().WithValue(3000).Build(),
            new PaymentBuilder().WithValue(100).Build(),
            new PaymentBuilder().WithValue(5000).Build()
        };
    }
}
