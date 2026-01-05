using AcademyIODevops.Core.Enums;
using AcademyIODevops.Courses.API.Application.Queries;
using AcademyIODevops.Courses.API.Models;
using AcademyIODevops.Courses.API.Tests.Builders;
using FluentAssertions;
using Moq;

namespace AcademyIODevops.Courses.API.Tests.Unit.Application.Queries
{
    public class LessonQueryTests
    {
        private readonly Mock<ILessonRepository> _lessonRepositoryMock;
        private readonly LessonQuery _query;

        public LessonQueryTests()
        {
            _lessonRepositoryMock = new Mock<ILessonRepository>();
            _query = new LessonQuery(_lessonRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllLessons_WhenLessonsExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var lessons = new List<Lesson>
            {
                new LessonBuilder()
                    .WithName("Docker Basics")
                    .WithSubject("Containers")
                    .WithCourseId(courseId)
                    .WithTotalHours(2.5)
                    .Build(),
                new LessonBuilder()
                    .WithName("Docker Compose")
                    .WithSubject("Orchestration")
                    .WithCourseId(courseId)
                    .WithTotalHours(3.0)
                    .Build()
            };

            _lessonRepositoryMock
                .Setup(x => x.GetAll())
                .ReturnsAsync(lessons);

            // Act
            var result = await _query.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            var resultList = result.ToList();
            resultList[0].Name.Should().Be("Docker Basics");
            resultList[0].Subject.Should().Be("Containers");
            resultList[0].TotalHours.Should().Be(2.5);
            resultList[1].Name.Should().Be("Docker Compose");
            resultList[1].Subject.Should().Be("Orchestration");
            resultList[1].TotalHours.Should().Be(3.0);

            _lessonRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoLessonsExist()
        {
            // Arrange
            _lessonRepositoryMock
                .Setup(x => x.GetAll())
                .ReturnsAsync(new List<Lesson>());

            // Act
            var result = await _query.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _lessonRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetByCourseId_ShouldReturnLessons_WhenLessonsExistForCourse()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var lessons = new List<Lesson>
            {
                new LessonBuilder()
                    .WithName("Introduction")
                    .WithSubject("DevOps")
                    .WithCourseId(courseId)
                    .WithTotalHours(1.5)
                    .Build(),
                new LessonBuilder()
                    .WithName("Advanced Topics")
                    .WithSubject("DevOps")
                    .WithCourseId(courseId)
                    .WithTotalHours(4.0)
                    .Build()
            };

            _lessonRepositoryMock
                .Setup(x => x.GetByCourseId(courseId))
                .ReturnsAsync(lessons);

            // Act
            var result = await _query.GetByCourseId(courseId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            var resultList = result.ToList();
            resultList.Should().AllSatisfy(l => l.CourseId.Should().Be(courseId));

            _lessonRepositoryMock.Verify(x => x.GetByCourseId(courseId), Times.Once);
        }

        [Fact]
        public async Task GetByCourseId_ShouldReturnEmptyList_WhenNoLessonsExistForCourse()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            _lessonRepositoryMock
                .Setup(x => x.GetByCourseId(courseId))
                .ReturnsAsync(new List<Lesson>());

            // Act
            var result = await _query.GetByCourseId(courseId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _lessonRepositoryMock.Verify(x => x.GetByCourseId(courseId), Times.Once);
        }

        [Fact]
        public void ExistsProgress_ShouldReturnTrue_WhenProgressExists()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            _lessonRepositoryMock
                .Setup(x => x.ExistProgress(lessonId, studentId))
                .Returns(true);

            // Act
            var result = _query.ExistsProgress(lessonId, studentId);

            // Assert
            result.Should().BeTrue();

            _lessonRepositoryMock.Verify(
                x => x.ExistProgress(lessonId, studentId),
                Times.Once
            );
        }

        [Fact]
        public void ExistsProgress_ShouldReturnFalse_WhenProgressDoesNotExist()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            _lessonRepositoryMock
                .Setup(x => x.ExistProgress(lessonId, studentId))
                .Returns(false);

            // Act
            var result = _query.ExistsProgress(lessonId, studentId);

            // Assert
            result.Should().BeFalse();

            _lessonRepositoryMock.Verify(
                x => x.ExistProgress(lessonId, studentId),
                Times.Once
            );
        }

        [Fact]
        public void GetProgressStatusLesson_ShouldReturnStatus_WhenCalled()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var expectedStatus = EProgressLesson.InProgress;

            _lessonRepositoryMock
                .Setup(x => x.GetProgressStatusLesson(lessonId, studentId))
                .Returns(expectedStatus);

            // Act
            var result = _query.GetProgressStatusLesson(lessonId, studentId);

            // Assert
            result.Should().Be(expectedStatus);

            _lessonRepositoryMock.Verify(
                x => x.GetProgressStatusLesson(lessonId, studentId),
                Times.Once
            );
        }

        [Theory]
        [InlineData(EProgressLesson.NotStarted)]
        [InlineData(EProgressLesson.InProgress)]
        [InlineData(EProgressLesson.Completed)]
        public void GetProgressStatusLesson_ShouldReturnCorrectStatus_ForDifferentStatuses(
            EProgressLesson status)
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            _lessonRepositoryMock
                .Setup(x => x.GetProgressStatusLesson(lessonId, studentId))
                .Returns(status);

            // Act
            var result = _query.GetProgressStatusLesson(lessonId, studentId);

            // Assert
            result.Should().Be(status);
        }

        [Fact]
        public async Task GetProgress_ShouldReturnProgressList_WhenProgressExists()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var lesson1 = new LessonBuilder()
                .WithName("Docker Introduction")
                .Build();
            var lesson2 = new LessonBuilder()
                .WithName("Kubernetes Basics")
                .Build();

            var progressions = new List<ProgressLesson>
            {
                new ProgressLesson
                {
                    Lesson = lesson1,
                    ProgressionStatus = EProgressLesson.InProgress
                },
                new ProgressLesson
                {
                    Lesson = lesson2,
                    ProgressionStatus = EProgressLesson.Completed
                }
            };

            _lessonRepositoryMock
                .Setup(x => x.GetProgression(studentId))
                .ReturnsAsync(progressions);

            // Act
            var result = await _query.GetProgress(studentId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            var resultList = result.ToList();
            resultList[0].LessonName.Should().Be("Docker Introduction");
            resultList[1].LessonName.Should().Be("Kubernetes Basics");

            _lessonRepositoryMock.Verify(x => x.GetProgression(studentId), Times.Once);
        }

        [Fact]
        public async Task GetProgress_ShouldReturnEmptyList_WhenNoProgressExists()
        {
            // Arrange
            var studentId = Guid.NewGuid();

            _lessonRepositoryMock
                .Setup(x => x.GetProgression(studentId))
                .ReturnsAsync(new List<ProgressLesson>());

            // Act
            var result = await _query.GetProgress(studentId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _lessonRepositoryMock.Verify(x => x.GetProgression(studentId), Times.Once);
        }
    }
}
