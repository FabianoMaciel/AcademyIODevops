using AcademyIODevops.Payments.API.AntiCorruption;
using AcademyIODevops.Payments.API.Business;
using AcademyIODevops.Payments.API.Tests.Builders;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;

namespace AcademyIODevops.Payments.API.Tests.Unit.AntiCorruption;

public class PaymentCreditCardFacadeTests
{
    private readonly Mock<IPayPalGateway> _payPalGatewayMock;
    private readonly Mock<IOptions<PaymentSettings>> _optionsMock;
    private readonly PaymentSettings _paymentSettings;
    private readonly PaymentCreditCardFacade _facade;

    public PaymentCreditCardFacadeTests()
    {
        _payPalGatewayMock = new Mock<IPayPalGateway>();
        _optionsMock = new Mock<IOptions<PaymentSettings>>();

        _paymentSettings = new PaymentSettings
        {
            ApiKey = "test-api-key",
            EncriptionKey = "test-encryption-key"
        };

        _optionsMock.Setup(x => x.Value).Returns(_paymentSettings);

        _facade = new PaymentCreditCardFacade(_payPalGatewayMock.Object, _optionsMock.Object);
    }

    [Fact]
    public void MakePayment_ShouldCallPayPalGateway_WithCorrectParameters()
    {
        // Arrange
        var payment = new PaymentBuilder()
            .WithCardNumber("4111111111111111")
            .WithValue(1500.00)
            .Build();

        var serviceKey = "service-key-123";
        var cardHashKey = "card-hash-456";
        var expectedTransaction = new TransactionBuilder().AsAccepted().Build();

        _payPalGatewayMock
            .Setup(x => x.GetPayPalServiceKey(_paymentSettings.ApiKey, _paymentSettings.EncriptionKey))
            .Returns(serviceKey);

        _payPalGatewayMock
            .Setup(x => x.GetCardHashKey(serviceKey, payment.CardNumber))
            .Returns(cardHashKey);

        _payPalGatewayMock
            .Setup(x => x.CommitTransaction(cardHashKey, payment.CourseId.ToString(), payment.Value))
            .Returns(expectedTransaction);

        // Act
        var result = _facade.MakePayment(payment);

        // Assert
        _payPalGatewayMock.Verify(x => x.GetPayPalServiceKey(_paymentSettings.ApiKey, _paymentSettings.EncriptionKey), Times.Once);
        _payPalGatewayMock.Verify(x => x.GetCardHashKey(serviceKey, payment.CardNumber), Times.Once);
        _payPalGatewayMock.Verify(x => x.CommitTransaction(cardHashKey, payment.CourseId.ToString(), payment.Value), Times.Once);
    }

