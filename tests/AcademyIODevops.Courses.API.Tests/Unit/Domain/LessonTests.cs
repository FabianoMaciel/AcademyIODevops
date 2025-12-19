using AcademyIODevops.Courses.API.Models;
using FluentAssertions;

namespace AcademyIODevops.Courses.API.Tests.Unit.Domain
{
    public class LessonTests
    {
        [Fact]
        public void Lesson_ShouldCreateValidLesson_WhenPropertiesAreValid()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var name = "Introduction to Kubernetes";
            var subject = "Container Orchestration";
            var totalHours = 3.5;

            // Act
            var lesson = new Lesson(name, subject, totalHours, courseId);

            // Assert
            lesson.Should().NotBeNull();
            lesson.Id.Should().NotBeEmpty();
            lesson.Name.Should().Be(name);
            lesson.Subject.Should().Be(subject);
            lesson.TotalHours.Should().Be(totalHours);
            lesson.CourseId.Should().Be(courseId);
        }

        [Fact]
        public void Lesson_ShouldGenerateUniqueIds_WhenMultipleLessonsCreated()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Act
            var lesson1 = new Lesson("Lesson 1", "Subject 1", 1.0, courseId);
            var lesson2 = new Lesson("Lesson 2", "Subject 2", 2.0, courseId);

            // Assert
            lesson1.Id.Should().NotBe(lesson2.Id);
            lesson1.Id.Should().NotBeEmpty();
            lesson2.Id.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(0.5)]
        [InlineData(1.0)]
        [InlineData(2.5)]
        [InlineData(8.0)]
        [InlineData(40.0)]
        public void Lesson_ShouldAcceptValidTotalHours(double validHours)
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Act
            var lesson = new Lesson("Test Lesson", "Test Subject", validHours, courseId);

            // Assert
            lesson.TotalHours.Should().Be(validHours);
        }

        [Fact]
        public void Lesson_ShouldHaveSameCourseId_AsProvidedInConstructor()
        {
            // Arrange
            var expectedCourseId = Guid.NewGuid();

            // Act
            var lesson = new Lesson("Docker Deep Dive", "Containers", 5.0, expectedCourseId);

            // Assert
            lesson.CourseId.Should().Be(expectedCourseId);
        }

        [Fact]
        public void Lesson_ShouldAllowUpdatingProperties()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var lesson = new Lesson("Original Name", "Original Subject", 2.0, courseId);

            // Act
            lesson.Name = "Updated Name";
            lesson.Subject = "Updated Subject";
            lesson.TotalHours = 3.5;

            // Assert
            lesson.Name.Should().Be("Updated Name");
            lesson.Subject.Should().Be("Updated Subject");
            lesson.TotalHours.Should().Be(3.5);
            lesson.CourseId.Should().Be(courseId); // CourseId should remain the same
        }

        [Fact]
        public void Lesson_ShouldImplementIAggregateRoot()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Act
            var lesson = new Lesson("Test", "Test", 1.0, courseId);

            // Assert
            lesson.Should().BeAssignableTo<Core.DomainObjects.IAggregateRoot>();
        }

        [Fact]
        public void Lesson_ShouldHaveCreatedDateProperty()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Act
            var lesson = new Lesson("CI/CD Pipeline", "DevOps", 4.0, courseId);

            // Assert
            // CreatedDate não é automaticamente inicializado no construtor da entidade
            // Geralmente é setado pelo EF Core ao salvar no banco
            lesson.CreatedDate.Should().Be(default(DateTime));
        }

        [Fact]
        public void Lesson_ShouldAllowChangingCourseId()
        {
            // Arrange
            var originalCourseId = Guid.NewGuid();
            var newCourseId = Guid.NewGuid();
            var lesson = new Lesson("Test", "Test", 1.0, originalCourseId);

            // Act
            lesson.CourseId = newCourseId;

            // Assert
            lesson.CourseId.Should().Be(newCourseId);
            lesson.CourseId.Should().NotBe(originalCourseId);
        }

        [Theory]
        [InlineData("Docker Basics", "Learn Docker fundamentals", 3.5)]
        [InlineData("Kubernetes Advanced", "Advanced K8s concepts", 8.0)]
        [InlineData("CI/CD with Jenkins", "Continuous Integration", 4.5)]
        public void Lesson_ShouldCreateDifferentLessons_WithDifferentData(
            string name, string subject, double hours)
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Act
            var lesson = new Lesson(name, subject, hours, courseId);

            // Assert
            lesson.Name.Should().Be(name);
            lesson.Subject.Should().Be(subject);
            lesson.TotalHours.Should().Be(hours);
            lesson.CourseId.Should().Be(courseId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-5.5)]
        public void Lesson_ShouldAllowNegativeOrZeroHours_ButValidationShouldCatchIt(double invalidHours)
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Act
            var lesson = new Lesson("Test", "Test", invalidHours, courseId);

            // Assert
            // A entidade permite, mas o Command Validator deve rejeitar
            lesson.TotalHours.Should().Be(invalidHours);
            (lesson.TotalHours > 0).Should().BeFalse("Total hours should be validated at command level");
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData(null)]
        public void Lesson_ShouldAllowEmptyName_ButValidationShouldCatchIt(string invalidName)
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Act
            var lesson = new Lesson(invalidName, "Valid Subject", 2.0, courseId);

            // Assert
            // A entidade permite, mas o Command Validator deve rejeitar
            lesson.Name.Should().Be(invalidName);
        }
    }
}
