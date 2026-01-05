using AcademyIODevops.Courses.API.Data.Repository;
using AcademyIODevops.Courses.API.Models;
using AcademyIODevops.Courses.API.Tests.Builders;
using AcademyIODevops.Courses.API.Tests.Fixtures;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace AcademyIODevops.Courses.API.Tests.Unit.Data.Repository;

public class CourseRepositoryTests
{
    private readonly RepositoryTestFixture _fixture;

    public CourseRepositoryTests()
    {
        _fixture = new RepositoryTestFixture();
    }

    [Fact]
    public void Add_ShouldAddCourse_ToDatabase()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CourseRepository(context);
        var course = new CourseBuilder().Build();

        // Act
        repository.Add(course);
        context.SaveChanges();

        // Assert
        var courseInDb = context.Set<Course>().Find(course.Id);
        courseInDb.Should().NotBeNull();
        courseInDb!.Id.Should().Be(course.Id);
        courseInDb.Name.Should().Be(course.Name);
        courseInDb.Price.Should().Be(course.Price);
    }

    [Fact]
    public void Add_ShouldPreserveAllCourseProperties()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CourseRepository(context);
        var course = new CourseBuilder()
            .WithName("DevOps Course")
            .WithDescription("Learn DevOps")
            .WithPrice(500)
            .Build();

        // Act
        repository.Add(course);
        context.SaveChanges();

        // Assert
        var courseInDb = context.Set<Course>().Find(course.Id);
        courseInDb.Should().NotBeNull();
        courseInDb!.Name.Should().Be("DevOps Course");
        courseInDb.Description.Should().Be("Learn DevOps");
        courseInDb.Price.Should().Be(500);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAllCourses()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CourseRepository(context);
        var courses = CourseBuilder.BuildMany(3);

        _fixture.SeedDatabase(context, courses.ToArray());

        // Act
        var result = await repository.GetAll();

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmptyList_WhenNoCourses()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CourseRepository(context);

        // Act
        var result = await repository.GetAll();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetById_ShouldReturnCourse_WhenCourseExists()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CourseRepository(context);
        var course = new CourseBuilder().Build();

        _fixture.SeedDatabase(context, course);

        // Act
        var result = await repository.GetById(course.Id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(course.Id);
        result.Name.Should().Be(course.Name);
    }

    [Fact]
    public async Task GetById_ShouldReturnNull_WhenCourseDoesNotExist()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CourseRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await repository.GetById(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void CourseExists_ShouldReturnTrue_WhenCourseExists()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CourseRepository(context);
        var course = new CourseBuilder().Build();

        _fixture.SeedDatabase(context, course);

        // Act
        var exists = repository.CourseExists(course.Id);

        // Assert
        exists.Should().BeTrue();
    }

    [Fact]
    public void CourseExists_ShouldReturnFalse_WhenCourseDoesNotExist()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CourseRepository(context);
        var nonExistentId = Guid.NewGuid();

        // Act
        var exists = repository.CourseExists(nonExistentId);

        // Assert
        exists.Should().BeFalse();
    }

    [Fact]
    public void Update_ShouldUpdateCourse_InDatabase()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CourseRepository(context);
        var course = new CourseBuilder()
            .WithName("Original Name")
            .WithPrice(100)
            .Build();

        _fixture.SeedDatabase(context, course);

        // Act
        course.Name = "Updated Name";
        course.Price = 200;
        repository.Update(course);
        context.SaveChanges();

        // Assert
        var courseInDb = context.Set<Course>().Find(course.Id);
        courseInDb.Should().NotBeNull();
        courseInDb!.Name.Should().Be("Updated Name");
        courseInDb.Price.Should().Be(200);
    }

    [Fact]
    public void UnitOfWork_ShouldReturnContext()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CourseRepository(context);

        // Act
        var unitOfWork = repository.UnitOfWork;

        // Assert
        unitOfWork.Should().Be(context);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(500)]
    [InlineData(1000)]
    public void Add_ShouldHandleDifferentPrices(double price)
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CourseRepository(context);
        var course = new CourseBuilder()
            .WithPrice(price)
            .Build();

        // Act
        repository.Add(course);
        context.SaveChanges();

        // Assert
        var courseInDb = context.Set<Course>().Find(course.Id);
        courseInDb.Should().NotBeNull();
        courseInDb!.Price.Should().Be(price);
    }

    [Fact]
    public async Task GetAll_ShouldNotReturnDeletedCourses()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CourseRepository(context);
        var activeCourse = new CourseBuilder().Build();
        var deletedCourse = new CourseBuilder().Build();
        deletedCourse.Deleted = true;

        _fixture.SeedDatabase(context, activeCourse, deletedCourse);

        // Act
        var result = await repository.GetAll();

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(c => c.Id == activeCourse.Id);
        result.Should().NotContain(c => c.Id == deletedCourse.Id);
    }

    [Fact]
    public void CourseExists_ShouldReturnTrue_ForDeletedCourses()
    {
        // Arrange
        using var context = _fixture.CreateContext();
        var repository = new CourseRepository(context);
        var course = new CourseBuilder().Build();
        course.Deleted = true;

        _fixture.SeedDatabase(context, course);

        // Act
        var exists = repository.CourseExists(course.Id);

        // Assert
        exists.Should().BeTrue();
    }
}
