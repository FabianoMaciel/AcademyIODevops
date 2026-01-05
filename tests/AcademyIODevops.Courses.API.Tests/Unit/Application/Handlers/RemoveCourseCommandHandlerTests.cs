using AcademyIODevops.Core.Data;
using AcademyIODevops.Core.Messages.Notifications;
using AcademyIODevops.Courses.API.Application.Commands;
using AcademyIODevops.Courses.API.Application.Handlers;
using AcademyIODevops.Courses.API.Models;
using AcademyIODevops.Courses.API.Tests.Builders;
using FluentAssertions;
using MediatR;
using Moq;

namespace AcademyIODevops.Courses.API.Tests.Unit.Application.Handlers
{
    public class RemoveCourseCommandHandlerTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly Mock<ILessonRepository> _lessonRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CourseCommandHandler _handler;

        public RemoveCourseCommandHandlerTests()
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
        public async Task Handle_ShouldRemoveCourse_WhenCommandIsValid()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var existingCourse = new CourseBuilder()
                .WithId(courseId)
                .WithName("DevOps Course")
                .Build();

            var command = new RemoveCourseCommand(courseId);

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
                x => x.GetById(courseId),
                Times.Once
            );

            _courseRepositoryMock.Verify(
                x => x.Delete(It.Is<Course>(c => c.Id == courseId)),
                Times.Once
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);

            _mediatorMock.Verify(
                x => x.Publish(It.IsAny<DomainNotification>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCourseNotFound()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var command = new RemoveCourseCommand(courseId);

            _courseRepositoryMock
                .Setup(x => x.GetById(courseId))
                .ReturnsAsync((Course?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _courseRepositoryMock.Verify(
                x => x.GetById(courseId),
                Times.Once
            );

            _courseRepositoryMock.Verify(
                x => x.Delete(It.IsAny<Course>()),
                Times.Never
            );

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
        public async Task Handle_ShouldReturnFalse_WhenCommitFails()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var existingCourse = new CourseBuilder()
                .WithId(courseId)
                .Build();

            var command = new RemoveCourseCommand(courseId);

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

            _courseRepositoryMock.Verify(
                x => x.Delete(It.Is<Course>(c => c.Id == courseId)),
                Times.Once
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldReturnFalse_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new RemoveCourseCommand(Guid.Empty);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeFalse();

            _courseRepositoryMock.Verify(
                x => x.GetById(It.IsAny<Guid>()),
                Times.Never
            );

            _courseRepositoryMock.Verify(
                x => x.Delete(It.IsAny<Course>()),
                Times.Never
            );

            _unitOfWorkMock.Verify(x => x.Commit(), Times.Never);
        }
    }
}
