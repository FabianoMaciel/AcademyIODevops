using AcademyIODevops.Students.API.Application.Commands;
using AcademyIODevops.Students.API.Application.Queries;
using AcademyIODevops.Students.API.Application.Queries.ViewModels;
using AcademyIODevops.Students.API.Controllers;
using AcademyIODevops.Students.API.Tests.Builders;
using AcademyIODevops.WebAPI.Core.User;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace AcademyIODevops.Students.API.Tests.Unit.Controllers
{
    public class StudentControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IAspNetUser> _aspNetUserMock;
        private readonly Mock<IRegistrationQuery> _registrationQueryMock;
        private readonly StudentController _controller;

        public StudentControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _aspNetUserMock = new Mock<IAspNetUser>();
            _registrationQueryMock = new Mock<IRegistrationQuery>();

            _controller = new StudentController(
                _mediatorMock.Object,
                _aspNetUserMock.Object,
                _registrationQueryMock.Object
            );
        }

        #region RegisterToCourse Tests

        [Fact]
        public async Task RegisterToCourse_ShouldCallMediator_WithCorrectCommand()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);

            // Act
            await _controller.RegisterToCourse(courseId);

            // Assert
            _mediatorMock.Verify(
                x => x.Send(
                    It.Is<AddRegistrationCommand>(cmd =>
                        cmd.StudentId == userId && cmd.CourseId == courseId),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task RegisterToCourse_ShouldGetUserId_FromIAspNetUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);

            // Act
            await _controller.RegisterToCourse(courseId);

            // Assert
            _aspNetUserMock.Verify(x => x.GetUserId(), Times.Once);
        }

        [Fact]
        public async Task RegisterToCourse_ShouldReturnActionResult()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);

            // Act
            var result = await _controller.RegisterToCourse(courseId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<IActionResult>();
        }

        [Fact]
        public async Task RegisterToCourse_ShouldUseGuidCourseId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.Parse("12345678-1234-1234-1234-123456789012");

            _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);

            // Act
            await _controller.RegisterToCourse(courseId);

            // Assert
            _mediatorMock.Verify(
                x => x.Send(
                    It.Is<AddRegistrationCommand>(cmd => cmd.CourseId == courseId),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task RegisterToCourse_ShouldHandleEmptyCourseId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.Empty;

            _aspNetUserMock.Setup(x => x.GetUserId()).Returns(userId);

            // Act
            var result = await _controller.RegisterToCourse(courseId);

            // Assert
            result.Should().NotBeNull();
            _mediatorMock.Verify(
                x => x.Send(It.IsAny<AddRegistrationCommand>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        #endregion

        #region GetRegistration Tests

        [Fact]
        public void GetRegistration_ShouldReturnOkResult_WithRegistrations()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var registrations = new List<RegistrationViewModel>
            {
                CreateRegistrationViewModel(studentId),
                CreateRegistrationViewModel(studentId),
                CreateRegistrationViewModel(studentId)
            };

            _registrationQueryMock.Setup(x => x.GetByStudent(studentId))
                .Returns(registrations);

            // Act
            var result = _controller.GetRegistration(studentId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(registrations);
        }

        [Fact]
        public void GetRegistration_ShouldReturnOkResult_WithEmptyList_WhenNoRegistrations()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var registrations = new List<RegistrationViewModel>();

            _registrationQueryMock.Setup(x => x.GetByStudent(studentId))
                .Returns(registrations);

            // Act
            var result = _controller.GetRegistration(studentId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            var returnedList = okResult.Value as List<RegistrationViewModel>;
            returnedList.Should().NotBeNull();
            returnedList.Should().BeEmpty();
        }

        [Fact]
        public void GetRegistration_ShouldCallQuery_WithCorrectStudentId()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var registrations = new List<RegistrationViewModel>();

            _registrationQueryMock.Setup(x => x.GetByStudent(studentId))
                .Returns(registrations);

            // Act
            _controller.GetRegistration(studentId);

            // Assert
            _registrationQueryMock.Verify(x => x.GetByStudent(studentId), Times.Once);
        }

        [Fact]
        public void GetRegistration_ShouldReturnCorrectViewModelStructure()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var registration = CreateRegistrationViewModel(studentId);
            var registrations = new List<RegistrationViewModel> { registration };

            _registrationQueryMock.Setup(x => x.GetByStudent(studentId))
                .Returns(registrations);

            // Act
            var result = _controller.GetRegistration(studentId);

            // Assert
            var okResult = result as OkObjectResult;
            var returnedList = okResult.Value as List<RegistrationViewModel>;

            returnedList.Should().HaveCount(1);
            returnedList.First().Should().BeEquivalentTo(registration);
        }

        [Fact]
        public void GetRegistration_ShouldHandleMultipleRegistrations()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var registrations = new List<RegistrationViewModel>
            {
                CreateRegistrationViewModel(studentId),
                CreateRegistrationViewModel(studentId),
                CreateRegistrationViewModel(studentId),
                CreateRegistrationViewModel(studentId),
                CreateRegistrationViewModel(studentId)
            };

            _registrationQueryMock.Setup(x => x.GetByStudent(studentId))
                .Returns(registrations);

            // Act
            var result = _controller.GetRegistration(studentId);

            // Assert
            var okResult = result as OkObjectResult;
            var returnedList = okResult.Value as List<RegistrationViewModel>;

            returnedList.Should().HaveCount(5);
        }

        #endregion

        #region GetAllRegistrations Tests

        [Fact]
        public void GetAllRegistrations_ShouldReturnOkResult_WithAllRegistrations()
        {
            // Arrange
            var student1Id = Guid.NewGuid();
            var student2Id = Guid.NewGuid();
            var student3Id = Guid.NewGuid();

            var registrations = new List<RegistrationViewModel>
            {
                CreateRegistrationViewModel(student1Id),
                CreateRegistrationViewModel(student1Id),
                CreateRegistrationViewModel(student2Id),
                CreateRegistrationViewModel(student3Id),
                CreateRegistrationViewModel(student3Id)
            };

            _registrationQueryMock.Setup(x => x.GetAllRegistrations())
                .Returns(registrations);

            // Act
            var result = _controller.GetAllRegistrations();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeEquivalentTo(registrations);
        }

        [Fact]
        public void GetAllRegistrations_ShouldReturnOkResult_WithEmptyList_WhenNoRegistrations()
        {
            // Arrange
            var registrations = new List<RegistrationViewModel>();

            _registrationQueryMock.Setup(x => x.GetAllRegistrations())
                .Returns(registrations);

            // Act
            var result = _controller.GetAllRegistrations();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            var returnedList = okResult.Value as List<RegistrationViewModel>;
            returnedList.Should().NotBeNull();
            returnedList.Should().BeEmpty();
        }

        [Fact]
        public void GetAllRegistrations_ShouldCallQuery_GetAllRegistrations()
        {
            // Arrange
            var registrations = new List<RegistrationViewModel>();

            _registrationQueryMock.Setup(x => x.GetAllRegistrations())
                .Returns(registrations);

            // Act
            _controller.GetAllRegistrations();

            // Assert
            _registrationQueryMock.Verify(x => x.GetAllRegistrations(), Times.Once);
        }

        [Fact]
        public void GetAllRegistrations_ShouldReturnRegistrationsFromMultipleStudents()
        {
            // Arrange
            var student1Id = Guid.NewGuid();
            var student2Id = Guid.NewGuid();
            var student3Id = Guid.NewGuid();

            var registrations = new List<RegistrationViewModel>
            {
                CreateRegistrationViewModel(student1Id),
                CreateRegistrationViewModel(student2Id),
                CreateRegistrationViewModel(student3Id)
            };

            _registrationQueryMock.Setup(x => x.GetAllRegistrations())
                .Returns(registrations);

            // Act
            var result = _controller.GetAllRegistrations();

            // Assert
            var okResult = result as OkObjectResult;
            var returnedList = okResult.Value as List<RegistrationViewModel>;

            returnedList.Should().HaveCount(3);
            returnedList.Select(r => r.StudentId).Distinct().Should().HaveCount(3);
        }

        [Fact]
        public void GetAllRegistrations_ShouldReturnCorrectResponseFormat()
        {
            // Arrange
            var registration = CreateRegistrationViewModel(Guid.NewGuid());
            var registrations = new List<RegistrationViewModel> { registration };

            _registrationQueryMock.Setup(x => x.GetAllRegistrations())
                .Returns(registrations);

            // Act
            var result = _controller.GetAllRegistrations();

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().BeOfType<List<RegistrationViewModel>>();
        }

        #endregion

        #region Helper Methods

        private RegistrationViewModel CreateRegistrationViewModel(Guid studentId)
        {
            return new RegistrationViewModel
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                CourseId = Guid.NewGuid(),
                RegistrationTime = DateTime.UtcNow,
                Status = Core.Enums.EProgressLesson.NotStarted
            };
        }

        #endregion
    }
}
