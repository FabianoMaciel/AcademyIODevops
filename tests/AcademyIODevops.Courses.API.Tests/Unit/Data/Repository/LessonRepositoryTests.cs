using AcademyIODevops.Courses.API.Data.Repository;
using AcademyIODevops.Courses.API.Models;
using AcademyIODevops.Courses.API.Tests.Builders;
using AcademyIODevops.Courses.API.Tests.Fixtures;
using FluentAssertions;

namespace AcademyIODevops.Courses.API.Tests.Unit.Data.Repository
{
    public class LessonRepositoryTests
    {
        private readonly RepositoryTestFixture _fixture;

        public LessonRepositoryTests()
        {
            _fixture = new RepositoryTestFixture();
        }

        [Fact]
        public void Add_ShouldAddLesson_ToDatabase()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new LessonRepository(context);
            var lesson = new LessonBuilder().Build();

            // Act
            repository.Add(lesson);
            context.SaveChanges();

            // Assert
            var lessonInDb = context.Set<Lesson>().Find(lesson.Id);
            lessonInDb.Should().NotBeNull();
            lessonInDb!.Id.Should().Be(lesson.Id);
            lessonInDb.Name.Should().Be(lesson.Name);
        }

        [Fact]
        public void Add_ShouldPreserveAllLessonProperties()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new LessonRepository(context);
            var courseId = Guid.NewGuid();
            var lesson = new LessonBuilder()
                .WithName("Docker Introduction")
                .WithSubject("Containers")
                .WithCourseId(courseId)
                .WithTotalHours(2.5)
                .Build();

            // Act
            repository.Add(lesson);
            context.SaveChanges();

            // Assert
            var lessonInDb = context.Set<Lesson>().Find(lesson.Id);
            lessonInDb.Should().NotBeNull();
            lessonInDb!.Name.Should().Be("Docker Introduction");
            lessonInDb.Subject.Should().Be("Containers");
            lessonInDb.CourseId.Should().Be(courseId);
            lessonInDb.TotalHours.Should().Be(2.5);
        }

        [Fact]
        public async Task GetAll_ShouldReturnAllLessons()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new LessonRepository(context);
            var courseId = Guid.NewGuid();
            var lessons = LessonBuilder.BuildMany(3, courseId);

            _fixture.SeedDatabase(context, lessons.ToArray());

            // Act
            var result = await repository.GetAll();

            // Assert
            result.Should().HaveCount(3);
        }

        [Fact]
        public async Task GetAll_ShouldReturnEmptyList_WhenNoLessons()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new LessonRepository(context);

            // Act
            var result = await repository.GetAll();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetByCourseId_ShouldReturnLessonsForSpecificCourse()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new LessonRepository(context);
            var courseId = Guid.NewGuid();
            var otherCourseId = Guid.NewGuid();

            var lesson1 = new LessonBuilder().WithCourseId(courseId).Build();
            var lesson2 = new LessonBuilder().WithCourseId(courseId).Build();
            var lesson3 = new LessonBuilder().WithCourseId(otherCourseId).Build();

            _fixture.SeedDatabase(context, lesson1, lesson2, lesson3);

            // Act
            var result = await repository.GetByCourseId(courseId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().AllSatisfy(l => l.CourseId.Should().Be(courseId));
            result.Should().Contain(l => l.Id == lesson1.Id);
            result.Should().Contain(l => l.Id == lesson2.Id);
            result.Should().NotContain(l => l.Id == lesson3.Id);
        }

        [Fact]
        public async Task GetByCourseId_ShouldReturnEmptyList_WhenNoCourseMatches()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new LessonRepository(context);
            var courseId = Guid.NewGuid();
            var nonExistentCourseId = Guid.NewGuid();

            var lesson = new LessonBuilder().WithCourseId(courseId).Build();
            _fixture.SeedDatabase(context, lesson);

            // Act
            var result = await repository.GetByCourseId(nonExistentCourseId);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Add_ShouldUpdateExistingLesson_WhenContextTracked()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new LessonRepository(context);
            var lesson = new LessonBuilder()
                .WithName("Original Name")
                .WithTotalHours(2.0)
                .Build();

            _fixture.SeedDatabase(context, lesson);

            // Act
            var lessonFromDb = context.Set<Lesson>().Find(lesson.Id);
            lessonFromDb!.Name = "Updated Name";
            lessonFromDb.TotalHours = 3.5;
            context.SaveChanges();

            // Assert
            var updatedLesson = context.Set<Lesson>().Find(lesson.Id);
            updatedLesson.Should().NotBeNull();
            updatedLesson!.Name.Should().Be("Updated Name");
            updatedLesson.TotalHours.Should().Be(3.5);
        }

        [Fact]
        public void UnitOfWork_ShouldReturnContext()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new LessonRepository(context);

            // Act
            var unitOfWork = repository.UnitOfWork;

            // Assert
            unitOfWork.Should().Be(context);
        }

        [Theory]
        [InlineData(1.0)]
        [InlineData(2.5)]
        [InlineData(5.0)]
        public void Add_ShouldHandleDifferentTotalHours(double totalHours)
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new LessonRepository(context);
            var lesson = new LessonBuilder()
                .WithTotalHours(totalHours)
                .Build();

            // Act
            repository.Add(lesson);
            context.SaveChanges();

            // Assert
            var lessonInDb = context.Set<Lesson>().Find(lesson.Id);
            lessonInDb.Should().NotBeNull();
            lessonInDb!.TotalHours.Should().Be(totalHours);
        }


        [Fact]
        public void Add_ShouldAddMultipleLessons_ToSameCourse()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new LessonRepository(context);
            var courseId = Guid.NewGuid();

            var lesson1 = new LessonBuilder()
                .WithName("Lesson 1")
                .WithCourseId(courseId)
                .Build();

            var lesson2 = new LessonBuilder()
                .WithName("Lesson 2")
                .WithCourseId(courseId)
                .Build();

            var lesson3 = new LessonBuilder()
                .WithName("Lesson 3")
                .WithCourseId(courseId)
                .Build();

            // Act
            repository.Add(lesson1);
            repository.Add(lesson2);
            repository.Add(lesson3);
            context.SaveChanges();

            // Assert
            var lessonsInDb = context.Set<Lesson>()
                .Where(l => l.CourseId == courseId)
                .ToList();

            lessonsInDb.Should().HaveCount(3);
            lessonsInDb.Should().Contain(l => l.Name == "Lesson 1");
            lessonsInDb.Should().Contain(l => l.Name == "Lesson 2");
            lessonsInDb.Should().Contain(l => l.Name == "Lesson 3");
        }
    }
}
