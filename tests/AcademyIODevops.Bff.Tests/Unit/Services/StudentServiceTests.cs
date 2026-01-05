using AcademyIODevops.Bff.Extensions;
using AcademyIODevops.Bff.Models;
using AcademyIODevops.Bff.Services;
using AcademyIODevops.Core.Communication;
using AcademyIODevops.Core.Enums;
using AcademyIODevops.WebAPI.Core.User;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace AcademyIODevops.Bff.Tests.Unit.Services
{
    public class StudentServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<IAspNetUser> _aspNetUserMock;
        private readonly Mock<IOptions<AppServicesSettings>> _settingsMock;
        private readonly StudentService _studentService;
        private readonly HttpClient _httpClient;

        public StudentServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _aspNetUserMock = new Mock<IAspNetUser>();
            _settingsMock = new Mock<IOptions<AppServicesSettings>>();

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _settingsMock
                .Setup(x => x.Value)
                .Returns(new AppServicesSettings
                {
                    StudentUrl = "http://localhost:5001"
                });

            _studentService = new StudentService(_httpClient, _settingsMock.Object, _aspNetUserMock.Object);
        }

        [Fact]
        public async Task GetRegistration_ShouldReturnRegistrations_ForSpecificStudent()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var registrations = new List<RegistrationViewModel>
            {
                new() { Id = Guid.NewGuid(), StudentId = studentId, CourseId = Guid.NewGuid(), RegistrationTime = DateTime.Now, Status = EProgressLesson.NotStarted },
                new() { Id = Guid.NewGuid(), StudentId = studentId, CourseId = Guid.NewGuid(), RegistrationTime = DateTime.Now, Status = EProgressLesson.InProgress }
            };

            _aspNetUserMock.Setup(x => x.GetUserId()).Returns(studentId);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString().Contains($"/api/student/get-registration/{studentId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(registrations))
                });

            // Act
            var result = await _studentService.GetRegistration();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(r => r.StudentId.Should().Be(studentId));
        }

        [Fact]
        public async Task GetRegistration_ShouldReturnEmptyList_WhenStudentHasNoRegistrations()
        {
            // Arrange
            var studentId = Guid.NewGuid();

            _aspNetUserMock.Setup(x => x.GetUserId()).Returns(studentId);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new List<RegistrationViewModel>()))
                });

            // Act
            var result = await _studentService.GetRegistration();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAllRegistrations_ShouldReturnAllRegistrations()
        {
            // Arrange
            var registrations = new List<RegistrationViewModel>
            {
                new() { Id = Guid.NewGuid(), StudentId = Guid.NewGuid(), CourseId = Guid.NewGuid(), RegistrationTime = DateTime.Now, Status = EProgressLesson.NotStarted },
                new() { Id = Guid.NewGuid(), StudentId = Guid.NewGuid(), CourseId = Guid.NewGuid(), RegistrationTime = DateTime.Now, Status = EProgressLesson.Completed },
                new() { Id = Guid.NewGuid(), StudentId = Guid.NewGuid(), CourseId = Guid.NewGuid(), RegistrationTime = DateTime.Now, Status = EProgressLesson.InProgress }
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString().Contains("/api/student/get-all-registrations")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(registrations))
                });

            // Act
            var result = await _studentService.GetAllRegistrations();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task RegisterToCourse_ShouldReturnSuccessResponse_WhenRegistrationSucceeds()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri!.ToString().Contains($"/api/student/register-to-course/{courseId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new ResponseResult()))
                });

            // Act
            var result = await _studentService.RegisterToCourse(courseId);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task RegisterToCourse_ShouldPostToCorrectEndpoint()
        {
            // Arrange
            var courseId = Guid.Parse("12345678-1234-1234-1234-123456789012");
            HttpRequestMessage? capturedRequest = null;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, token) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new ResponseResult()))
                });

            // Act
            await _studentService.RegisterToCourse(courseId);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.Method.Should().Be(HttpMethod.Post);
            capturedRequest.RequestUri!.ToString().Should().Contain($"/api/student/register-to-course/{courseId}");
        }

        [Fact]
        public async Task GetRegistration_ShouldUseStudentIdFromAspNetUser()
        {
            // Arrange
            var expectedStudentId = Guid.NewGuid();
            _aspNetUserMock.Setup(x => x.GetUserId()).Returns(expectedStudentId);

            HttpRequestMessage? capturedRequest = null;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, token) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new List<RegistrationViewModel>()))
                });

            // Act
            await _studentService.GetRegistration();

            // Assert
            _aspNetUserMock.Verify(x => x.GetUserId(), Times.Once);
            capturedRequest!.RequestUri!.ToString().Should().Contain($"/api/student/get-registration/{expectedStudentId}");
        }

        [Fact]
        public async Task GetAllRegistrations_ShouldReturnEmptyList_WhenNoRegistrationsExist()
        {
            // Arrange
            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new List<RegistrationViewModel>()))
                });

            // Act
            var result = await _studentService.GetAllRegistrations();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task RegisterToCourse_ShouldReturnErrorResponse_WhenRegistrationFails()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var errorResponse = new ResponseResult();
            errorResponse.Errors.Messages.Add("Erro ao registrar no curso");

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(JsonSerializer.Serialize(errorResponse))
                });

            // Act
            var result = await _studentService.RegisterToCourse(courseId);

            // Assert
            result.Should().NotBeNull();
            result.Errors.Messages.Should().Contain("Erro ao registrar no curso");
        }

        [Fact]
        public async Task GetRegistration_ShouldReturnRegistrationsWithDifferentStatuses()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var registrations = new List<RegistrationViewModel>
            {
                new() { Id = Guid.NewGuid(), StudentId = studentId, CourseId = Guid.NewGuid(), Status = EProgressLesson.NotStarted },
                new() { Id = Guid.NewGuid(), StudentId = studentId, CourseId = Guid.NewGuid(), Status = EProgressLesson.InProgress },
                new() { Id = Guid.NewGuid(), StudentId = studentId, CourseId = Guid.NewGuid(), Status = EProgressLesson.Completed }
            };

            _aspNetUserMock.Setup(x => x.GetUserId()).Returns(studentId);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(registrations))
                });

            // Act
            var result = await _studentService.GetRegistration();

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(r => r.Status == EProgressLesson.NotStarted);
            result.Should().Contain(r => r.Status == EProgressLesson.InProgress);
            result.Should().Contain(r => r.Status == EProgressLesson.Completed);
        }

        [Fact]
        public async Task GetAllRegistrations_ShouldCallCorrectEndpoint()
        {
            // Arrange
            HttpRequestMessage? capturedRequest = null;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, token) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new List<RegistrationViewModel>()))
                });

            // Act
            await _studentService.GetAllRegistrations();

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.Method.Should().Be(HttpMethod.Get);
            capturedRequest.RequestUri!.ToString().Should().Contain("/api/student/get-all-registrations");
        }

        [Fact]
        public async Task RegisterToCourse_ShouldSendPostRequestWithNullContent()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            HttpRequestMessage? capturedRequest = null;

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Callback<HttpRequestMessage, CancellationToken>((req, token) => capturedRequest = req)
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonSerializer.Serialize(new ResponseResult()))
                });

            // Act
            await _studentService.RegisterToCourse(courseId);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.Content.Should().BeNull();
        }
    }
}
