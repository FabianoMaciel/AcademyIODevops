using AcademyIODevops.Core.Messages.Integration;
using AcademyIODevops.MessageBus;
using AcademyIODevops.Payments.API.Services;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection;
using Xunit;

namespace AcademyIODevops.Payments.API.Tests.Services
{
    public class PaymentRequestedIntegrationHandlerTests
    {
        private readonly Mock<IMessageBus> _messageBusMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<IServiceScope> _serviceScopeMock;
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly PaymentRequestedIntegrationHandler _handler;

        public PaymentRequestedIntegrationHandlerTests()
        {
            _messageBusMock = new Mock<IMessageBus>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _serviceScopeMock = new Mock<IServiceScope>();
            _serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            _mediatorMock = new Mock<IMediator>();

            // Setup service scope
            _serviceScopeMock
                .Setup(x => x.ServiceProvider)
                .Returns(_serviceProviderMock.Object);

            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(_serviceScopeFactoryMock.Object);

            _serviceScopeFactoryMock
                .Setup(x => x.CreateScope())
                .Returns(_serviceScopeMock.Object);

            _serviceProviderMock
                .Setup(x => x.GetService(typeof(IMediator)))
                .Returns(_mediatorMock.Object);

            _handler = new PaymentRequestedIntegrationHandler(
                _serviceProviderMock.Object,
                _messageBusMock.Object
            );
        }