    [Fact]
    public void MakePayment_ShouldReturnTransaction_WithPaymentId()
    {
        // Arrange
        var payment = new PaymentBuilder().Build();
        var transaction = new TransactionBuilder().Build();

        _payPalGatewayMock
            .Setup(x => x.GetPayPalServiceKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("service-key");

        _payPalGatewayMock
            .Setup(x => x.GetCardHashKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("card-hash");

        _payPalGatewayMock
            .Setup(x => x.CommitTransaction(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
            .Returns(transaction);

        // Act
        var result = _facade.MakePayment(payment);

        // Assert
        result.Should().NotBeNull();
        result.PaymentId.Should().Be(payment.Id);
    }

    [Fact]
    public void MakePayment_ShouldUseApiKeyFromSettings()
    {
        // Arrange
        var payment = new PaymentBuilder().Build();
        var transaction = new TransactionBuilder().Build();

        _payPalGatewayMock
            .Setup(x => x.GetPayPalServiceKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("service-key");

        _payPalGatewayMock
            .Setup(x => x.GetCardHashKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("card-hash");

        _payPalGatewayMock
            .Setup(x => x.CommitTransaction(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
            .Returns(transaction);

        // Act
        _facade.MakePayment(payment);

        // Assert
        _payPalGatewayMock.Verify(x => x.GetPayPalServiceKey(_paymentSettings.ApiKey, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void MakePayment_ShouldUseEncryptionKeyFromSettings()
    {
        // Arrange
        var payment = new PaymentBuilder().Build();
        var transaction = new TransactionBuilder().Build();

        _payPalGatewayMock
            .Setup(x => x.GetPayPalServiceKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("service-key");

        _payPalGatewayMock
            .Setup(x => x.GetCardHashKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("card-hash");

        _payPalGatewayMock
            .Setup(x => x.CommitTransaction(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
            .Returns(transaction);

        // Act
        _facade.MakePayment(payment);

        // Assert
        _payPalGatewayMock.Verify(x => x.GetPayPalServiceKey(It.IsAny<string>(), _paymentSettings.EncriptionKey), Times.Once);
    }

    [Fact]
    public void MakePayment_ShouldPassCorrectCourseId()
    {
        // Arrange
        var payment = new PaymentBuilder().Build();
        var transaction = new TransactionBuilder().Build();

        _payPalGatewayMock
            .Setup(x => x.GetPayPalServiceKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("service-key");

        _payPalGatewayMock
            .Setup(x => x.GetCardHashKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("card-hash");

        _payPalGatewayMock
            .Setup(x => x.CommitTransaction(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
            .Returns(transaction);

        // Act
        _facade.MakePayment(payment);

        // Assert
        _payPalGatewayMock.Verify(x => x.CommitTransaction(
            It.IsAny<string>(),
            payment.CourseId.ToString(),
            It.IsAny<double>()), Times.Once);
    }

    [Fact]
    public void MakePayment_ShouldPassCorrectPaymentValue()
    {
        // Arrange
        var payment = new PaymentBuilder()
            .WithValue(2500.00)
            .Build();
        var transaction = new TransactionBuilder().Build();

        _payPalGatewayMock
            .Setup(x => x.GetPayPalServiceKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("service-key");

        _payPalGatewayMock
            .Setup(x => x.GetCardHashKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("card-hash");

        _payPalGatewayMock
            .Setup(x => x.CommitTransaction(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
            .Returns(transaction);

        // Act
        _facade.MakePayment(payment);

        // Assert
        _payPalGatewayMock.Verify(x => x.CommitTransaction(
            It.IsAny<string>(),
            It.IsAny<string>(),
            payment.Value), Times.Once);
    }

    [Fact]
    public void MakePayment_ShouldReturnAcceptedTransaction()
    {
        // Arrange
        var payment = new PaymentBuilder().Build();
        var acceptedTransaction = new TransactionBuilder().AsAccepted().Build();

        _payPalGatewayMock
            .Setup(x => x.GetPayPalServiceKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("service-key");

        _payPalGatewayMock
            .Setup(x => x.GetCardHashKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("card-hash");

        _payPalGatewayMock
            .Setup(x => x.CommitTransaction(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
            .Returns(acceptedTransaction);

        // Act
        var result = _facade.MakePayment(payment);

        // Assert
        result.StatusTransaction.Should().Be(StatusTransaction.Accept);
    }

    [Fact]
    public void MakePayment_ShouldReturnDeclinedTransaction()
    {
        // Arrange
        var payment = new PaymentBuilder().Build();
        var declinedTransaction = new TransactionBuilder().AsDeclined().Build();

        _payPalGatewayMock
            .Setup(x => x.GetPayPalServiceKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("service-key");

        _payPalGatewayMock
            .Setup(x => x.GetCardHashKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("card-hash");

        _payPalGatewayMock
            .Setup(x => x.CommitTransaction(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
            .Returns(declinedTransaction);

        // Act
        var result = _facade.MakePayment(payment);

        // Assert
        result.StatusTransaction.Should().Be(StatusTransaction.Declined);
    }

    [Theory]
    [InlineData("4111111111111111")]
    [InlineData("5500000000000004")]
    [InlineData("340000000000009")]
    public void MakePayment_ShouldHandleDifferentCardNumbers(string cardNumber)
    {
        // Arrange
        var payment = new PaymentBuilder()
            .WithCardNumber(cardNumber)
            .Build();
        var transaction = new TransactionBuilder().Build();

        _payPalGatewayMock
            .Setup(x => x.GetPayPalServiceKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("service-key");

        _payPalGatewayMock
            .Setup(x => x.GetCardHashKey(It.IsAny<string>(), It.IsAny<string>()))
            .Returns("card-hash");

        _payPalGatewayMock
            .Setup(x => x.CommitTransaction(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<double>()))
            .Returns(transaction);

        // Act
        var result = _facade.MakePayment(payment);

        // Assert
        result.Should().NotBeNull();
        _payPalGatewayMock.Verify(x => x.GetCardHashKey(It.IsAny<string>(), cardNumber), Times.Once);
    }
}
