using AcademyIODevops.Core.Enums;
using AcademyIODevops.Courses.API.Application.Commands;
using AcademyIODevops.Courses.API.Application.Queries;
using AcademyIODevops.Courses.API.Application.Queries.ViewModels;
using AcademyIODevops.Courses.API.Controllers;
using AcademyIODevops.WebAPI.Core.User;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AcademyIODevops.Courses.API.Tests.Unit.Controllers;

public class LessonsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILessonQuery> _lessonQueryMock;
    private readonly Mock<IAspNetUser> _aspNetUserMock;
    private readonly LessonsController _controller;

    public LessonsControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _lessonQueryMock = new Mock<ILessonQuery>();
        _aspNetUserMock = new Mock<IAspNetUser>();

        _controller = new LessonsController(
            _mediatorMock.Object,
            _lessonQueryMock.Object,
            _aspNetUserMock.Object
        );
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithLessons()
    {
        // Arrange
        var lessons = new List<LessonViewModel>
        {
            new() { Id = Guid.NewGuid(), Name = "Lesson 1", Subject = "Subject 1" },
            new() { Id = Guid.NewGuid(), Name = "Lesson 2", Subject = "Subject 2" }
        };

        _lessonQueryMock.Setup(x => x.GetAll()).ReturnsAsync(lessons);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _lessonQueryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoLessons()
    {
        // Arrange
        _lessonQueryMock.Setup(x => x.GetAll()).ReturnsAsync(new List<LessonViewModel>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _lessonQueryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetByCourseId_ShouldReturnOkWithLessons()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var lessons = new List<LessonViewModel>
        {
            new() { Id = Guid.NewGuid(), Name = "Lesson 1", CourseId = courseId },
            new() { Id = Guid.NewGuid(), Name = "Lesson 2", CourseId = courseId }
        };

        _lessonQueryMock.Setup(x => x.GetByCourseId(courseId)).ReturnsAsync(lessons);

        // Act
        var result = await _controller.GetByCourseId(courseId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _lessonQueryMock.Verify(x => x.GetByCourseId(courseId), Times.Once);
    }

    [Fact]
    public async Task GetProgress_ShouldReturnOkWithProgress()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var progress = new List<LessonProgressViewModel>
        {
            new("Lesson 1", "InProgress")
        };

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);
        _lessonQueryMock.Setup(x => x.GetProgress(userId)).ReturnsAsync(progress);

        // Act
        var result = await _controller.GetProgress();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        _lessonQueryMock.Verify(x => x.GetProgress(userId), Times.Once);
    }

    [Fact]
    public async Task Add_ShouldSendAddLessonCommand()
    {
        // Arrange
        var lesson = new LessonViewModel
        {
            Name = "New Lesson",
            Subject = "New Subject",
            CourseId = Guid.NewGuid(),
            TotalHours = 10
        };

        // Act
        await _controller.Add(lesson);

        // Assert
        _mediatorMock.Verify(x => x.Send(
            It.Is<AddLessonCommand>(c =>
                c.Name == lesson.Name &&
                c.Subject == lesson.Subject &&
                c.CourseId == lesson.CourseId &&
                c.TotalHours == lesson.TotalHours
            ), default), Times.Once);
    }

    [Fact]
    public async Task Add_ShouldReturnOkResult()
    {
        // Arrange
        var lesson = new LessonViewModel
        {
            Name = "New Lesson",
            Subject = "New Subject",
            CourseId = Guid.NewGuid(),
            TotalHours = 10
        };

        // Act
        var result = await _controller.Add(lesson);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task StartClass_ShouldReturnBadRequest_WhenProgressDoesNotExist()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);
        _lessonQueryMock.Setup(x => x.ExistsProgress(lessonId, userId)).Returns(false);

        // Act
        var result = await _controller.StartClass(lessonId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _mediatorMock.Verify(x => x.Send(It.IsAny<StartLessonCommand>(), default), Times.Never);
    }

    [Fact]
    public async Task StartClass_ShouldReturnBadRequest_WhenLessonAlreadyCompleted()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);
        _lessonQueryMock.Setup(x => x.ExistsProgress(lessonId, userId)).Returns(true);
        _lessonQueryMock.Setup(x => x.GetProgressStatusLesson(lessonId, userId)).Returns(EProgressLesson.Completed);

        // Act
        var result = await _controller.StartClass(lessonId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _mediatorMock.Verify(x => x.Send(It.IsAny<StartLessonCommand>(), default), Times.Never);
    }

    [Fact]
    public async Task StartClass_ShouldSendCommand_WhenValidRequest()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);
        _lessonQueryMock.Setup(x => x.ExistsProgress(lessonId, userId)).Returns(true);
        _lessonQueryMock.Setup(x => x.GetProgressStatusLesson(lessonId, userId)).Returns(EProgressLesson.NotStarted);

        // Act
        await _controller.StartClass(lessonId);

        // Assert
        _mediatorMock.Verify(x => x.Send(
            It.Is<StartLessonCommand>(c => c.LessonId == lessonId && c.StudentId == userId), default), Times.Once);
    }

    [Fact]
    public async Task StartClass_ShouldReturnOkResult_WhenSuccessful()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);
        _lessonQueryMock.Setup(x => x.ExistsProgress(lessonId, userId)).Returns(true);
        _lessonQueryMock.Setup(x => x.GetProgressStatusLesson(lessonId, userId)).Returns(EProgressLesson.NotStarted);

        // Act
        var result = await _controller.StartClass(lessonId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task FinishClass_ShouldReturnBadRequest_WhenProgressDoesNotExist()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);
        _lessonQueryMock.Setup(x => x.ExistsProgress(lessonId, userId)).Returns(false);

        // Act
        var result = await _controller.FinishClass(lessonId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _mediatorMock.Verify(x => x.Send(It.IsAny<FinishLessonCommand>(), default), Times.Never);
    }

    [Fact]
    public async Task FinishClass_ShouldReturnBadRequest_WhenLessonNotStarted()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);
        _lessonQueryMock.Setup(x => x.ExistsProgress(lessonId, userId)).Returns(true);
        _lessonQueryMock.Setup(x => x.GetProgressStatusLesson(lessonId, userId)).Returns(EProgressLesson.NotStarted);

        // Act
        var result = await _controller.FinishClass(lessonId);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        _mediatorMock.Verify(x => x.Send(It.IsAny<FinishLessonCommand>(), default), Times.Never);
    }

    [Fact]
    public async Task FinishClass_ShouldSendCommand_WhenValidRequest()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);
        _lessonQueryMock.Setup(x => x.ExistsProgress(lessonId, userId)).Returns(true);
        _lessonQueryMock.Setup(x => x.GetProgressStatusLesson(lessonId, userId)).Returns(EProgressLesson.InProgress);

        // Act
        await _controller.FinishClass(lessonId);

        // Assert
        _mediatorMock.Verify(x => x.Send(
            It.Is<FinishLessonCommand>(c => c.LessonId == lessonId && c.StudentId == userId), default), Times.Once);
    }

    [Fact]
    public async Task FinishClass_ShouldReturnOkResult_WhenSuccessful()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);
        _lessonQueryMock.Setup(x => x.ExistsProgress(lessonId, userId)).Returns(true);
        _lessonQueryMock.Setup(x => x.GetProgressStatusLesson(lessonId, userId)).Returns(EProgressLesson.InProgress);

        // Act
        var result = await _controller.FinishClass(lessonId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Theory]
    [InlineData(5)]
    [InlineData(10)]
    [InlineData(20)]
    public async Task Add_ShouldHandleDifferentTotalHours(int totalHours)
    {
        // Arrange
        var lesson = new LessonViewModel
        {
            Name = "Lesson",
            Subject = "Subject",
            CourseId = Guid.NewGuid(),
            TotalHours = totalHours
        };

        // Act
        await _controller.Add(lesson);

        // Assert
        _mediatorMock.Verify(x => x.Send(
            It.Is<AddLessonCommand>(c => c.TotalHours == totalHours), default), Times.Once);
    }
}