        [Fact]
        public async Task RegisterPayment_ShouldReturnSuccess_WhenMediatorReturnsTrue()
        {
            // Arrange
            var integrationEvent = new PaymentRegisteredIntegrationEvent(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4111111111111111",
                cardExpirationDate: "12/25",
                cardCVV: "123"
            );

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ValidatePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await InvokeRegisterPayment(integrationEvent);

            // Assert
            result.Should().NotBeNull();
            result.ValidationResult.Should().NotBeNull();
            result.ValidationResult.IsValid.Should().BeTrue();
            result.ValidationResult.Errors.Should().BeEmpty();

            _mediatorMock.Verify(
                x => x.Send(
                    It.Is<ValidatePaymentCourseCommand>(cmd =>
                        cmd.CourseId == integrationEvent.CourseId &&
                        cmd.StudentId == integrationEvent.StudentId &&
                        cmd.CardName == integrationEvent.CardName &&
                        cmd.CardNumber == integrationEvent.CardNumber &&
                        cmd.CardExpirationDate == integrationEvent.CardExpirationDate &&
                        cmd.CardCVV == integrationEvent.CardCVV
                    ),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task RegisterPayment_ShouldReturnFailure_WhenMediatorReturnsFalse()
        {
            // Arrange
            var integrationEvent = new PaymentRegisteredIntegrationEvent(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4111111111111111",
                cardExpirationDate: "12/25",
                cardCVV: "123"
            );

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ValidatePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await InvokeRegisterPayment(integrationEvent);

            // Assert
            result.Should().NotBeNull();
            result.ValidationResult.Should().NotBeNull();
            result.ValidationResult.IsValid.Should().BeFalse();
            result.ValidationResult.Errors.Should().HaveCount(1);
            result.ValidationResult.Errors[0].ErrorMessage.Should().Be("Falha ao realizar pagamento.");

            _mediatorMock.Verify(
                x => x.Send(It.IsAny<ValidatePaymentCourseCommand>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task RegisterPayment_ShouldCreateScope_BeforeSendingCommand()
        {
            // Arrange
            var integrationEvent = new PaymentRegisteredIntegrationEvent(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4111111111111111",
                cardExpirationDate: "12/25",
                cardCVV: "123"
            );

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ValidatePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await InvokeRegisterPayment(integrationEvent);

            // Assert
            _serviceScopeFactoryMock.Verify(
                x => x.CreateScope(),
                Times.Once
            );

            _serviceScopeMock.Verify(
                x => x.ServiceProvider.GetService(typeof(IMediator)),
                Times.Once
            );
        }

        [Fact]
        public async Task RegisterPayment_ShouldMapAllProperties_FromIntegrationEvent()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var cardName = "TEST USER";
            var cardNumber = "4111111111111111";
            var cardExpirationDate = "06/26";
            var cardCVV = "456";

            var integrationEvent = new PaymentRegisteredIntegrationEvent(
                courseId: courseId,
                studentId: studentId,
                cardName: cardName,
                cardNumber: cardNumber,
                cardExpirationDate: cardExpirationDate,
                cardCVV: cardCVV
            );

            ValidatePaymentCourseCommand? capturedCommand = null;

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ValidatePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>((cmd, ct) =>
                {
                    capturedCommand = cmd as ValidatePaymentCourseCommand;
                })
                .ReturnsAsync(true);

            // Act
            await InvokeRegisterPayment(integrationEvent);

            // Assert
            capturedCommand.Should().NotBeNull();
            capturedCommand!.CourseId.Should().Be(courseId);
            capturedCommand.StudentId.Should().Be(studentId);
            capturedCommand.CardName.Should().Be(cardName);
            capturedCommand.CardNumber.Should().Be(cardNumber);
            capturedCommand.CardExpirationDate.Should().Be(cardExpirationDate);
            capturedCommand.CardCVV.Should().Be(cardCVV);
        }

        [Fact]
        public async Task RegisterPayment_ShouldReturnValidationResult_WithNoErrors_OnSuccess()
        {
            // Arrange
            var integrationEvent = new PaymentRegisteredIntegrationEvent(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4111111111111111",
                cardExpirationDate: "12/25",
                cardCVV: "123"
            );

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ValidatePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await InvokeRegisterPayment(integrationEvent);

            // Assert
            result.ValidationResult.Errors.Should().BeEmpty();
            result.ValidationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task RegisterPayment_ShouldHandleMultipleFailures()
        {
            // Arrange
            var integrationEvent = new PaymentRegisteredIntegrationEvent(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4111111111111111",
                cardExpirationDate: "12/25",
                cardCVV: "123"
            );

            _mediatorMock
                .SetupSequence(x => x.Send(It.IsAny<ValidatePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false)
                .ReturnsAsync(false)
                .ReturnsAsync(true);

            // Act
            var result1 = await InvokeRegisterPayment(integrationEvent);
            var result2 = await InvokeRegisterPayment(integrationEvent);
            var result3 = await InvokeRegisterPayment(integrationEvent);

            // Assert
            result1.ValidationResult.IsValid.Should().BeFalse();
            result2.ValidationResult.IsValid.Should().BeFalse();
            result3.ValidationResult.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData("4111111111111111")]
        [InlineData("5555555555554444")]
        [InlineData("378282246310005")]
        public async Task RegisterPayment_ShouldAcceptDifferentCardNumbers(string cardNumber)
        {
            // Arrange
            var integrationEvent = new PaymentRegisteredIntegrationEvent(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: cardNumber,
                cardExpirationDate: "12/25",
                cardCVV: "123"
            );

            ValidatePaymentCourseCommand? capturedCommand = null;

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ValidatePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>((cmd, ct) =>
                {
                    capturedCommand = cmd as ValidatePaymentCourseCommand;
                })
                .ReturnsAsync(true);

            // Act
            await InvokeRegisterPayment(integrationEvent);

            // Assert
            capturedCommand.Should().NotBeNull();
            capturedCommand!.CardNumber.Should().Be(cardNumber);
        }

        [Theory]
        [InlineData("JOHN DOE")]
        [InlineData("Jane Smith")]
        [InlineData("María García")]
        public async Task RegisterPayment_ShouldAcceptDifferentCardNames(string cardName)
        {
            // Arrange
            var integrationEvent = new PaymentRegisteredIntegrationEvent(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: cardName,
                cardNumber: "4111111111111111",
                cardExpirationDate: "12/25",
                cardCVV: "123"
            );

            ValidatePaymentCourseCommand? capturedCommand = null;

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ValidatePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>((cmd, ct) =>
                {
                    capturedCommand = cmd as ValidatePaymentCourseCommand;
                })
                .ReturnsAsync(true);

            // Act
            await InvokeRegisterPayment(integrationEvent);

            // Assert
            capturedCommand.Should().NotBeNull();
            capturedCommand!.CardName.Should().Be(cardName);
        }

        [Fact]
        public async Task RegisterPayment_ShouldCallMediator_WithCorrectCommandType()
        {
            // Arrange
            var integrationEvent = new PaymentRegisteredIntegrationEvent(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4111111111111111",
                cardExpirationDate: "12/25",
                cardCVV: "123"
            );

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ValidatePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await InvokeRegisterPayment(integrationEvent);

            // Assert
            _mediatorMock.Verify(
                x => x.Send(It.IsAny<ValidatePaymentCourseCommand>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task RegisterPayment_ShouldPreserveCardExpirationDate()
        {
            // Arrange
            var expirationDate = "12/28";
            var integrationEvent = new PaymentRegisteredIntegrationEvent(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4111111111111111",
                cardExpirationDate: expirationDate,
                cardCVV: "123"
            );

            ValidatePaymentCourseCommand? capturedCommand = null;

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ValidatePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>((cmd, ct) =>
                {
                    capturedCommand = cmd as ValidatePaymentCourseCommand;
                })
                .ReturnsAsync(true);

            // Act
            await InvokeRegisterPayment(integrationEvent);

            // Assert
            capturedCommand.Should().NotBeNull();
            capturedCommand!.CardExpirationDate.Should().Be(expirationDate);
        }

        [Fact]
        public async Task RegisterPayment_ShouldPreserveCardCVV()
        {
            // Arrange
            var cvv = "987";
            var integrationEvent = new PaymentRegisteredIntegrationEvent(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4111111111111111",
                cardExpirationDate: "12/25",
                cardCVV: cvv
            );

            ValidatePaymentCourseCommand? capturedCommand = null;

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<ValidatePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>((cmd, ct) =>
                {
                    capturedCommand = cmd as ValidatePaymentCourseCommand;
                })
                .ReturnsAsync(true);

            // Act
            await InvokeRegisterPayment(integrationEvent);

            // Assert
            capturedCommand.Should().NotBeNull();
            capturedCommand!.CardCVV.Should().Be(cvv);
        }

        // Helper method to invoke the private RegisterPayment method using reflection
        private async Task<ResponseMessage> InvokeRegisterPayment(PaymentRegisteredIntegrationEvent message)
        {
            var method = typeof(PaymentRequestedIntegrationHandler)
                .GetMethod("RegisterPayment", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
                throw new InvalidOperationException("RegisterPayment method not found");

            var task = (Task<ResponseMessage>)method.Invoke(_handler, new object[] { message })!;
            return await task;
        }
    }
}
