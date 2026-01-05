using AcademyIODevops.Bff.Extensions;
using AcademyIODevops.Bff.Services;
using AcademyIODevops.WebAPI.Core.User;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using System.Net;

namespace AcademyIODevops.Bff.Tests.Unit.Services
{
    public class PaymentServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly Mock<IAspNetUser> _aspNetUserMock;
        private readonly Mock<IOptions<AppServicesSettings>> _settingsMock;
        private readonly PaymentService _paymentService;
        private readonly HttpClient _httpClient;

        public PaymentServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _aspNetUserMock = new Mock<IAspNetUser>();
            _settingsMock = new Mock<IOptions<AppServicesSettings>>();

            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _settingsMock
                .Setup(x => x.Value)
                .Returns(new AppServicesSettings
                {
                    PaymentUrl = "http://localhost:5000"
                });

            _paymentService = new PaymentService(_httpClient, _settingsMock.Object, _aspNetUserMock.Object);
        }

        [Fact]
        public async Task PaymentExists_ShouldReturnTrue_WhenPaymentExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            _aspNetUserMock.Setup(x => x.GetUserId()).Returns(studentId);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri!.ToString().Contains($"/api/payments/exists?courseId={courseId}&studentId={studentId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("true")
                });

            // Act
            var result = await _paymentService.PaymentExists(courseId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task PaymentExists_ShouldReturnFalse_WhenPaymentDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();
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
                    Content = new StringContent("false")
                });

            // Act
            var result = await _paymentService.PaymentExists(courseId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task PaymentExists_ShouldReturnFalse_WhenHttpResponseIsBadRequest()
        {
            // Arrange
            var courseId = Guid.NewGuid();
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
                    StatusCode = HttpStatusCode.BadRequest
                });

            // Act
            var result = await _paymentService.PaymentExists(courseId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task PaymentExists_ShouldCallCorrectEndpoint_WithCorrectParameters()
        {
            // Arrange
            var courseId = Guid.Parse("12345678-1234-1234-1234-123456789012");
            var studentId = Guid.Parse("87654321-4321-4321-4321-210987654321");

            _aspNetUserMock.Setup(x => x.GetUserId()).Returns(studentId);

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
                    Content = new StringContent("true")
                });

            // Act
            await _paymentService.PaymentExists(courseId);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest!.RequestUri!.ToString().Should().Contain($"courseId={courseId}");
            capturedRequest.RequestUri.ToString().Should().Contain($"studentId={studentId}");
        }

        [Fact]
        public async Task PaymentExists_ShouldUseStudentIdFromAspNetUser()
        {
            // Arrange
            var courseId = Guid.NewGuid();
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
                    Content = new StringContent("true")
                });

            // Act
            await _paymentService.PaymentExists(courseId);

            // Assert
            _aspNetUserMock.Verify(x => x.GetUserId(), Times.Once);
            capturedRequest!.RequestUri!.ToString().Should().Contain($"studentId={expectedStudentId}");
        }

        [Fact]
        public async Task PaymentExists_ShouldHandleMultipleCalls_ForDifferentCourses()
        {
            // Arrange
            var courseId1 = Guid.NewGuid();
            var courseId2 = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            _aspNetUserMock.Setup(x => x.GetUserId()).Returns(studentId);

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(() => new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("true")
                });

            // Act
            var result1 = await _paymentService.PaymentExists(courseId1);
            var result2 = await _paymentService.PaymentExists(courseId2);

            // Assert
            result1.Should().BeTrue();
            result2.Should().BeTrue();
            _httpMessageHandlerMock.Protected().Verify(
                "SendAsync",
                Times.Exactly(2),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task PaymentExists_ShouldReturnFalse_WhenResponseContentIsNull()
        {
            // Arrange
            var courseId = Guid.NewGuid();
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
                    StatusCode = HttpStatusCode.BadRequest,
                    Content = new StringContent(string.Empty)
                });

            // Act
            var result = await _paymentService.PaymentExists(courseId);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task PaymentExists_ShouldUseCorrectBaseAddress()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            _aspNetUserMock.Setup(x => x.GetUserId()).Returns(studentId);

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
                    Content = new StringContent("true")
                });

            // Act
            await _paymentService.PaymentExists(courseId);

            // Assert
            _httpClient.BaseAddress.Should().NotBeNull();
            _httpClient.BaseAddress!.ToString().Should().Be("http://localhost:5000/");
        }
    }
}
