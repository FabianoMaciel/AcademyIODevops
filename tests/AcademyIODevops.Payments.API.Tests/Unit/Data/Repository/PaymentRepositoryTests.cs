using AcademyIODevops.Payments.API.Business;
using AcademyIODevops.Payments.API.Data.Repository;
using AcademyIODevops.Payments.API.Tests.Builders;
using AcademyIODevops.Payments.API.Tests.Fixtures;
using FluentAssertions;

namespace AcademyIODevops.Payments.API.Tests.Unit.Data.Repository;

public class PaymentRepositoryTests
{
    private readonly RepositoryTestFixture _fixture;

    public PaymentRepositoryTests()
    {
        _fixture = new RepositoryTestFixture();
    }

    [Fact]
    public void Add_ShouldAddPayment_ToDatabase()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new PaymentRepository(context);
        var payment = new PaymentBuilder().Build();

        // Act
        repository.Add(payment);
        context.SaveChanges();

        // Assert
        var paymentInDb = context.Set<Payment>().Find(payment.Id);
        paymentInDb.Should().NotBeNull();
        paymentInDb!.Id.Should().Be(payment.Id);
        paymentInDb.CourseId.Should().Be(payment.CourseId);
        paymentInDb.StudentId.Should().Be(payment.StudentId);
        paymentInDb.Value.Should().Be(payment.Value);
    }

    [Fact]
    public void Add_ShouldAddMultiplePayments()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new PaymentRepository(context);
        var payments = PaymentBuilder.BuildMany(3);

        // Act
        foreach (var payment in payments)
        {
            repository.Add(payment);
        }
        context.SaveChanges();

        // Assert
        var paymentsInDb = context.Set<Payment>().ToList();
        paymentsInDb.Should().HaveCount(3);
    }

    [Fact]
    public void AddTransaction_ShouldAddTransaction_ToDatabase()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new PaymentRepository(context);
        var transaction = new TransactionBuilder().Build();

        // Act
        repository.AddTransaction(transaction);
        context.SaveChanges();

        // Assert
        var transactionInDb = context.Set<Transaction>().Find(transaction.Id);
        transactionInDb.Should().NotBeNull();
        transactionInDb!.Id.Should().Be(transaction.Id);
        transactionInDb.PaymentId.Should().Be(transaction.PaymentId);
        transactionInDb.Total.Should().Be(transaction.Total);
        transactionInDb.StatusTransaction.Should().Be(transaction.StatusTransaction);
    }

    [Fact]
    public void AddTransaction_ShouldAddMultipleTransactions()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new PaymentRepository(context);
        var transactions = TransactionBuilder.BuildMany(5);

        // Act
        foreach (var transaction in transactions)
        {
            repository.AddTransaction(transaction);
        }
        context.SaveChanges();

        // Assert
        var transactionsInDb = context.Set<Transaction>().ToList();
        transactionsInDb.Should().HaveCount(5);
    }

    [Fact]
    public async Task PaymentExists_ShouldReturnTrue_WhenPaymentExists()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new PaymentRepository(context);
        var payment = new PaymentBuilder().Build();

        _fixture.SeedDatabase(context, payment);

        // Act
        var exists = await repository.PaymentExists(payment.StudentId, payment.CourseId);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task PaymentExists_ShouldReturnFalse_WhenPaymentDoesNotExist()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new PaymentRepository(context);
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        // Act
        var exists = await repository.PaymentExists(studentId, courseId);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task PaymentExists_ShouldReturnTrue_WhenPaymentExistsForSpecificStudentAndCourse()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new PaymentRepository(context);
        var studentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        var payment1 = new PaymentBuilder()
            .WithStudentId(studentId)
            .WithCourseId(courseId)
            .Build();

        var payment2 = new PaymentBuilder().Build(); // Different student and course

        _fixture.SeedDatabase(context, payment1, payment2);

        // Act
        var exists = await repository.PaymentExists(studentId, courseId);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task PaymentExists_ShouldReturnFalse_ForDifferentStudentId()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new PaymentRepository(context);
        var courseId = Guid.NewGuid();
        var payment = new PaymentBuilder()
            .WithCourseId(courseId)
            .Build();

        _fixture.SeedDatabase(context, payment);

        // Act
        var exists = await repository.PaymentExists(Guid.NewGuid(), courseId);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public async Task PaymentExists_ShouldReturnFalse_ForDifferentCourseId()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new PaymentRepository(context);
        var studentId = Guid.NewGuid();
        var payment = new PaymentBuilder()
            .WithStudentId(studentId)
            .Build();

        _fixture.SeedDatabase(context, payment);

        // Act
        var exists = await repository.PaymentExists(studentId, Guid.NewGuid());

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public void UnitOfWork_ShouldReturnContext()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new PaymentRepository(context);

        // Act
        var unitOfWork = repository.UnitOfWork;

        // Assert
        unitOfWork.Should().Be(context);
    }

    [Fact]
    public void Add_ShouldPreserveAllPaymentProperties()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new PaymentRepository(context);
        var payment = new PaymentBuilder()
            .WithValue(1500.00)
            .WithCardName("John Doe")
            .WithCardNumber("4111111111111111")
            .WithCardExpirationDate("12/25")
            .WithCardCVV("123")
            .Build();

        // Act
        repository.Add(payment);
        context.SaveChanges();

        // Assert
        var paymentInDb = context.Set<Payment>().Find(payment.Id);
        paymentInDb.Should().NotBeNull();
        paymentInDb!.Value.Should().Be(1500.00);
        paymentInDb.CardName.Should().Be("John Doe");
        paymentInDb.CardNumber.Should().Be("4111111111111111");
        paymentInDb.CardExpirationDate.Should().Be("12/25");
        paymentInDb.CardCVV.Should().Be("123");
    }

    [Fact]
    public void AddTransaction_ShouldPreserveTransactionStatus()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new PaymentRepository(context);
        var acceptedTransaction = new TransactionBuilder().AsAccepted().Build();
        var declinedTransaction = new TransactionBuilder().AsDeclined().Build();

        // Act
        repository.AddTransaction(acceptedTransaction);
        repository.AddTransaction(declinedTransaction);
        context.SaveChanges();

        // Assert
        var acceptedInDb = context.Set<Transaction>().Find(acceptedTransaction.Id);
        var declinedInDb = context.Set<Transaction>().Find(declinedTransaction.Id);

        acceptedInDb!.StatusTransaction.Should().Be(StatusTransaction.Accept);
        declinedInDb!.StatusTransaction.Should().Be(StatusTransaction.Declined);
    }
}
