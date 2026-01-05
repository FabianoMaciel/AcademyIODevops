using AcademyIODevops.Bff.Controllers;
using AcademyIODevops.Bff.Models;
using AcademyIODevops.Bff.Services;
using AcademyIODevops.Core.Communication;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AcademyIODevops.Bff.Tests.Unit.Controllers
{
    public class CourseControllerTests
    {
        private readonly Mock<ICourseService> _courseServiceMock;
        private readonly CourseController _controller;

        public CourseControllerTests()
        {
            _courseServiceMock = new Mock<ICourseService>();
            _controller = new CourseController(_courseServiceMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WithCourses()
        {
            // Arrange
            var courses = new List<CourseViewModel>
            {
                new() { Id = Guid.NewGuid(), Name = "Docker Course", Description = "Learn Docker", Price = 99.99 },
                new() { Id = Guid.NewGuid(), Name = "Kubernetes Course", Description = "Learn Kubernetes", Price = 149.99 }
            };

            _courseServiceMock.Setup(x => x.GetAll()).ReturnsAsync(courses);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCourses = okResult.Value.Should().BeAssignableTo<List<CourseViewModel>>().Subject;
            returnedCourses.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetById_ShouldReturnOkResult_WhenCourseExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseViewModel { Id = courseId, Name = "Docker Course", Description = "Learn Docker", Price = 99.99 };

            _courseServiceMock.Setup(x => x.GetById(courseId)).ReturnsAsync(course);

            // Act
            var result = await _controller.GetById(courseId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedCourse = okResult.Value.Should().BeAssignableTo<CourseViewModel>().Subject;
            returnedCourse.Id.Should().Be(courseId);
            returnedCourse.Name.Should().Be("Docker Course");
        }

        [Fact]
        public async Task CreateCourse_ShouldReturnOkResult_WhenCourseIsCreated()
        {
            // Arrange
            var course = new CourseViewModel { Name = "New Course", Description = "New Description", Price = 199.99 };
            var response = new ResponseResult();

            _courseServiceMock.Setup(x => x.Create(course)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreateCourse(course);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _courseServiceMock.Verify(x => x.Create(course), Times.Once);
        }

        [Fact]
        public async Task UpdateCourse_ShouldReturnOkResult_WhenCourseIsUpdated()
        {
            // Arrange
            var course = new CourseViewModel { Id = Guid.NewGuid(), Name = "Updated Course", Description = "Updated Description", Price = 249.99 };
            var response = new ResponseResult();

            _courseServiceMock.Setup(x => x.Update(course)).ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateCourse(course);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _courseServiceMock.Verify(x => x.Update(course), Times.Once);
        }

        [Fact]
        public async Task RemoveCourse_ShouldReturnOkResult_WhenCourseIsRemoved()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var response = new ResponseResult();

            _courseServiceMock.Setup(x => x.Remove(courseId)).ReturnsAsync(response);

            // Act
            var result = await _controller.RemoveCourse(courseId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _courseServiceMock.Verify(x => x.Remove(courseId), Times.Once);
        }

        [Fact]
        public async Task MakePaymentCourse_ShouldReturnOkResult_WhenPaymentIsSuccessful()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var payment = new PaymentViewModel
            {
                CardName = "John Doe",
                CardNumber = "4532015112830366",
                CardExpirationDate = "12/2025",
                CardCVV = "123"
            };
            var response = new ResponseResult();

            _courseServiceMock.Setup(x => x.MakePayment(courseId, payment)).ReturnsAsync(response);

            // Act
            var result = await _controller.MakePaymentCourse(courseId, payment);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _courseServiceMock.Verify(x => x.MakePayment(courseId, payment), Times.Once);
        }

        [Fact]
        public async Task GetLessonByCourse_ShouldReturnOkResult_WithLessons()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var lessons = new List<LessonViewModel>
            {
                new() { Id = Guid.NewGuid(), Name = "Lesson 1", Subject = "Intro", TotalHours = 2.0, CourseId = courseId },
                new() { Id = Guid.NewGuid(), Name = "Lesson 2", Subject = "Advanced", TotalHours = 3.0, CourseId = courseId }
            };

            _courseServiceMock.Setup(x => x.GetLessonByCourse(courseId)).ReturnsAsync(lessons);

            // Act
            var result = await _controller.GetLessonByCourse(courseId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedLessons = okResult.Value.Should().BeAssignableTo<List<LessonViewModel>>().Subject;
            returnedLessons.Should().HaveCount(2);
            returnedLessons.Should().AllSatisfy(l => l.CourseId.Should().Be(courseId));
        }

        [Fact]
        public async Task GetProgressLesson_ShouldReturnOkResult_WithProgress()
        {
            // Arrange
            var progress = new List<LessonProgressViewModel>
            {
                new() { LessonName = "Lesson 1", ProgressLesson = "Not Started" },
                new() { LessonName = "Lesson 2", ProgressLesson = "In Progress" }
            };

            _courseServiceMock.Setup(x => x.GetProgressLesson()).ReturnsAsync(progress);

            // Act
            var result = await _controller.GetProgressLesson();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedProgress = okResult.Value.Should().BeAssignableTo<List<LessonProgressViewModel>>().Subject;
            returnedProgress.Should().HaveCount(2);
        }

        [Fact]
        public async Task CreateLesson_ShouldReturnOkResult_WhenLessonIsCreated()
        {
            // Arrange
            var lesson = new LessonViewModel { Name = "New Lesson", Subject = "Subject", TotalHours = 2.5, CourseId = Guid.NewGuid() };
            var response = new ResponseResult();

            _courseServiceMock.Setup(x => x.CreateLesson(lesson)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreateLesson(lesson);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _courseServiceMock.Verify(x => x.CreateLesson(lesson), Times.Once);
        }

        [Fact]
        public async Task StartLesson_ShouldReturnOkResult_WhenLessonIsStarted()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var response = new ResponseResult();

            _courseServiceMock.Setup(x => x.StartLesson(lessonId)).ReturnsAsync(response);

            // Act
            var result = await _controller.StartLesson(lessonId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _courseServiceMock.Verify(x => x.StartLesson(lessonId), Times.Once);
        }

        [Fact]
        public async Task FinishLesson_ShouldReturnOkResult_WhenLessonIsFinished()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var response = new ResponseResult();

            _courseServiceMock.Setup(x => x.FinishLesson(lessonId)).ReturnsAsync(response);

            // Act
            var result = await _controller.FinishLesson(lessonId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _courseServiceMock.Verify(x => x.FinishLesson(lessonId), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldCallCourseService_ExactlyOnce()
        {
            // Arrange
            _courseServiceMock.Setup(x => x.GetAll()).ReturnsAsync(new List<CourseViewModel>());

            // Act
            await _controller.GetAll();

            // Assert
            _courseServiceMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldCallCourseService_WithCorrectId()
        {
            // Arrange
            var courseId = Guid.Parse("12345678-1234-1234-1234-123456789012");
            _courseServiceMock.Setup(x => x.GetById(courseId)).ReturnsAsync(new CourseViewModel());

            // Act
            await _controller.GetById(courseId);

            // Assert
            _courseServiceMock.Verify(x => x.GetById(courseId), Times.Once);
        }

        [Fact]
        public async Task CreateCourse_ShouldHandleErrorResponse()
        {
            // Arrange
            var course = new CourseViewModel { Name = "Test Course", Description = "Test", Price = 100 };
            var response = new ResponseResult();
            response.Errors.Messages.Add("Error creating course");

            _courseServiceMock.Setup(x => x.Create(course)).ReturnsAsync(response);

            // Act
            var result = await _controller.CreateCourse(course);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task UpdateCourse_ShouldHandleErrorResponse()
        {
            // Arrange
            var course = new CourseViewModel { Id = Guid.NewGuid(), Name = "Test Course", Description = "Test", Price = 100 };
            var response = new ResponseResult();
            response.Errors.Messages.Add("Error updating course");

            _courseServiceMock.Setup(x => x.Update(course)).ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateCourse(course);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task RemoveCourse_ShouldHandleErrorResponse()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var response = new ResponseResult();
            response.Errors.Messages.Add("Error removing course");

            _courseServiceMock.Setup(x => x.Remove(courseId)).ReturnsAsync(response);

            // Act
            var result = await _controller.RemoveCourse(courseId);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task MakePaymentCourse_ShouldHandleErrorResponse()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var payment = new PaymentViewModel { CardName = "Test", CardNumber = "123", CardExpirationDate = "12/25", CardCVV = "123" };
            var response = new ResponseResult();
            response.Errors.Messages.Add("Payment failed");

            _courseServiceMock.Setup(x => x.MakePayment(courseId, payment)).ReturnsAsync(response);

            // Act
            var result = await _controller.MakePaymentCourse(courseId, payment);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task GetLessonByCourse_ShouldReturnEmptyList_WhenNoLessonsExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _courseServiceMock.Setup(x => x.GetLessonByCourse(courseId)).ReturnsAsync(new List<LessonViewModel>());

            // Act
            var result = await _controller.GetLessonByCourse(courseId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var lessons = okResult.Value.Should().BeAssignableTo<List<LessonViewModel>>().Subject;
            lessons.Should().BeEmpty();
        }

        [Fact]
        public async Task GetProgressLesson_ShouldReturnEmptyList_WhenNoProgressExists()
        {
            // Arrange
            _courseServiceMock.Setup(x => x.GetProgressLesson()).ReturnsAsync(new List<LessonProgressViewModel>());

            // Act
            var result = await _controller.GetProgressLesson();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var progress = okResult.Value.Should().BeAssignableTo<List<LessonProgressViewModel>>().Subject;
            progress.Should().BeEmpty();
        }
    }
}
