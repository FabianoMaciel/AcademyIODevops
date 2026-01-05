using AcademyIODevops.Core.DomainObjects;
using AcademyIODevops.Payments.API.Business;
using AcademyIODevops.Payments.API.Tests.Builders;
using FluentAssertions;

namespace AcademyIODevops.Payments.API.Tests.Unit.Domain;

public class PaymentTests
{
    [Fact]
    public void Payment_ShouldBeOfType_Entity()
    {
        // Arrange & Act
        var payment = new PaymentBuilder().Build();

        // Assert
        payment.Should().BeAssignableTo<Entity>();
    }

    [Fact]
    public void Payment_ShouldImplement_IAggregateRoot()
    {
        // Arrange & Act
        var payment = new PaymentBuilder().Build();

        // Assert
        payment.Should().BeAssignableTo<IAggregateRoot>();
    }

    [Fact]
    public void Payment_ShouldHaveId_WhenCreated()
    {
        // Arrange & Act
        var payment = new PaymentBuilder().Build();

        // Assert
        payment.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Payment_ShouldSetCourseId_Correctly()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        // Act
        var payment = new PaymentBuilder()
            .WithCourseId(courseId)
            .Build();

        // Assert
        payment.CourseId.Should().Be(courseId);
    }

    [Fact]
    public void Payment_ShouldSetStudentId_Correctly()
    {
        // Arrange
        var studentId = Guid.NewGuid();

        // Act
        var payment = new PaymentBuilder()
            .WithStudentId(studentId)
            .Build();

        // Assert
        payment.StudentId.Should().Be(studentId);
    }

    [Fact]
    public void Payment_ShouldSetValue_Correctly()
    {
        // Arrange & Act
        var payment = new PaymentBuilder()
            .WithValue(1500.50)
            .Build();

        // Assert
        payment.Value.Should().Be(1500.50);
    }

    [Fact]
    public void Payment_ShouldSetCardName_Correctly()
    {
        // Arrange & Act
        var payment = new PaymentBuilder()
            .WithCardName("John Doe")
            .Build();

        // Assert
        payment.CardName.Should().Be("John Doe");
    }

    [Fact]
    public void Payment_ShouldSetCardNumber_Correctly()
    {
        // Arrange & Act
        var payment = new PaymentBuilder()
            .WithCardNumber("4111111111111111")
            .Build();

        // Assert
        payment.CardNumber.Should().Be("4111111111111111");
    }

    [Fact]
    public void Payment_ShouldSetCardExpirationDate_Correctly()
    {
        // Arrange & Act
        var payment = new PaymentBuilder()
            .WithCardExpirationDate("12/25")
            .Build();

        // Assert
        payment.CardExpirationDate.Should().Be("12/25");
    }

    [Fact]
    public void Payment_ShouldSetCardCVV_Correctly()
    {
        // Arrange & Act
        var payment = new PaymentBuilder()
            .WithCardCVV("123")
            .Build();

        // Assert
        payment.CardCVV.Should().Be("123");
    }

    [Fact]
    public void Payment_ShouldAllowTransaction_ToBeSet()
    {
        // Arrange
        var transaction = new TransactionBuilder().Build();

        // Act
        var payment = new PaymentBuilder()
            .WithTransaction(transaction)
            .Build();

        // Assert
        payment.Transaction.Should().NotBeNull();
        payment.Transaction.Should().Be(transaction);
    }

    [Fact]
    public void Payment_ShouldHaveAllRequiredProperties()
    {
        // Arrange & Act
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Value = 2500.00,
            CardName = "Jane Smith",
            CardNumber = "5500000000000004",
            CardExpirationDate = "06/26",
            CardCVV = "456"
        };

        // Assert
        payment.Id.Should().NotBeEmpty();
        payment.CourseId.Should().NotBeEmpty();
        payment.StudentId.Should().NotBeEmpty();
        payment.Value.Should().BeGreaterThan(0);
        payment.CardName.Should().NotBeNullOrWhiteSpace();
        payment.CardNumber.Should().NotBeNullOrWhiteSpace();
        payment.CardExpirationDate.Should().NotBeNullOrWhiteSpace();
        payment.CardCVV.Should().NotBeNullOrWhiteSpace();
    }

    [Theory]
    [InlineData(100.00)]
    [InlineData(500.50)]
    [InlineData(1000.99)]
    [InlineData(5000.00)]
    public void Payment_ShouldAcceptDifferentValues(double value)
    {
        // Arrange & Act
        var payment = new PaymentBuilder()
            .WithValue(value)
            .Build();

        // Assert
        payment.Value.Should().Be(value);
    }
}
