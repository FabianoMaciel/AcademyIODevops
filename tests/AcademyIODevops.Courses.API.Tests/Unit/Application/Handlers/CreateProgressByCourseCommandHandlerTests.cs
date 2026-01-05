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
    public class CreateProgressByCourseCommandHandlerTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<ILessonRepository> _lessonRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CourseCommandHandler _handler;

        public CreateProgressByCourseCommandHandlerTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _lessonRepositoryMock = new Mock<ILessonRepository>();
            _mediatorMock = new Mock<IMediator>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _lessonRepositoryMock
                .Setup(x => x.UnitOfWork)
                .Returns(_unitOfWorkMock.Object);

            _handler = new CourseCommandHandler(
                _courseRepositoryMock.Object,
                _lessonRepositoryMock.Object,
                _mediatorMock.Object
            );
        }

        [Fact]
        public async Task Handle_ShouldCreateProgress_WhenCommandIsValid()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var command = new CreateProgressByCourseCommand(courseId, studentId);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _lessonRepositoryMock.Verify(
                x => x.CreateProgressLessonByCourse(courseId, studentId),
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
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var command = new CreateProgressByCourseCommand(courseId, studentId);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _lessonRepositoryMock.Verify(
                x => x.CreateProgressLessonByCourse(courseId, studentId),
                Times.Once
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new CreateProgressByCourseCommand(Guid.Empty, Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _lessonRepositoryMock.Verify(
                x => x.CreateProgressLessonByCourse(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldCallRepositoryWithCorrectParameters_WhenCalled()
        {
            // Arrange
            var courseId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
            var studentId = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8");
            var command = new CreateProgressByCourseCommand(courseId, studentId);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _lessonRepositoryMock.Verify(
                x => x.CreateProgressLessonByCourse(
                    It.Is<Guid>(id => id == courseId),
                    It.Is<Guid>(id => id == studentId)
                ),
                Times.Once
            );
        }

        [Theory]
        [InlineData("a1b2c3d4-e5f6-4a5b-8c7d-9e0f1a2b3c4d", "b2c3d4e5-f6a7-4b5c-8d7e-9f0a1b2c3d4e")]
        [InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479", "6ba7b811-9dad-11d1-80b4-00c04fd430c8")]
        public async Task Handle_ShouldSucceed_WithDifferentValidGuids(string courseIdStr, string studentIdStr)
        {
            // Arrange
            var courseId = Guid.Parse(courseIdStr);
            var studentId = Guid.Parse(studentIdStr);
            var command = new CreateProgressByCourseCommand(courseId, studentId);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _lessonRepositoryMock.Verify(
                x => x.CreateProgressLessonByCourse(courseId, studentId),
                Times.Once
            );
        }
    }
}
