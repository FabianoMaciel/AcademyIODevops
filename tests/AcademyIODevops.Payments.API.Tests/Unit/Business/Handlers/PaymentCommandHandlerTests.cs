using AcademyIODevops.Core.DomainObjects.DTOs;
using AcademyIODevops.Core.Messages.IntegrationCommands;
using AcademyIODevops.Payments.API.Business;
using AcademyIODevops.Payments.API.Business.Handlers;
using FluentAssertions;
using Moq;

namespace AcademyIODevops.Payments.API.Tests.Unit.Business.Handlers;

public class PaymentCommandHandlerTests
{
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly PaymentCommandHandler _handler;

    public PaymentCommandHandlerTests()
    {
        _paymentServiceMock = new Mock<IPaymentService>();
        _handler = new PaymentCommandHandler(_paymentServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallPaymentService_WithCorrectPaymentCourse()
    {
        // Arrange
        var paymentCourse = new PaymentCourse
        {
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Total = 1500.00,
            CardName = "John Doe",
            CardNumber = "4111111111111111",
            CardExpirationDate = "12/25",
            CardCVV = "123"
        };

        var command = new MakePaymentCourseCommand(paymentCourse);

        _paymentServiceMock
            .Setup(x => x.MakePaymentCourse(It.IsAny<PaymentCourse>()))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        _paymentServiceMock.Verify(x => x.MakePaymentCourse(paymentCourse), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenPaymentIsSuccessful()
    {
        // Arrange
        var paymentCourse = new PaymentCourse
        {
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Total = 2000.00,
            CardName = "Jane Smith",
            CardNumber = "5500000000000004",
            CardExpirationDate = "06/26",
            CardCVV = "456"
        };

        var command = new MakePaymentCourseCommand(paymentCourse);

        _paymentServiceMock
            .Setup(x => x.MakePaymentCourse(paymentCourse))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenPaymentFails()
    {
        // Arrange
        var paymentCourse = new PaymentCourse
        {
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Total = 1500.00,
            CardName = "John Doe",
            CardNumber = "4111111111111111",
            CardExpirationDate = "12/25",
            CardCVV = "123"
        };

        var command = new MakePaymentCourseCommand(paymentCourse);

        _paymentServiceMock
            .Setup(x => x.MakePaymentCourse(paymentCourse))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_ShouldPassCancellationToken()
    {
        // Arrange
        var paymentCourse = new PaymentCourse
        {
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Total = 1000.00,
            CardName = "Test User",
            CardNumber = "4000000000000002",
            CardExpirationDate = "03/27",
            CardCVV = "789"
        };

        var command = new MakePaymentCourseCommand(paymentCourse);
        var cancellationToken = new CancellationToken();

        _paymentServiceMock
            .Setup(x => x.MakePaymentCourse(paymentCourse))
            .ReturnsAsync(true);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _paymentServiceMock.Verify(x => x.MakePaymentCourse(paymentCourse), Times.Once);
    }

    [Theory]
    [InlineData(100.00, true)]
    [InlineData(500.50, true)]
    [InlineData(1000.00, false)]
    [InlineData(5000.00, false)]
    public async Task Handle_ShouldReturnCorrectResult_ForDifferentScenarios(double amount, bool expectedResult)
    {
        // Arrange
        var paymentCourse = new PaymentCourse
        {
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Total = amount,
            CardName = "Test User",
            CardNumber = "4111111111111111",
            CardExpirationDate = "12/25",
            CardCVV = "123"
        };

        var command = new MakePaymentCourseCommand(paymentCourse);

        _paymentServiceMock
            .Setup(x => x.MakePaymentCourse(paymentCourse))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be(expectedResult);
    }
}
