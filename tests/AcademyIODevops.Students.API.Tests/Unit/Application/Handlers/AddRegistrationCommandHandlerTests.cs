using AcademyIODevops.Core.Data;
using AcademyIODevops.Core.Messages.Notifications;
using AcademyIODevops.Students.API.Application.Commands;
using AcademyIODevops.Students.API.Application.Handler;
using AcademyIODevops.Students.API.Models;
using AcademyIODevops.Students.API.Tests.Builders;
using FluentAssertions;
using MediatR;
using Moq;

namespace AcademyIODevops.Students.API.Tests.Unit.Application.Handlers
{
    public class AddRegistrationCommandHandlerTests
    {
        private readonly Mock<IRegistrationRepository> _registrationRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly RegistrationCommandHandler _handler;

        public AddRegistrationCommandHandlerTests()
        {
            _registrationRepositoryMock = new Mock<IRegistrationRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _mediatorMock = new Mock<IMediator>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            // Setup UnitOfWork
            _registrationRepositoryMock
                .Setup(x => x.UnitOfWork)
                .Returns(_unitOfWorkMock.Object);

            _handler = new RegistrationCommandHandler(
                _mediatorMock.Object,
                _registrationRepositoryMock.Object,
                _userRepositoryMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldAddRegistration_WhenCommandIsValid()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var student = new StudentUserBuilder()
                .WithId(studentId)
                .Build();

            var command = new AddRegistrationCommand(studentId, courseId);

            _userRepositoryMock
                .Setup(x => x.GetById(studentId))
                .ReturnsAsync(student);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _userRepositoryMock.Verify(
                x => x.GetById(studentId),
                Times.Once
            );

            _registrationRepositoryMock.Verify(
                x => x.AddRegistration(studentId, courseId),
                Times.Once
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);

            _mediatorMock.Verify(
                x => x.Publish(It.IsAny<DomainNotification>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenStudentNotFound()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var command = new AddRegistrationCommand(studentId, courseId);

            _userRepositoryMock
                .Setup(x => x.GetById(studentId))
                .ReturnsAsync((StudentUser?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _userRepositoryMock.Verify(
                x => x.GetById(studentId),
                Times.Once
            );

            _registrationRepositoryMock.Verify(
                x => x.AddRegistration(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);

            _mediatorMock.Verify(
                x => x.Publish(
                    It.Is<DomainNotification>(n => n.Value == "Aluno não encontrado."),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCommitFails()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var student = new StudentUserBuilder()
                .WithId(studentId)
                .Build();

            var command = new AddRegistrationCommand(studentId, courseId);

            _userRepositoryMock
                .Setup(x => x.GetById(studentId))
                .ReturnsAsync(student);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _registrationRepositoryMock.Verify(
                x => x.AddRegistration(studentId, courseId),
                Times.Once
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new AddRegistrationCommand(Guid.Empty, Guid.Empty); // IDs inválidos

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _userRepositoryMock.Verify(
                x => x.GetById(It.IsAny<Guid>()),
                Times.Never
            );

            _registrationRepositoryMock.Verify(
                x => x.AddRegistration(It.IsAny<Guid>(), It.IsAny<Guid>()),
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
            var command = new AddRegistrationCommand(Guid.Empty, Guid.Empty);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            // Deve publicar notificações para cada erro de validação
            _mediatorMock.Verify(
                x => x.Publish(It.IsAny<DomainNotification>(), It.IsAny<CancellationToken>()),
                Times.AtLeast(2) // 2 erros de validação (StudentId e CourseId)
            );
        }

        [Fact]
        public async Task Handle_ShouldNotCallRepository_WhenStudentIdIsEmpty()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var command = new AddRegistrationCommand(Guid.Empty, courseId);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _registrationRepositoryMock.Verify(
                x => x.AddRegistration(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_ShouldNotCallRepository_WhenCourseIdIsEmpty()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var command = new AddRegistrationCommand(studentId, Guid.Empty);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _registrationRepositoryMock.Verify(
                x => x.AddRegistration(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_ShouldCallGetById_BeforeAddingRegistration()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var student = new StudentUserBuilder()
                .WithId(studentId)
                .Build();

            var command = new AddRegistrationCommand(studentId, courseId);

            var callOrder = new List<string>();

            _userRepositoryMock
                .Setup(x => x.GetById(studentId))
                .Callback(() => callOrder.Add("GetById"))
                .ReturnsAsync(student);

            _registrationRepositoryMock
                .Setup(x => x.AddRegistration(studentId, courseId))
                .Callback(() => callOrder.Add("AddRegistration"));

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .Callback(() => callOrder.Add("Commit"))
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            callOrder.Should().HaveCount(3);
            callOrder[0].Should().Be("GetById");
            callOrder[1].Should().Be("AddRegistration");
            callOrder[2].Should().Be("Commit");
        }

        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012", "87654321-4321-4321-4321-210987654321")]
        [InlineData("11111111-1111-1111-1111-111111111111", "22222222-2222-2222-2222-222222222222")]
        public async Task Handle_ShouldAddRegistration_WithDifferentValidIds(string studentIdStr, string courseIdStr)
        {
            // Arrange
            var studentId = Guid.Parse(studentIdStr);
            var courseId = Guid.Parse(courseIdStr);

            var student = new StudentUserBuilder()
                .WithId(studentId)
                .Build();

            var command = new AddRegistrationCommand(studentId, courseId);

            _userRepositoryMock
                .Setup(x => x.GetById(studentId))
                .ReturnsAsync(student);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _registrationRepositoryMock.Verify(
                x => x.AddRegistration(studentId, courseId),
                Times.Once
            );
        }
    }
}
