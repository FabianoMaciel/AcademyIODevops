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
    public class LessonCommandHandlerTests
    {
        private readonly Mock<ILessonRepository> _lessonRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly LessonCommandHandler _handler;

        public LessonCommandHandlerTests()
        {
            _lessonRepositoryMock = new Mock<ILessonRepository>();
            _mediatorMock = new Mock<IMediator>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _lessonRepositoryMock
                .Setup(x => x.UnitOfWork)
                .Returns(_unitOfWorkMock.Object);

            _handler = new LessonCommandHandler(
                _lessonRepositoryMock.Object,
                _mediatorMock.Object
            );
        }

        #region AddLessonCommand Tests

        [Fact]
        public async Task Handle_ShouldAddLesson_WhenAddLessonCommandIsValid()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var command = new AddLessonCommand(
                name: "Docker Introduction",
                subject: "Containers",
                courseId: courseId,
                totalHours: 2.5
            );

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _lessonRepositoryMock.Verify(
                x => x.Add(It.Is<Lesson>(l =>
                    l.Name == command.Name &&
                    l.Subject == command.Subject &&
                    l.CourseId == command.CourseId &&
                    l.TotalHours == command.TotalHours
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
        public async Task Handle_ShouldReturnFalse_WhenAddLessonCommandIsInvalid()
        {
            // Arrange
            var command = new AddLessonCommand(
                name: "",
                subject: "Containers",
                courseId: Guid.NewGuid(),
                totalHours: 2.5
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _lessonRepositoryMock.Verify(
                x => x.Add(It.IsAny<Lesson>()),
                Times.Never
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenAddLessonCommitFails()
        {
            // Arrange
            var command = new AddLessonCommand(
                name: "Docker Introduction",
                subject: "Containers",
                courseId: Guid.NewGuid(),
                totalHours: 2.5
            );

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _lessonRepositoryMock.Verify(
                x => x.Add(It.IsAny<Lesson>()),
                Times.Once
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Theory]
        [InlineData("", "Containers", 2.5)]
        [InlineData("Docker Intro", "", 2.5)]
        [InlineData("Docker Intro", "Containers", 0)]
        [InlineData("Docker Intro", "Containers", -1)]
        public async Task Handle_ShouldReturnFalse_WhenAddLessonCommandHasInvalidData(
            string name, string subject, double totalHours)
        {
            // Arrange
            var command = new AddLessonCommand(name, subject, Guid.NewGuid(), totalHours);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _lessonRepositoryMock.Verify(
                x => x.Add(It.IsAny<Lesson>()),
                Times.Never
            );
        }

        #endregion

        #region StartLessonCommand Tests

        [Fact]
        public async Task Handle_ShouldStartLesson_WhenStartLessonCommandIsValid()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var command = new StartLessonCommand(lessonId, studentId);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _lessonRepositoryMock.Verify(
                x => x.StartLesson(lessonId, studentId),
                Times.Once
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);

            _mediatorMock.Verify(
                x => x.Publish(It.IsAny<DomainNotification>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenStartLessonCommandIsInvalid()
        {
            // Arrange
            var command = new StartLessonCommand(Guid.Empty, Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _lessonRepositoryMock.Verify(
                x => x.StartLesson(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenStartLessonCommitFails()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var command = new StartLessonCommand(lessonId, studentId);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _lessonRepositoryMock.Verify(
                x => x.StartLesson(lessonId, studentId),
                Times.Once
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        #endregion

        #region FinishLessonCommand Tests

        [Fact]
        public async Task Handle_ShouldFinishLesson_WhenFinishLessonCommandIsValid()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var command = new FinishLessonCommand(lessonId, studentId);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();

            _lessonRepositoryMock.Verify(
                x => x.FinishLesson(lessonId, studentId),
                Times.Once
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);

            _mediatorMock.Verify(
                x => x.Publish(It.IsAny<DomainNotification>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenFinishLessonCommandIsInvalid()
        {
            // Arrange
            var command = new FinishLessonCommand(Guid.Empty, Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _lessonRepositoryMock.Verify(
                x => x.FinishLesson(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenFinishLessonCommitFails()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var command = new FinishLessonCommand(lessonId, studentId);

            _unitOfWorkMock
                .Setup(x => x.Commit())
                .ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _lessonRepositoryMock.Verify(
                x => x.FinishLesson(lessonId, studentId),
                Times.Once
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")]
        public async Task Handle_ShouldReturnFalse_WhenFinishLessonCommandHasInvalidLessonId(string lessonIdString)
        {
            // Arrange
            var lessonId = Guid.Parse(lessonIdString);
            var command = new FinishLessonCommand(lessonId, Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _lessonRepositoryMock.Verify(
                x => x.FinishLesson(It.IsAny<Guid>(), It.IsAny<Guid>()),
                Times.Never
            );
        }

        #endregion
    }
}
