using AcademyIODevops.Bff.Controllers;
using AcademyIODevops.Bff.Models;
using AcademyIODevops.Bff.Services;
using AcademyIODevops.Core.Communication;
using AcademyIODevops.Core.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AcademyIODevops.Bff.Tests.Unit.Controllers
{
    public class StudentsControllerTests
    {
        private readonly Mock<IStudentService> _studentServiceMock;
        private readonly Mock<ICourseService> _courseServiceMock;
        private readonly Mock<IPaymentService> _paymentServiceMock;
        private readonly StudentsController _controller;

        public StudentsControllerTests()
        {
            _studentServiceMock = new Mock<IStudentService>();
            _courseServiceMock = new Mock<ICourseService>();
            _paymentServiceMock = new Mock<IPaymentService>();

            _controller = new StudentsController(
                _studentServiceMock.Object,
                _courseServiceMock.Object,
                _paymentServiceMock.Object
            );
        }

        [Fact]
        public async Task RegisterToCourse_ShouldReturnOkResult_WhenRegistrationSucceeds()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseViewModel { Id = courseId, Name = "Docker Course", Description = "Learn Docker", Price = 99.99 };
            var response = new ResponseResult();

            _courseServiceMock.Setup(x => x.GetById(courseId)).ReturnsAsync(course);
            _paymentServiceMock.Setup(x => x.PaymentExists(courseId)).ReturnsAsync(true);
            _studentServiceMock.Setup(x => x.RegisterToCourse(courseId)).ReturnsAsync(response);

            // Act
            var result = await _controller.RegisterToCourse(courseId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _courseServiceMock.Verify(x => x.GetById(courseId), Times.Once);
            _paymentServiceMock.Verify(x => x.PaymentExists(courseId), Times.Once);
            _studentServiceMock.Verify(x => x.RegisterToCourse(courseId), Times.Once);
        }

        [Fact]
        public async Task RegisterToCourse_ShouldReturnNotFound_WhenCourseDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            _courseServiceMock.Setup(x => x.GetById(courseId)).ReturnsAsync((CourseViewModel?)null);

            // Act
            var result = await _controller.RegisterToCourse(courseId);

            // Assert
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Subject;
            notFoundResult.Value.Should().Be("Curso não encontrado.");

            _courseServiceMock.Verify(x => x.GetById(courseId), Times.Once);
            _paymentServiceMock.Verify(x => x.PaymentExists(It.IsAny<Guid>()), Times.Never);
            _studentServiceMock.Verify(x => x.RegisterToCourse(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task RegisterToCourse_ShouldReturnUnprocessableEntity_WhenPaymentDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseViewModel { Id = courseId, Name = "Docker Course", Description = "Learn Docker", Price = 99.99 };

            _courseServiceMock.Setup(x => x.GetById(courseId)).ReturnsAsync(course);
            _paymentServiceMock.Setup(x => x.PaymentExists(courseId)).ReturnsAsync(false);

            // Act
            var result = await _controller.RegisterToCourse(courseId);

            // Assert
            var unprocessableResult = result.Should().BeOfType<UnprocessableEntityObjectResult>().Subject;
            unprocessableResult.Value.Should().Be("Você não possui acesso a esse curso.");

            _courseServiceMock.Verify(x => x.GetById(courseId), Times.Once);
            _paymentServiceMock.Verify(x => x.PaymentExists(courseId), Times.Once);
            _studentServiceMock.Verify(x => x.RegisterToCourse(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetRegistration_ShouldReturnOkResult_WithRegistrations()
        {
            // Arrange
            var registrations = new List<RegistrationViewModel>
            {
                new() { Id = Guid.NewGuid(), StudentId = Guid.NewGuid(), CourseId = Guid.NewGuid(), Status = EProgressLesson.NotStarted },
                new() { Id = Guid.NewGuid(), StudentId = Guid.NewGuid(), CourseId = Guid.NewGuid(), Status = EProgressLesson.InProgress }
            };

            _studentServiceMock.Setup(x => x.GetRegistration()).ReturnsAsync(registrations);

            // Act
            var result = await _controller.GetRegistration();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedRegistrations = okResult.Value.Should().BeAssignableTo<List<RegistrationViewModel>>().Subject;
            returnedRegistrations.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkResult_WithAllRegistrations()
        {
            // Arrange
            var registrations = new List<RegistrationViewModel>
            {
                new() { Id = Guid.NewGuid(), StudentId = Guid.NewGuid(), CourseId = Guid.NewGuid(), Status = EProgressLesson.NotStarted },
                new() { Id = Guid.NewGuid(), StudentId = Guid.NewGuid(), CourseId = Guid.NewGuid(), Status = EProgressLesson.Completed },
                new() { Id = Guid.NewGuid(), StudentId = Guid.NewGuid(), CourseId = Guid.NewGuid(), Status = EProgressLesson.InProgress }
            };

            _studentServiceMock.Setup(x => x.GetAllRegistrations()).ReturnsAsync(registrations);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedRegistrations = okResult.Value.Should().BeAssignableTo<List<RegistrationViewModel>>().Subject;
            returnedRegistrations.Should().HaveCount(3);
        }

        [Fact]
        public async Task RegisterToCourse_ShouldCheckPaymentBeforeRegistering()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseViewModel { Id = courseId, Name = "Test Course", Description = "Test", Price = 100 };

            _courseServiceMock.Setup(x => x.GetById(courseId)).ReturnsAsync(course);
            _paymentServiceMock.Setup(x => x.PaymentExists(courseId)).ReturnsAsync(false);

            // Act
            var result = await _controller.RegisterToCourse(courseId);

            // Assert
            result.Should().BeOfType<UnprocessableEntityObjectResult>();
            _studentServiceMock.Verify(x => x.RegisterToCourse(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task RegisterToCourse_ShouldHandleErrorResponseFromStudentService()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseViewModel { Id = courseId, Name = "Test Course", Description = "Test", Price = 100 };
            var errorResponse = new ResponseResult();
            errorResponse.Errors.Messages.Add("Erro ao registrar");

            _courseServiceMock.Setup(x => x.GetById(courseId)).ReturnsAsync(course);
            _paymentServiceMock.Setup(x => x.PaymentExists(courseId)).ReturnsAsync(true);
            _studentServiceMock.Setup(x => x.RegisterToCourse(courseId)).ReturnsAsync(errorResponse);

            // Act
            var result = await _controller.RegisterToCourse(courseId);

            // Assert
            var badRequestResult = result.Should().BeOfType<BadRequestObjectResult>().Subject;
            badRequestResult.Value.Should().NotBeNull();
        }

        [Fact]
        public async Task GetRegistration_ShouldReturnEmptyList_WhenStudentHasNoRegistrations()
        {
            // Arrange
            _studentServiceMock.Setup(x => x.GetRegistration()).ReturnsAsync(new List<RegistrationViewModel>());

            // Act
            var result = await _controller.GetRegistration();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var registrations = okResult.Value.Should().BeAssignableTo<List<RegistrationViewModel>>().Subject;
            registrations.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoRegistrationsExist()
        {
            // Arrange
            _studentServiceMock.Setup(x => x.GetAllRegistrations()).ReturnsAsync(new List<RegistrationViewModel>());

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var registrations = okResult.Value.Should().BeAssignableTo<List<RegistrationViewModel>>().Subject;
            registrations.Should().BeEmpty();
        }

        [Fact]
        public async Task RegisterToCourse_ShouldCallServicesInCorrectOrder()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseViewModel { Id = courseId, Name = "Test Course", Description = "Test", Price = 100 };
            var response = new ResponseResult();

            var callOrder = new List<string>();

            _courseServiceMock
                .Setup(x => x.GetById(courseId))
                .Callback(() => callOrder.Add("GetById"))
                .ReturnsAsync(course);

            _paymentServiceMock
                .Setup(x => x.PaymentExists(courseId))
                .Callback(() => callOrder.Add("PaymentExists"))
                .ReturnsAsync(true);

            _studentServiceMock
                .Setup(x => x.RegisterToCourse(courseId))
                .Callback(() => callOrder.Add("RegisterToCourse"))
                .ReturnsAsync(response);

            // Act
            await _controller.RegisterToCourse(courseId);

            // Assert
            callOrder.Should().Equal("GetById", "PaymentExists", "RegisterToCourse");
        }

        [Fact]
        public async Task RegisterToCourse_ShouldNotCheckPayment_WhenCourseNotFound()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _courseServiceMock.Setup(x => x.GetById(courseId)).ReturnsAsync((CourseViewModel?)null);

            // Act
            await _controller.RegisterToCourse(courseId);

            // Assert
            _paymentServiceMock.Verify(x => x.PaymentExists(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetRegistration_ShouldCallStudentService_ExactlyOnce()
        {
            // Arrange
            _studentServiceMock.Setup(x => x.GetRegistration()).ReturnsAsync(new List<RegistrationViewModel>());

            // Act
            await _controller.GetRegistration();

            // Assert
            _studentServiceMock.Verify(x => x.GetRegistration(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldCallStudentService_ExactlyOnce()
        {
            // Arrange
            _studentServiceMock.Setup(x => x.GetAllRegistrations()).ReturnsAsync(new List<RegistrationViewModel>());

            // Act
            await _controller.GetAll();

            // Assert
            _studentServiceMock.Verify(x => x.GetAllRegistrations(), Times.Once);
        }

        [Fact]
        public async Task RegisterToCourse_ShouldValidateCourseExists_BeforeAnyOtherAction()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _courseServiceMock.Setup(x => x.GetById(courseId)).ReturnsAsync((CourseViewModel?)null);

            // Act
            var result = await _controller.RegisterToCourse(courseId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
            _paymentServiceMock.VerifyNoOtherCalls();
            _studentServiceMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task RegisterToCourse_ShouldValidatePayment_BeforeRegistering()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseViewModel { Id = courseId, Name = "Test", Description = "Test", Price = 50 };

            _courseServiceMock.Setup(x => x.GetById(courseId)).ReturnsAsync(course);
            _paymentServiceMock.Setup(x => x.PaymentExists(courseId)).ReturnsAsync(false);

            // Act
            var result = await _controller.RegisterToCourse(courseId);

            // Assert
            result.Should().BeOfType<UnprocessableEntityObjectResult>();
            _studentServiceMock.Verify(x => x.RegisterToCourse(It.IsAny<Guid>()), Times.Never);
        }
    }
}
