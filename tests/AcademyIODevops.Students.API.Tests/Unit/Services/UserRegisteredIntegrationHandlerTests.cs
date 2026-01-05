using AcademyIODevops.Core.Messages.Integration;
using AcademyIODevops.MessageBus;
using AcademyIODevops.Students.API.Application.Commands;
using AcademyIODevops.Students.API.Services;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;

namespace AcademyIODevops.Students.API.Tests.Unit.Services
{
    public class UserRegisteredIntegrationHandlerTests
    {
        private readonly Mock<IMessageBus> _messageBusMock;
        private readonly Mock<IServiceProvider> _serviceProviderMock;
        private readonly Mock<ILogger<UserRegisteredIntegrationHandler>> _loggerMock;
        private readonly Mock<IServiceScope> _serviceScopeMock;
        private readonly Mock<IServiceScopeFactory> _serviceScopeFactoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly UserRegisteredIntegrationHandler _handler;

        public UserRegisteredIntegrationHandlerTests()
        {
            _messageBusMock = new Mock<IMessageBus>();
            _serviceProviderMock = new Mock<IServiceProvider>();
            _loggerMock = new Mock<ILogger<UserRegisteredIntegrationHandler>>();
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

            _handler = new UserRegisteredIntegrationHandler(
                _serviceProviderMock.Object,
                _messageBusMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task RegisterStudent_ShouldReturnSuccess_WhenMediatorReturnsTrue()
        {
            // Arrange
            var integrationEvent = new UserRegisteredIntegrationEvent(
                id: Guid.NewGuid(),
                firstName: "John",
                lastName: "Doe",
                userName: "john.doe",
                dateOfBirth: new DateTime(1990, 1, 1),
                isAdmin: false
            );

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<AddUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await InvokeRegisterStudent(integrationEvent);

            // Assert
            result.Should().NotBeNull();
            result.ValidationResult.Should().NotBeNull();
            result.ValidationResult.IsValid.Should().BeTrue();
            result.ValidationResult.Errors.Should().BeEmpty();

            _mediatorMock.Verify(
                x => x.Send(
                    It.Is<AddUserCommand>(cmd =>
                        cmd.UserId == integrationEvent.Id &&
                        cmd.UserName == integrationEvent.UserName &&
                        cmd.Name == integrationEvent.FirstName &&
                        cmd.LastName == integrationEvent.LastName &&
                        cmd.Email == integrationEvent.UserName
                    ),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task RegisterStudent_ShouldReturnFailure_WhenMediatorReturnsFalse()
        {
            // Arrange
            var integrationEvent = new UserRegisteredIntegrationEvent(
                id: Guid.NewGuid(),
                firstName: "John",
                lastName: "Doe",
                userName: "john.doe",
                dateOfBirth: new DateTime(1990, 1, 1),
                isAdmin: false
            );

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<AddUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            // Act
            var result = await InvokeRegisterStudent(integrationEvent);

            // Assert
            result.Should().NotBeNull();
            result.ValidationResult.Should().NotBeNull();
            result.ValidationResult.IsValid.Should().BeFalse();
            result.ValidationResult.Errors.Should().HaveCount(1);
            result.ValidationResult.Errors[0].ErrorMessage.Should().Be("Falha ao cadastrar estudante");

            _mediatorMock.Verify(
                x => x.Send(It.IsAny<AddUserCommand>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task RegisterStudent_ShouldHandleOperationCanceledException()
        {
            // Arrange
            var integrationEvent = new UserRegisteredIntegrationEvent(
                id: Guid.NewGuid(),
                firstName: "John",
                lastName: "Doe",
                userName: "john.doe",
                dateOfBirth: new DateTime(1990, 1, 1),
                isAdmin: false
            );

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<AddUserCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new OperationCanceledException());

            // Act
            var result = await InvokeRegisterStudent(integrationEvent);

            // Assert
            result.Should().NotBeNull();
            result.ValidationResult.Should().NotBeNull();
            result.ValidationResult.IsValid.Should().BeFalse();
            result.ValidationResult.Errors.Should().HaveCount(1);
            result.ValidationResult.Errors[0].ErrorMessage.Should().Be("Operação cancelada");
        }

        [Fact]
        public async Task RegisterStudent_ShouldCreateScope_BeforeSendingCommand()
        {
            // Arrange
            var integrationEvent = new UserRegisteredIntegrationEvent(
                id: Guid.NewGuid(),
                firstName: "John",
                lastName: "Doe",
                userName: "john.doe",
                dateOfBirth: new DateTime(1990, 1, 1),
                isAdmin: false
            );

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<AddUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            await InvokeRegisterStudent(integrationEvent);

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
        public async Task RegisterStudent_ShouldMapAllProperties_FromIntegrationEvent()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userName = "test.user";
            var firstName = "Test";
            var lastName = "User";
            var dateOfBirth = new DateTime(1995, 5, 15);
            var isAdmin = true;

            var integrationEvent = new UserRegisteredIntegrationEvent(
                id: userId,
                firstName: firstName,
                lastName: lastName,
                userName: userName,
                dateOfBirth: dateOfBirth,
                isAdmin: isAdmin
            );

            AddUserCommand? capturedCommand = null;

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<AddUserCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>((cmd, ct) =>
                {
                    capturedCommand = cmd as AddUserCommand;
                })
                .ReturnsAsync(true);

            // Act
            await InvokeRegisterStudent(integrationEvent);

            // Assert
            capturedCommand.Should().NotBeNull();
            capturedCommand!.UserId.Should().Be(userId);
            capturedCommand.UserName.Should().Be(userName);
            capturedCommand.Name.Should().Be(firstName);
            capturedCommand.LastName.Should().Be(lastName);
            capturedCommand.Email.Should().Be(userName); // O handler usa UserName como email
            capturedCommand.DateOfBirth.Should().Be(dateOfBirth);
            capturedCommand.IsAdmin.Should().Be(isAdmin);
        }

        [Fact]
        public async Task RegisterStudent_ShouldReturnValidationResult_WithNoErrors_OnSuccess()
        {
            // Arrange
            var integrationEvent = new UserRegisteredIntegrationEvent(
                id: Guid.NewGuid(),
                firstName: "John",
                lastName: "Doe",
                userName: "john.doe",
                dateOfBirth: new DateTime(1990, 1, 1),
                isAdmin: false
            );

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<AddUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await InvokeRegisterStudent(integrationEvent);

            // Assert
            result.ValidationResult.Errors.Should().BeEmpty();
            result.ValidationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task RegisterStudent_ShouldHandleMultipleFailures()
        {
            // Arrange
            var integrationEvent = new UserRegisteredIntegrationEvent(
                id: Guid.NewGuid(),
                firstName: "John",
                lastName: "Doe",
                userName: "john.doe",
                dateOfBirth: new DateTime(1990, 1, 1),
                isAdmin: false
            );

            _mediatorMock
                .SetupSequence(x => x.Send(It.IsAny<AddUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false)
                .ReturnsAsync(false)
                .ReturnsAsync(true);

            // Act
            var result1 = await InvokeRegisterStudent(integrationEvent);
            var result2 = await InvokeRegisterStudent(integrationEvent);
            var result3 = await InvokeRegisterStudent(integrationEvent);

            // Assert
            result1.ValidationResult.IsValid.Should().BeFalse();
            result2.ValidationResult.IsValid.Should().BeFalse();
            result3.ValidationResult.IsValid.Should().BeTrue();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task RegisterStudent_ShouldHandleBothAdminAndNonAdminUsers(bool isAdmin)
        {
            // Arrange
            var integrationEvent = new UserRegisteredIntegrationEvent(
                id: Guid.NewGuid(),
                firstName: "User",
                lastName: "Test",
                userName: "user",
                dateOfBirth: new DateTime(1990, 1, 1),
                isAdmin: isAdmin
            );

            AddUserCommand? capturedCommand = null;

            _mediatorMock
                .Setup(x => x.Send(It.IsAny<AddUserCommand>(), It.IsAny<CancellationToken>()))
                .Callback<IRequest<bool>, CancellationToken>((cmd, ct) =>
                {
                    capturedCommand = cmd as AddUserCommand;
                })
                .ReturnsAsync(true);

            // Act
            await InvokeRegisterStudent(integrationEvent);

            // Assert
            capturedCommand.Should().NotBeNull();
            capturedCommand!.IsAdmin.Should().Be(isAdmin);
        }

        // Helper method to invoke the private RegisterStudent method using reflection
        private async Task<ResponseMessage> InvokeRegisterStudent(UserRegisteredIntegrationEvent message)
        {
            var method = typeof(UserRegisteredIntegrationHandler)
                .GetMethod("RegisterStudent", BindingFlags.NonPublic | BindingFlags.Instance);

            if (method == null)
                throw new InvalidOperationException("RegisterStudent method not found");

            var task = (Task<ResponseMessage>)method.Invoke(_handler, new object[] { message })!;
            return await task;
        }
    }
}
