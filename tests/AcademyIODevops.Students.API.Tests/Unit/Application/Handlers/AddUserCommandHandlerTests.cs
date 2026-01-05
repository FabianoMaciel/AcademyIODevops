using AcademyIODevops.Core.Data;
using AcademyIODevops.Core.Messages.Notifications;
using AcademyIODevops.Students.API.Application.Commands;
using AcademyIODevops.Students.API.Application.Handler;
using AcademyIODevops.Students.API.Models;
using FluentAssertions;
using MediatR;
using Moq;

namespace AcademyIODevops.Students.API.Tests.Unit.Application.Handlers
{
    public class AddUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UserCommandHandler _handler;

        public AddUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _mediatorMock = new Mock<IMediator>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            // Setup UnitOfWork
            _userRepositoryMock
                .Setup(x => x.UnitOfWork)
                .Returns(_unitOfWorkMock.Object);

            _handler = new UserCommandHandler(
                _mediatorMock.Object,
                _userRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldAddUser_WhenCommandIsValid()
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: "john.doe",
                isAdmin: false,
                name: "John",
                lastName: "Doe",
                dateOfBirth: new DateTime(1995, 5, 15),
                email: "john.doe@academyio.com"
            );

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _userRepositoryMock.Verify(
                x => x.Add(It.Is<StudentUser>(u =>
                    u.Id == command.UserId &&
                    u.UserName == command.UserName &&
                    u.FirstName == command.Name &&
                    u.LastName == command.LastName &&
                    u.Email == command.Email &&
                    u.IsAdmin == command.IsAdmin
                )),
                Times.Once
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);

            _mediatorMock.Verify(
                x => x.Publish(It.IsAny<DomainNotification>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCommitFails()
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: "test.user",
                isAdmin: false,
                name: "Test",
                lastName: "User",
                dateOfBirth: new DateTime(1990, 1, 1),
                email: "test@academyio.com"
            );

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _userRepositoryMock.Verify(
                x => x.Add(It.IsAny<StudentUser>()),
                Times.Once
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: string.Empty, // UserName inválido
                isAdmin: false,
                name: "John",
                lastName: "Doe",
                dateOfBirth: DateTime.Now,
                email: "john@test.com"
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _userRepositoryMock.Verify(
                x => x.Add(It.IsAny<StudentUser>()),
                Times.Never
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);

            _mediatorMock.Verify(
                x => x.Publish(It.IsAny<DomainNotification>(), It.IsAny<CancellationToken>()),
                Times.AtLeastOnce
            );
        }

        [Fact]
        public async Task Handle_ShouldPublishNotifications_WhenValidationFails()
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: string.Empty,  // Inválido
                isAdmin: false,
                name: string.Empty,      // Inválido
                lastName: string.Empty,  // Inválido
                dateOfBirth: DateTime.Now,
                email: string.Empty      // Inválido
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            // Deve publicar notificações para cada erro de validação
            _mediatorMock.Verify(
                x => x.Publish(It.IsAny<DomainNotification>(), It.IsAny<CancellationToken>()),
                Times.AtLeast(4) // 4 erros de validação
            );
        }

        [Theory]
        [InlineData("john.doe", "John", "Doe", "john.doe@academyio.com")]
        [InlineData("jane.smith", "Jane", "Smith", "jane.smith@academyio.com")]
        [InlineData("admin.user", "Admin", "User", "admin@academyio.com")]
        public async Task Handle_ShouldAddUser_WithDifferentValidData(
            string userName, string firstName, string lastName, string email)
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: userName,
                isAdmin: false,
                name: firstName,
                lastName: lastName,
                dateOfBirth: new DateTime(1995, 1, 1),
                email: email
            );

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _userRepositoryMock.Verify(
                x => x.Add(It.Is<StudentUser>(u =>
                    u.UserName == userName &&
                    u.FirstName == firstName &&
                    u.LastName == lastName &&
                    u.Email == email
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_ShouldUseProvidedUserId_WhenCreatingUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new AddUserCommand(
                userId: userId,
                userName: "test.user",
                isAdmin: false,
                name: "Test",
                lastName: "User",
                dateOfBirth: DateTime.Now.AddYears(-20),
                email: "test@academyio.com"
            );

            StudentUser? capturedUser = null;

            _userRepositoryMock
                .Setup(x => x.Add(It.IsAny<StudentUser>()))
                .Callback<StudentUser>(u => capturedUser = u);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            capturedUser.Should().NotBeNull();
            capturedUser!.Id.Should().Be(userId);
        }

        [Fact]
        public async Task Handle_ShouldCreateAdminUser_WhenIsAdminIsTrue()
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: "admin.user",
                isAdmin: true,
                name: "Admin",
                lastName: "User",
                dateOfBirth: new DateTime(1985, 1, 1),
                email: "admin@academyio.com"
            );

            StudentUser? capturedUser = null;

            _userRepositoryMock
                .Setup(x => x.Add(It.IsAny<StudentUser>()))
                .Callback<StudentUser>(u => capturedUser = u);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            capturedUser.Should().NotBeNull();
            capturedUser!.IsAdmin.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_ShouldNotCallRepository_WhenUserNameIsEmpty()
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: string.Empty, // UserName inválido
                isAdmin: false,
                name: "Valid Name",
                lastName: "Valid LastName",
                dateOfBirth: DateTime.Now.AddYears(-20),
                email: "valid@academyio.com"
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _userRepositoryMock.Verify(
                x => x.Add(It.IsAny<StudentUser>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_ShouldCallCommit_OnlyAfterAddingUser()
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: "test.user",
                isAdmin: false,
                name: "Test",
                lastName: "User",
                dateOfBirth: DateTime.Now.AddYears(-20),
                email: "test@academyio.com"
            );

            var callOrder = new List<string>();

            _userRepositoryMock
                .Setup(x => x.Add(It.IsAny<StudentUser>()))
                .Callback(() => callOrder.Add("Add"));

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .Callback(() => callOrder.Add("Commit"))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            callOrder.Should().HaveCount(2);
            callOrder[0].Should().Be("Add");
            callOrder[1].Should().Be("Commit");
        }
    }
}
