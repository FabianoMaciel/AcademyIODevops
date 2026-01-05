using AcademyIODevops.Core.DomainObjects;
using AcademyIODevops.Payments.API.Business;
using AcademyIODevops.Payments.API.Tests.Builders;
using FluentAssertions;

namespace AcademyIODevops.Payments.API.Tests.Unit.Domain;

public class TransactionTests
{
    [Fact]
    public void Transaction_ShouldBeOfType_Entity()
    {
        // Arrange & Act
        var transaction = new TransactionBuilder().Build();

        // Assert
        transaction.Should().BeAssignableTo<Entity>();
    }

    [Fact]
    public void Transaction_ShouldHaveId_WhenCreated()
    {
        // Arrange & Act
        var transaction = new TransactionBuilder().Build();

        // Assert
        transaction.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Transaction_ShouldSetRegistrationId_Correctly()
    {
        // Arrange
        var registrationId = Guid.NewGuid();

        // Act
        var transaction = new TransactionBuilder()
            .WithRegistrationId(registrationId)
            .Build();

        // Assert
        transaction.RegistrationId.Should().Be(registrationId);
    }

    [Fact]
    public void Transaction_ShouldSetPaymentId_Correctly()
    {
        // Arrange
        var paymentId = Guid.NewGuid();

        // Act
        var transaction = new TransactionBuilder()
            .WithPaymentId(paymentId)
            .Build();

        // Assert
        transaction.PaymentId.Should().Be(paymentId);
    }

    [Fact]
    public void Transaction_ShouldSetTotal_Correctly()
    {
        // Arrange & Act
        var transaction = new TransactionBuilder()
            .WithTotal(2500.50)
            .Build();

        // Assert
        transaction.Total.Should().Be(2500.50);
    }

    [Fact]
    public void Transaction_ShouldSetStatusTransaction_AsAccepted()
    {
        // Arrange & Act
        var transaction = new TransactionBuilder()
            .AsAccepted()
            .Build();

        // Assert
        transaction.StatusTransaction.Should().Be(StatusTransaction.Accept);
    }

    [Fact]
    public void Transaction_ShouldSetStatusTransaction_AsDeclined()
    {
        // Arrange & Act
        var transaction = new TransactionBuilder()
            .AsDeclined()
            .Build();

        // Assert
        transaction.StatusTransaction.Should().Be(StatusTransaction.Declined);
    }

    [Fact]
    public void Transaction_ShouldAllowPayment_ToBeSet()
    {
        // Arrange
        var payment = new PaymentBuilder().Build();

        // Act
        var transaction = new TransactionBuilder()
            .WithPayment(payment)
            .Build();

        // Assert
        transaction.Payment.Should().NotBeNull();
        transaction.Payment.Should().Be(payment);
        transaction.PaymentId.Should().Be(payment.Id);
    }

    [Fact]
    public void Transaction_ShouldHaveAllRequiredProperties()
    {
        // Arrange & Act
        var transaction = new Transaction
        {
            Id = Guid.NewGuid(),
            RegistrationId = Guid.NewGuid(),
            PaymentId = Guid.NewGuid(),
            Total = 1500.00,
            StatusTransaction = StatusTransaction.Accept
        };

        // Assert
        transaction.Id.Should().NotBeEmpty();
        transaction.RegistrationId.Should().NotBeEmpty();
        transaction.PaymentId.Should().NotBeEmpty();
        transaction.Total.Should().BeGreaterThan(0);
        transaction.StatusTransaction.Should().BeOneOf(StatusTransaction.Accept, StatusTransaction.Declined);
    }

    [Theory]
    [InlineData(100.00)]
    [InlineData(500.50)]
    [InlineData(1000.99)]
    [InlineData(5000.00)]
    public void Transaction_ShouldAcceptDifferentTotalValues(double total)
    {
        // Arrange & Act
        var transaction = new TransactionBuilder()
            .WithTotal(total)
            .Build();

        // Assert
        transaction.Total.Should().Be(total);
    }

    [Fact]
    public void Transaction_ShouldHaveAcceptedStatus_ByDefault()
    {
        // Arrange & Act
        var transaction = new TransactionBuilder().Build();

        // Assert
        transaction.StatusTransaction.Should().Be(StatusTransaction.Accept);
    }

    [Fact]
    public void StatusTransaction_ShouldHaveAcceptValue()
    {
        // Arrange & Act
        var status = StatusTransaction.Accept;

        // Assert
        status.Should().Be(StatusTransaction.Accept);
    }

    [Fact]
    public void StatusTransaction_ShouldHaveDeclinedValue()
    {
        // Arrange & Act
        var status = StatusTransaction.Declined;

        // Assert
        status.Should().Be(StatusTransaction.Declined);
    }
}
