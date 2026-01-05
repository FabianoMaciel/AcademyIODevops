using AcademyIODevops.Courses.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AcademyIODevops.Courses.API.Tests.Fixtures;

public class RepositoryTestFixture
{
    public CoursesContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<CoursesContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mediatorMock = new Mock<IMediator>();
        return new CoursesContext(options, mediatorMock.Object);
    }

    public void SeedDatabase(CoursesContext context, params object[] entities)
    {
        context.AddRange(entities);
        context.SaveChanges();
    }

    public void ClearDatabase(CoursesContext context)
    {
        context.RemoveRange(context.ChangeTracker.Entries());
        context.SaveChanges();
    }
}
