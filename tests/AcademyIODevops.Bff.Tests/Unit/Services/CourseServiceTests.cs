using System.Net;
using AcademyIODevops.Bff.Extensions;
using AcademyIODevops.Bff.Models;
using AcademyIODevops.Bff.Services;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;

namespace AcademyIODevops.Bff.Tests.Unit.Services
{
    public class CourseServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly CourseService _courseService;
        private readonly AppServicesSettings _settings;

        public CourseServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);

            _settings = new AppServicesSettings
            {
                CourseUrl = "http://localhost:5001"
            };

            var options = Options.Create(_settings);
            _courseService = new CourseService(_httpClient, options);
        }

        [Fact]
        public async Task GetAll_ShouldReturnCourses_WhenHttpClientReturnsSuccess()
        {
            // Arrange
            var courses = new List<CourseViewModel>
            {
                new CourseViewModel { Id = Guid.NewGuid(), Name = "Docker Course" },
                new CourseViewModel { Id = Guid.NewGuid(), Name = "Kubernetes Course" }
            };

            var json = System.Text.Json.JsonSerializer.Serialize(courses);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains("/api/courses")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _courseService.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetById_ShouldReturnCourse_WhenCourseExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseViewModel
            {
                Id = courseId,
                Name = "Docker Course",
                Description = "Learn Docker",
                Price = 99.99
            };

            var json = System.Text.Json.JsonSerializer.Serialize(course);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Get &&
                        req.RequestUri.ToString().Contains($"/api/courses/{courseId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _courseService.GetById(courseId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(courseId);
            result.Name.Should().Be("Docker Course");
        }

        [Fact]
        public async Task Create_ShouldReturnOk_WhenCourseCreatedSuccessfully()
        {
            // Arrange
            var course = new CourseViewModel
            {
                Name = "New Course",
                Description = "Description",
                Price = 149.99
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString().Contains("api/courses/create")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _courseService.Create(course);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Update_ShouldReturnOk_WhenCourseUpdatedSuccessfully()
        {
            // Arrange
            var course = new CourseViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Updated Course",
                Description = "Updated Description",
                Price = 199.99
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Put &&
                        req.RequestUri.ToString().Contains("api/courses/update")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _courseService.Update(course);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Remove_ShouldReturnOk_WhenCourseRemovedSuccessfully()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Delete &&
                        req.RequestUri.ToString().Contains($"api/courses/remove/{courseId}")),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _courseService.Remove(courseId);

            // Assert
            result.Should().NotBeNull();
        }

        [Theory]
        [InlineData("Course 1", 50.0)]
        [InlineData("Course 2", 100.0)]
        [InlineData("Course 3", 150.0)]
        public async Task GetById_ShouldReturnCorrectCourse_WithDifferentData(string name, double price)
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseViewModel
            {
                Id = courseId,
                Name = name,
                Price = price
            };

            var json = System.Text.Json.JsonSerializer.Serialize(course);
            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };

            _httpMessageHandlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);

            // Act
            var result = await _courseService.GetById(courseId);

            // Assert
            result.Name.Should().Be(name);
            result.Price.Should().Be(price);
        }
    }
}
