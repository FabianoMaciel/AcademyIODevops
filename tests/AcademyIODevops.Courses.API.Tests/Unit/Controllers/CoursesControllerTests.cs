using AcademyIODevops.Core.Messages.Integration;
using AcademyIODevops.Courses.API.Application.Commands;
using AcademyIODevops.Courses.API.Application.Queries;
using AcademyIODevops.Courses.API.Application.Queries.ViewModels;
using AcademyIODevops.Courses.API.Controllers;
using AcademyIODevops.Courses.API.Models.ViewModels;
using AcademyIODevops.MessageBus;
using AcademyIODevops.WebAPI.Core.User;
using FluentAssertions;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AcademyIODevops.Courses.API.Tests.Unit.Controllers;

public class CoursesControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ICourseQuery> _courseQueryMock;
    private readonly Mock<IAspNetUser> _aspNetUserMock;
    private readonly Mock<IMessageBus> _messageBusMock;
    private readonly CoursesController _controller;

    public CoursesControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _courseQueryMock = new Mock<ICourseQuery>();
        _aspNetUserMock = new Mock<IAspNetUser>();
        _messageBusMock = new Mock<IMessageBus>();

        _controller = new CoursesController(
            _mediatorMock.Object,
            _courseQueryMock.Object,
            _aspNetUserMock.Object,
            _messageBusMock.Object
        );
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkWithCourses()
    {
        // Arrange
        var courses = new List<CourseViewModel>
        {
            new() { Id = Guid.NewGuid(), Name = "Course 1", Description = "Desc 1", Price = 100 },
            new() { Id = Guid.NewGuid(), Name = "Course 2", Description = "Desc 2", Price = 200 }
        };

        _courseQueryMock.Setup(x => x.GetAll()).ReturnsAsync(courses);

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _courseQueryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoCourses()
    {
        // Arrange
        _courseQueryMock.Setup(x => x.GetAll()).ReturnsAsync(new List<CourseViewModel>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _courseQueryMock.Verify(x => x.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldReturnOkWithCourse()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new CourseViewModel
        {
            Id = courseId,
            Name = "Test Course",
            Description = "Test Description",
            Price = 150
        };

        _courseQueryMock.Setup(x => x.GetById(courseId)).ReturnsAsync(course);

        // Act
        var result = await _controller.GetById(courseId);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _courseQueryMock.Verify(x => x.GetById(courseId), Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldCallQueryWithCorrectId()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _courseQueryMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync((CourseViewModel)null!);

        // Act
        await _controller.GetById(courseId);

        // Assert
        _courseQueryMock.Verify(x => x.GetById(courseId), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldSendAddCourseCommand()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var course = new CourseViewModel
        {
            Name = "New Course",
            Description = "New Description",
            Price = 300
        };

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);

        // Act
        await _controller.Create(course);

        // Assert
        _mediatorMock.Verify(x => x.Send(
            It.Is<AddCourseCommand>(c =>
                c.Name == course.Name &&
                c.Description == course.Description &&
                c.Price == course.Price &&
                c.UserCreationId == userId
            ), default), Times.Once);
    }

    [Fact]
    public async Task Create_ShouldReturnOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var course = new CourseViewModel
        {
            Name = "New Course",
            Description = "New Description",
            Price = 300
        };

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);

        // Act
        var result = await _controller.Create(course);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Update_ShouldSendUpdateCourseCommand()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var course = new CourseViewModel
        {
            Id = Guid.NewGuid(),
            Name = "Updated Course",
            Description = "Updated Description",
            Price = 400
        };

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);

        // Act
        await _controller.Update(course);

        // Assert
        _mediatorMock.Verify(x => x.Send(
            It.Is<UpdateCourseCommand>(c =>
                c.Name == course.Name &&
                c.Description == course.Description &&
                c.Price == course.Price &&
                c.CourseId == course.Id &&
                c.UserCreationId == userId
            ), default), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldReturnOkResult()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var course = new CourseViewModel
        {
            Id = Guid.NewGuid(),
            Name = "Updated Course",
            Description = "Updated Description",
            Price = 400
        };

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);

        // Act
        var result = await _controller.Update(course);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Remove_ShouldSendRemoveCourseCommand()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        // Act
        await _controller.Remove(courseId);

        // Assert
        _mediatorMock.Verify(x => x.Send(
            It.Is<RemoveCourseCommand>(c => c.CourseId == courseId), default), Times.Once);
    }

    [Fact]
    public async Task Remove_ShouldReturnOkResult()
    {
        // Arrange
        var courseId = Guid.NewGuid();

        // Act
        var result = await _controller.Remove(courseId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task MakePayment_ShouldCallMessageBusWithCorrectEvent()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var payment = new PaymentViewModel
        {
            CardName = "John Doe",
            CardNumber = "4111111111111111",
            CardExpirationDate = "12/25",
            CardCVV = "123"
        };

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);
        _messageBusMock
            .Setup(x => x.RequestAsync<PaymentRegisteredIntegrationEvent, ResponseMessage>(It.IsAny<PaymentRegisteredIntegrationEvent>()))
            .ReturnsAsync(new ResponseMessage(new ValidationResult()));

        // Act
        await _controller.MakePayment(courseId, payment);

        // Assert
        _messageBusMock.Verify(x => x.RequestAsync<PaymentRegisteredIntegrationEvent, ResponseMessage>(
            It.Is<PaymentRegisteredIntegrationEvent>(e =>
                e.CourseId == courseId &&
                e.StudentId == userId &&
                e.CardName == payment.CardName &&
                e.CardNumber == payment.CardNumber &&
                e.CardExpirationDate == payment.CardExpirationDate &&
                e.CardCVV == payment.CardCVV
            )), Times.Once);
    }

    [Fact]
    public async Task MakePayment_ShouldReturnResponseMessage()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var payment = new PaymentViewModel
        {
            CardName = "Jane Smith",
            CardNumber = "5500000000000004",
            CardExpirationDate = "06/26",
            CardCVV = "456"
        };

        var expectedResponse = new ResponseMessage(new ValidationResult());

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);
        _messageBusMock
            .Setup(x => x.RequestAsync<PaymentRegisteredIntegrationEvent, ResponseMessage>(It.IsAny<PaymentRegisteredIntegrationEvent>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.MakePayment(courseId, payment);

        // Assert
        result.Should().Be(expectedResponse);
    }

    [Theory]
    [InlineData(100.00)]
    [InlineData(500.50)]
    [InlineData(1000.00)]
    public async Task Create_ShouldHandleDifferentPrices(double price)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var course = new CourseViewModel
        {
            Name = "Course",
            Description = "Description",
            Price = price
        };

        _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);

        // Act
        await _controller.Create(course);

        // Assert
        _mediatorMock.Verify(x => x.Send(
            It.Is<AddCourseCommand>(c => c.Price == price), default), Times.Once);
    }
}
