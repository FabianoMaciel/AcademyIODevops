using AcademyIODevops.Core.Data;
using AcademyIODevops.Core.DomainObjects.DTOs;
using AcademyIODevops.Core.Messages.Notifications;
using AcademyIODevops.Payments.API.Business;
using AcademyIODevops.Payments.API.Tests.Builders;
using FluentAssertions;
using MediatR;
using Moq;

namespace AcademyIODevops.Payments.API.Tests.Unit.Business;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentCreditCardFacade> _paymentFacadeMock;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _paymentFacadeMock = new Mock<IPaymentCreditCardFacade>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _mediatorMock = new Mock<IMediator>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _paymentRepositoryMock.Setup(x => x.UnitOfWork).Returns(_unitOfWorkMock.Object);

        _paymentService = new PaymentService(
            _paymentFacadeMock.Object,
            _paymentRepositoryMock.Object,
            _mediatorMock.Object
        );
    }

    [Fact]
    public async Task MakePaymentCourse_ShouldReturnTrue_WhenPaymentIsAccepted()
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

        var acceptedTransaction = new TransactionBuilder()
            .AsAccepted()
            .WithTotal(paymentCourse.Total)
            .Build();

        Payment? capturedPayment = null;
        _paymentFacadeMock
            .Setup(x => x.MakePayment(It.IsAny<Payment>()))
            .Callback<Payment>(p => capturedPayment = p)
            .Returns(acceptedTransaction);

        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(true);

        // Act
        var result = await _paymentService.MakePaymentCourse(paymentCourse);

        // Assert
        result.Should().BeTrue();
        capturedPayment.Should().NotBeNull();
        capturedPayment!.Value.Should().Be(paymentCourse.Total);
        capturedPayment.CardName.Should().Be(paymentCourse.CardName);
        capturedPayment.CardNumber.Should().Be(paymentCourse.CardNumber);
        capturedPayment.StudentId.Should().Be(paymentCourse.StudentId);
        capturedPayment.CourseId.Should().Be(paymentCourse.CourseId);

        _paymentRepositoryMock.Verify(x => x.Add(It.IsAny<Payment>()), Times.Once);
        _paymentRepositoryMock.Verify(x => x.AddTransaction(acceptedTransaction), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        _mediatorMock.Verify(x => x.Publish(It.IsAny<DomainNotification>(), default), Times.Never);
    }

    [Fact]
    public async Task MakePaymentCourse_ShouldReturnFalse_WhenPaymentIsDeclined()
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

        var declinedTransaction = new TransactionBuilder()
            .AsDeclined()
            .WithTotal(paymentCourse.Total)
            .Build();

        _paymentFacadeMock
            .Setup(x => x.MakePayment(It.IsAny<Payment>()))
            .Returns(declinedTransaction);

        // Act
        var result = await _paymentService.MakePaymentCourse(paymentCourse);

        // Assert
        result.Should().BeFalse();
        _paymentRepositoryMock.Verify(x => x.Add(It.IsAny<Payment>()), Times.Never);
        _paymentRepositoryMock.Verify(x => x.AddTransaction(It.IsAny<Transaction>()), Times.Never);
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        _mediatorMock.Verify(x => x.Publish(
            It.Is<DomainNotification>(n => n.Key == "Payment" && n.Value == "The transaction was declined"),
            default), Times.Once);
    }

    [Fact]
    public async Task MakePaymentCourse_ShouldAddPaymentAndTransaction_WhenAccepted()
    {
        // Arrange
        var paymentCourse = new PaymentCourse
        {
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Total = 2500.00,
            CardName = "Jane Smith",
            CardNumber = "5500000000000004",
            CardExpirationDate = "06/26",
            CardCVV = "456"
        };

        var acceptedTransaction = new TransactionBuilder()
            .AsAccepted()
            .Build();

        _paymentFacadeMock
            .Setup(x => x.MakePayment(It.IsAny<Payment>()))
            .Returns(acceptedTransaction);

        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(true);

        // Act
        var result = await _paymentService.MakePaymentCourse(paymentCourse);

        // Assert
        result.Should().BeTrue();
        _paymentRepositoryMock.Verify(x => x.Add(It.Is<Payment>(p =>
            p.CourseId == paymentCourse.CourseId &&
            p.StudentId == paymentCourse.StudentId &&
            p.Value == paymentCourse.Total &&
            p.CardName == paymentCourse.CardName &&
            p.CardNumber == paymentCourse.CardNumber &&
            p.CardExpirationDate == paymentCourse.CardExpirationDate &&
            p.CardCVV == paymentCourse.CardCVV
        )), Times.Once);

        _paymentRepositoryMock.Verify(x => x.AddTransaction(acceptedTransaction), Times.Once);
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task MakePaymentCourse_ShouldCallPaymentFacade_WithCorrectPaymentData()
    {
        // Arrange
        var paymentCourse = new PaymentCourse
        {
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Total = 750.50,
            CardName = "Test User",
            CardNumber = "4000000000000002",
            CardExpirationDate = "03/27",
            CardCVV = "789"
        };

        var acceptedTransaction = new TransactionBuilder().AsAccepted().Build();
        _paymentFacadeMock
            .Setup(x => x.MakePayment(It.IsAny<Payment>()))
            .Returns(acceptedTransaction);

        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(true);

        // Act
        await _paymentService.MakePaymentCourse(paymentCourse);

        // Assert
        _paymentFacadeMock.Verify(x => x.MakePayment(It.Is<Payment>(p =>
            p.Value == paymentCourse.Total &&
            p.CardName == paymentCourse.CardName &&
            p.CardNumber == paymentCourse.CardNumber &&
            p.CardExpirationDate == paymentCourse.CardExpirationDate &&
            p.CardCVV == paymentCourse.CardCVV &&
            p.StudentId == paymentCourse.StudentId &&
            p.CourseId == paymentCourse.CourseId
        )), Times.Once);
    }

    [Fact]
    public async Task MakePaymentCourse_ShouldCommitUnitOfWork_OnlyWhenTransactionIsAccepted()
    {
        // Arrange
        var paymentCourse = new PaymentCourse
        {
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Total = 1000.00,
            CardName = "Test User",
            CardNumber = "4111111111111111",
            CardExpirationDate = "12/25",
            CardCVV = "123"
        };

        var declinedTransaction = new TransactionBuilder().AsDeclined().Build();
        _paymentFacadeMock
            .Setup(x => x.MakePayment(It.IsAny<Payment>()))
            .Returns(declinedTransaction);

        // Act
        await _paymentService.MakePaymentCourse(paymentCourse);

        // Assert
        _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
    }

    [Theory]
    [InlineData(100.00)]
    [InlineData(500.50)]
    [InlineData(1000.00)]
    [InlineData(5000.00)]
    public async Task MakePaymentCourse_ShouldHandleDifferentPaymentAmounts(double amount)
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

        var acceptedTransaction = new TransactionBuilder()
            .AsAccepted()
            .WithTotal(amount)
            .Build();

        _paymentFacadeMock
            .Setup(x => x.MakePayment(It.IsAny<Payment>()))
            .Returns(acceptedTransaction);

        _unitOfWorkMock.Setup(x => x.Commit()).ReturnsAsync(true);

        // Act
        var result = await _paymentService.MakePaymentCourse(paymentCourse);

        // Assert
        result.Should().BeTrue();
        _paymentRepositoryMock.Verify(x => x.Add(It.Is<Payment>(p => p.Value == amount)), Times.Once);
    }
}
