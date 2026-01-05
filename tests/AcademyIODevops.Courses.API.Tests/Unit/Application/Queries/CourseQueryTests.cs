using AcademyIODevops.Courses.API.Application.Queries;
using AcademyIODevops.Courses.API.Models;
using AcademyIODevops.Courses.API.Tests.Builders;
using FluentAssertions;
using Moq;

namespace AcademyIODevops.Courses.API.Tests.Unit.Application.Queries
{
    public class CourseQueryTests
    {
        private readonly Mock<ICourseRepository> _courseRepositoryMock;
        private readonly CourseQuery _query;

        public CourseQueryTests()
        {
            _courseRepositoryMock = new Mock<ICourseRepository>();
            _query = new CourseQuery(_courseRepositoryMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllCourses_WhenCoursesExist()
        {
            // Arrange
            var courses = new List<Course>
            {
                new CourseBuilder()
                    .WithName("Docker Course")
                    .WithDescription("Learn Docker")
                    .WithPrice(99.99)
                    .Build(),
                new CourseBuilder()
                    .WithName("Kubernetes Course")
                    .WithDescription("Learn K8s")
                    .WithPrice(149.99)
                    .Build(),
                new CourseBuilder()
                    .WithName("CI/CD Course")
                    .WithDescription("Learn DevOps")
                    .WithPrice(79.99)
                    .Build()
            };

            _courseRepositoryMock
                .Setup(x => x.GetAll())
                .ReturnsAsync(courses);

            // Act
            var result = await _query.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);

            var resultList = result.ToList();
            resultList[0].Name.Should().Be("Docker Course");
            resultList[0].Price.Should().Be(99.99);
            resultList[1].Name.Should().Be("Kubernetes Course");
            resultList[1].Price.Should().Be(149.99);
            resultList[2].Name.Should().Be("CI/CD Course");
            resultList[2].Price.Should().Be(79.99);

            _courseRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoCoursesExist()
        {
            // Arrange
            _courseRepositoryMock
                .Setup(x => x.GetAll())
                .ReturnsAsync(new List<Course>());

            // Act
            var result = await _query.GetAll();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();

            _courseRepositoryMock.Verify(x => x.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnCourse_WhenCourseExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseBuilder()
                .WithId(courseId)
                .WithName("Docker Course")
                .WithDescription("Learn Docker containers")
                .WithPrice(99.99)
                .Build();

            _courseRepositoryMock
                .Setup(x => x.CourseExists(courseId))
                .Returns(true);

            _courseRepositoryMock
                .Setup(x => x.GetById(courseId))
                .ReturnsAsync(course);

            // Act
            var result = await _query.GetById(courseId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(courseId);
            result.Name.Should().Be("Docker Course");
            result.Description.Should().Be("Learn Docker containers");
            result.Price.Should().Be(99.99);

            _courseRepositoryMock.Verify(x => x.CourseExists(courseId), Times.Once);
            _courseRepositoryMock.Verify(x => x.GetById(courseId), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenCourseDoesNotExist()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            _courseRepositoryMock
                .Setup(x => x.CourseExists(courseId))
                .Returns(false);

            // Act
            var result = await _query.GetById(courseId);

            // Assert
            result.Should().BeNull();

            _courseRepositoryMock.Verify(x => x.CourseExists(courseId), Times.Once);
            _courseRepositoryMock.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
        }

        [Theory]
        [InlineData("DevOps Fundamentals", "Complete DevOps Guide", 199.99)]
        [InlineData("Docker Basics", "Introduction to containers", 49.99)]
        [InlineData("Advanced Kubernetes", "K8s production deployment", 299.99)]
        public async Task GetById_ShouldReturnCorrectCourse_WithDifferentData(
            string name, string description, double price)
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseBuilder()
                .WithId(courseId)
                .WithName(name)
                .WithDescription(description)
                .WithPrice(price)
                .Build();

            _courseRepositoryMock
                .Setup(x => x.CourseExists(courseId))
                .Returns(true);

            _courseRepositoryMock
                .Setup(x => x.GetById(courseId))
                .ReturnsAsync(course);

            // Act
            var result = await _query.GetById(courseId);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(name);
            result.Description.Should().Be(description);
            result.Price.Should().Be(price);
        }
    }
}
