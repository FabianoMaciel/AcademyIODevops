using AcademyIODevops.Core.Data;
using AcademyIODevops.Core.Messages.Notifications;
using AcademyIODevops.Courses.API.Application.Commands;
using AcademyIODevops.Courses.API.Application.Handlers;
using AcademyIODevops.Courses.API.Models;
using FluentAssertions;
using MediatR;
using Moq;

namespace AcademyIODevops.Courses.API.Tests.Unit.Application.Handlers
{
    public class AddCourseCommandHandlerTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<ILessonRepository> _lessonRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CourseCommandHandler _handler;

        public AddCourseCommandHandlerTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _lessonRepositoryMock = new Mock<ILessonRepository>();
            _mediatorMock = new Mock<IMediator>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            // Setup UnitOfWork
            _courseRepositoryMock
                .Setup(x => x.UnitOfWork)
                .Returns(_unitOfWorkMock.Object);

            _handler = new CourseCommandHandler(
                _courseRepositoryMock.Object,
                _lessonRepositoryMock.Object,
                _mediatorMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldAddCourse_WhenCommandIsValid()
        {
            // Arrange
            var command = new AddCourseCommand(
                name: "DevOps Fundamentals",
                description: "Learn Docker and Kubernetes",
                userCreationId: Guid.NewGuid(),
                price: 99.99
            );

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _courseRepositoryMock.Verify(
                x => x.Add(It.Is<Course>(c =>
                    c.Name == command.Name &&
                    c.Description == command.Description &&
                    c.Price == command.Price
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
            var command = new AddCourseCommand(
                name: "Test Course",
                description: "Test Description",
                userCreationId: Guid.NewGuid(),
                price: 149.99
            );

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _courseRepositoryMock.Verify(
                x => x.Add(It.IsAny<Course>()),
                Times.Once
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new AddCourseCommand(
                name: string.Empty, // Nome inválido
                description: "Valid Description",
                userCreationId: Guid.NewGuid(),
                price: 99.99
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _courseRepositoryMock.Verify(
                x => x.Add(It.IsAny<Course>()),
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
            var command = new AddCourseCommand(
                name: string.Empty,
                description: string.Empty,
                userCreationId: Guid.Empty,
                price: -10
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
        [InlineData("Docker Course", "Learn Docker", 50.00)]
        [InlineData("Kubernetes Course", "Learn K8s", 150.00)]
        [InlineData("CI/CD Pipeline", "DevOps automation", 99.99)]
        public async Task Handle_ShouldAddCourse_WithDifferentValidData(
            string name, string description, double price)
        {
            // Arrange
            var command = new AddCourseCommand(
                name: name,
                description: description,
                userCreationId: Guid.NewGuid(),
                price: price
            );

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _courseRepositoryMock.Verify(
                x => x.Add(It.Is<Course>(c =>
                    c.Name == name &&
                    c.Description == description &&
                    c.Price == price
                )),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_ShouldGenerateNewCourseId_WhenCreatingCourse()
        {
            // Arrange
            var command = new AddCourseCommand(
                name: "Test Course",
                description: "Test Description",
                userCreationId: Guid.NewGuid(),
                price: 99.99
            );

            Course capturedCourse = null;

            _courseRepositoryMock
                .Setup(x => x.Add(It.IsAny<Course>()))
                .Callback<Course>(c => capturedCourse = c);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            capturedCourse.Should().NotBeNull();
            capturedCourse.Id.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldNotCallRepository_WhenPriceIsInvalid()
        {
            // Arrange
            var command = new AddCourseCommand(
                name: "Valid Name",
                description: "Valid Description",
                userCreationId: Guid.NewGuid(),
                price: 0 // Preço inválido
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _courseRepositoryMock.Verify(
                x => x.Add(It.IsAny<Course>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_ShouldCallCommit_OnlyAfterAddingCourse()
        {
            // Arrange
            var command = new AddCourseCommand(
                name: "Test Course",
                description: "Test Description",
                userCreationId: Guid.NewGuid(),
                price: 99.99
            );

            var callOrder = new List<string>();

            _courseRepositoryMock
                .Setup(x => x.Add(It.IsAny<Course>()))
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
