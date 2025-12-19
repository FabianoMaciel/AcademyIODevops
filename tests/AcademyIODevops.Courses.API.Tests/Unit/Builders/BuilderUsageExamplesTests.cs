using AcademyIODevops.Courses.API.Tests.Builders;
using FluentAssertions;

namespace AcademyIODevops.Courses.API.Tests.Unit.Builders
{
    /// <summary>
    /// Exemplos de como usar os Builders em testes.
    /// Esta classe demonstra os padrões e práticas recomendadas.
    /// </summary>
    public class BuilderUsageExamplesTests
    {
        [Fact]
        public void Example_SimpleUsage_CreateDefaultCourse()
        {
            // Arrange & Act
            var course = new CourseBuilder().Build();

            // Assert
            course.Should().NotBeNull();
            course.Name.Should().Be("Default Course Name");
            course.Price.Should().Be(99.99);
        }

        [Fact]
        public void Example_FluentInterface_CustomizeCourse()
        {
            // Arrange & Act
            var course = new CourseBuilder()
                .WithName("Docker Mastery")
                .WithDescription("Learn Docker from scratch")
                .WithPrice(149.99)
                .Build();

            // Assert
            course.Name.Should().Be("Docker Mastery");
            course.Description.Should().Be("Learn Docker from scratch");
            course.Price.Should().Be(149.99);
        }

        [Fact]
        public void Example_PreConfiguredCourse_DevOps()
        {
            // Arrange & Act
            var course = new CourseBuilder()
                .AsDevOpsCourse()
                .Build();

            // Assert
            course.Name.Should().Be("DevOps Fundamentals");
            course.Price.Should().Be(149.99);
        }

        [Fact]
        public void Example_WithSpecificId_UsefulForRelationships()
        {
            // Arrange
            var specificId = Guid.NewGuid();

            // Act
            var course = new CourseBuilder()
                .WithId(specificId)
                .WithName("Test Course")
                .Build();

            // Assert
            course.Id.Should().Be(specificId);
        }

        [Fact]
        public void Example_InvalidData_ForValidationTests()
        {
            // Arrange & Act
            var course = new CourseBuilder()
                .WithEmptyName()
                .WithInvalidPrice()
                .Build();

            // Assert
            course.Name.Should().BeEmpty();
            course.Price.Should().BeNegative();
        }

        [Fact]
        public void Example_BuildMany_CreateMultipleCourses()
        {
            // Arrange & Act
            var courses = CourseBuilder.BuildMany(5);

            // Assert
            courses.Should().HaveCount(5);
            courses.Select(c => c.Id).Should().OnlyHaveUniqueItems();
        }

        [Fact]
        public void Example_LessonBuilder_CreateLesson()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Act
            var lesson = new LessonBuilder()
                .WithName("Docker Basics")
                .WithSubject("Containers")
                .WithTotalHours(2.5)
                .WithCourseId(courseId)
                .Build();

            // Assert
            lesson.Name.Should().Be("Docker Basics");
            lesson.Subject.Should().Be("Containers");
            lesson.TotalHours.Should().Be(2.5);
            lesson.CourseId.Should().Be(courseId);
        }

        [Fact]
        public void Example_LessonBuilder_PreConfigured()
        {
            // Arrange & Act
            var lesson = new LessonBuilder()
                .AsDockerIntroduction()
                .Build();

            // Assert
            lesson.Name.Should().Be("Introduction to Docker");
            lesson.Subject.Should().Be("Container Basics");
            lesson.TotalHours.Should().Be(2.5);
        }

        [Fact]
        public void Example_LessonBuilder_BuildMany()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Act
            var lessons = LessonBuilder.BuildMany(3, courseId);

            // Assert
            lessons.Should().HaveCount(3);
            lessons.Should().AllSatisfy(l => l.CourseId.Should().Be(courseId));
        }

        [Fact]
        public void Example_CompleteDevOpsCourse_WithLessons()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Act
            var course = new CourseBuilder()
                .WithId(courseId)
                .AsDevOpsCourse()
                .Build();

            var lessons = LessonBuilder.BuildDevOpsCourseLessons(courseId);

            // Assert
            course.Should().NotBeNull();
            lessons.Should().HaveCount(4);
            lessons.Should().AllSatisfy(l => l.CourseId.Should().Be(courseId));

            // Verify specific lessons
            lessons.Should().Contain(l => l.Name == "Docker Basics");
            lessons.Should().Contain(l => l.Name == "Kubernetes Essentials");
            lessons.Should().Contain(l => l.Name == "CI/CD Pipelines");
        }

        [Fact]
        public void Example_BuildersCombined_CreateRelatedEntities()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Act
            var course = new CourseBuilder()
                .WithId(courseId)
                .WithName("Complete DevOps Bootcamp")
                .WithPrice(299.99)
                .Build();

            var lesson1 = new LessonBuilder()
                .WithCourseId(courseId)
                .AsDockerIntroduction()
                .Build();

            var lesson2 = new LessonBuilder()
                .WithCourseId(courseId)
                .AsKubernetesLesson()
                .Build();

            // Assert
            course.Id.Should().Be(courseId);
            lesson1.CourseId.Should().Be(courseId);
            lesson2.CourseId.Should().Be(courseId);

            // Verify relationship
            new[] { lesson1, lesson2 }
                .Should().AllSatisfy(l => l.CourseId.Should().Be(course.Id));
        }
    }
}
