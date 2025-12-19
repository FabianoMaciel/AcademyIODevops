using AcademyIODevops.Courses.API.Tests.Fixtures;
using FluentAssertions;

namespace AcademyIODevops.Courses.API.Tests.Unit.Fixtures
{
    /// <summary>
    /// Exemplo de como usar Fixtures em testes xUnit.
    /// O Fixture é compartilhado entre todos os testes desta classe.
    /// </summary>
    [Collection("Course Collection")]
    public class FixtureUsageExampleTests
    {
        private readonly CourseTestFixture _fixture;

        public FixtureUsageExampleTests(CourseTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Example_UseSampleCourses_FromFixture()
        {
            // Arrange & Act
            var courses = _fixture.SampleCourses;

            // Assert
            courses.Should().HaveCount(3);
            courses.Should().Contain(c => c.Name == "DevOps Fundamentals");
            courses.Should().Contain(c => c.Name == "Docker Deep Dive");
            courses.Should().Contain(c => c.Name == "Kubernetes Mastery");
        }

        [Fact]
        public void Example_UseSpecificCourse_ById()
        {
            // Arrange & Act
            var devOpsCourse = _fixture.GetDevOpsCourse();

            // Assert
            devOpsCourse.Should().NotBeNull();
            devOpsCourse.Id.Should().Be(_fixture.DevOpsCourseId);
            devOpsCourse.Name.Should().Be("DevOps Fundamentals");
        }

        [Fact]
        public void Example_CreateFreshCourse_ForIsolatedTest()
        {
            // Arrange & Act
            var course = _fixture.CreateFreshCourse();

            // Modificar sem afetar outros testes
            course.Name = "Modified Name";
            course.Price = 999.99;

            // Assert
            course.Name.Should().Be("Modified Name");
            course.Price.Should().Be(999.99);

            // O fixture original não é afetado
            _fixture.SampleCourses.Should().NotContain(c => c.Name == "Modified Name");
        }

        [Fact]
        public void Example_UseSampleLessons_FromFixture()
        {
            // Arrange & Act
            var lessons = _fixture.SampleLessons;

            // Assert
            lessons.Should().HaveCount(4);
            lessons.Should().AllSatisfy(l => l.CourseId.Should().Be(_fixture.DevOpsCourseId));
        }

        [Fact]
        public void Example_CreateFreshLesson_ForSpecificCourse()
        {
            // Arrange & Act
            var lesson = _fixture.CreateFreshLesson(_fixture.DockerCourseId);

            // Assert
            lesson.Should().NotBeNull();
            lesson.CourseId.Should().Be(_fixture.DockerCourseId);
        }

        [Fact]
        public void Example_VerifyFixtureIds_AreConsistent()
        {
            // Arrange & Act
            var devOpsCourse = _fixture.GetDevOpsCourse();
            var lessons = _fixture.SampleLessons;

            // Assert
            devOpsCourse.Id.Should().Be(_fixture.DevOpsCourseId);
            lessons.Should().AllSatisfy(l => l.CourseId.Should().Be(_fixture.DevOpsCourseId));
        }
    }

    /// <summary>
    /// Segundo exemplo mostrando que o Fixture é compartilhado na mesma Collection
    /// </summary>
    [Collection("Course Collection")]
    public class AnotherTestUsingTheSameFixture
    {
        private readonly CourseTestFixture _fixture;

        public AnotherTestUsingTheSameFixture(CourseTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public void Example_SameFixtureInstance_SameData()
        {
            // Arrange & Act
            var courses = _fixture.SampleCourses;

            // Assert
            // O mesmo fixture compartilhado pela Collection
            courses.Should().HaveCount(3);
            _fixture.DevOpsCourseId.Should().NotBeEmpty();
        }
    }
}
