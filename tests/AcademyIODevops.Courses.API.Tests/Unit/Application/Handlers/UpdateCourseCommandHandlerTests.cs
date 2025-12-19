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
    public class UpdateCourseCommandHandlerTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<ILessonRepository> _lessonRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CourseCommandHandler _handler;

        public UpdateCourseCommandHandlerTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _lessonRepositoryMock = new Mock<ILessonRepository>();
            _mediatorMock = new Mock<IMediator>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

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
        public async Task Handle_ShouldUpdateCourse_WhenCommandIsValidAndCourseExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var existingCourse = new Course
            {
                Id = courseId,
                Name = "Old Name",
                Description = "Old Description",
                Price = 50.00
            };

            var command = new UpdateCourseCommand(
                name: "Updated Name",
                description: "Updated Description",
                userCreationId: Guid.NewGuid(),
                price: 99.99,
                courseId: courseId
            );

            _courseRepositoryMock
                .Setup(x => x.GetById(courseId))
                .ReturnsAsync(existingCourse);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _courseRepositoryMock.Verify(x => x.GetById(courseId), Times.Once);
            _courseRepositoryMock.Verify(
                x => x.Update(It.Is<Course>(c =>
                    c.Id == courseId &&
                    c.Name == "Updated Name" &&
                    c.Description == "Updated Description" &&
                    c.Price == 99.99
                )),
                Times.Once
            );
            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCourseDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var command = new UpdateCourseCommand(
                name: "Updated Name",
                description: "Updated Description",
                userCreationId: Guid.NewGuid(),
                price: 99.99,
                courseId: courseId
            );

            _courseRepositoryMock
                .Setup(x => x.GetById(courseId))
                .ReturnsAsync((Course)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _courseRepositoryMock.Verify(x => x.GetById(courseId), Times.Once);
            _courseRepositoryMock.Verify(x => x.Update(It.IsAny<Course>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);

            _mediatorMock.Verify(
                x => x.Publish(
                    It.Is<DomainNotification>(n => n.Value == "Curso não encontrado."),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new UpdateCourseCommand(
                name: string.Empty, // Nome inválido
                description: "Valid Description",
                userCreationId: Guid.NewGuid(),
                price: 99.99,
                courseId: Guid.NewGuid()
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _courseRepositoryMock.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
            _courseRepositoryMock.Verify(x => x.Update(It.IsAny<Course>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCommitFails()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var existingCourse = new Course
            {
                Id = courseId,
                Name = "Original Name",
                Description = "Original Description",
                Price = 50.00
            };

            var command = new UpdateCourseCommand(
                name: "Updated Name",
                description: "Updated Description",
                userCreationId: Guid.NewGuid(),
                price: 99.99,
                courseId: courseId
            );

            _courseRepositoryMock
                .Setup(x => x.GetById(courseId))
                .ReturnsAsync(existingCourse);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _courseRepositoryMock.Verify(x => x.Update(It.IsAny<Course>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCourseIdIsEmpty()
        {
            // Arrange
            var command = new UpdateCourseCommand(
                name: "Valid Name",
                description: "Valid Description",
                userCreationId: Guid.NewGuid(),
                price: 99.99,
                courseId: Guid.Empty // ID inválido
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _courseRepositoryMock.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
            _mediatorMock.Verify(
                x => x.Publish(It.IsAny<DomainNotification>(), It.IsAny<CancellationToken>()),
                Times.AtLeastOnce
            );
        }

        [Fact]
        public async Task Handle_ShouldPreserveOriginalCourseId_WhenUpdating()
        {
            // Arrange
            var originalCourseId = Guid.NewGuid();
            var existingCourse = new Course
            {
                Id = originalCourseId,
                Name = "Old Name",
                Description = "Old Description",
                Price = 50.00
            };

            var command = new UpdateCourseCommand(
                name: "New Name",
                description: "New Description",
                userCreationId: Guid.NewGuid(),
                price: 150.00,
                courseId: originalCourseId
            );

            Course updatedCourse = null;

            _courseRepositoryMock
                .Setup(x => x.GetById(originalCourseId))
                .ReturnsAsync(existingCourse);

            _courseRepositoryMock
                .Setup(x => x.Update(It.IsAny<Course>()))
                .Callback<Course>(c => updatedCourse = c);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            updatedCourse.Should().NotBeNull();
            updatedCourse.Id.Should().Be(originalCourseId);
        }

        [Theory]
        [InlineData("Updated Docker", "New Docker content", 75.00)]
        [InlineData("Updated Kubernetes", "New K8s content", 125.00)]
        [InlineData("Updated CI/CD", "New DevOps content", 199.99)]
        public async Task Handle_ShouldUpdateCourse_WithDifferentValidData(
            string name, string description, double price)
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var existingCourse = new Course
            {
                Id = courseId,
                Name = "Original",
                Description = "Original",
                Price = 50.00
            };

            var command = new UpdateCourseCommand(
                name: name,
                description: description,
                userCreationId: Guid.NewGuid(),
                price: price,
                courseId: courseId
            );

            _courseRepositoryMock
                .Setup(x => x.GetById(courseId))
                .ReturnsAsync(existingCourse);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _courseRepositoryMock.Verify(
                x => x.Update(It.Is<Course>(c =>
                    c.Name == name &&
                    c.Description == description &&
                    c.Price == price
                )),
                Times.Once
            );
        }
    }
}
