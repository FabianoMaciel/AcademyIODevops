using AcademyIODevops.Payments.API.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace AcademyIODevops.Payments.API.Tests.Fixtures;

public class RepositoryTestFixture
{
    public PaymentsContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PaymentsContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var mediatorMock = new Mock<IMediator>();
        return new PaymentsContext(options, mediatorMock.Object);
    }

    public void SeedDatabase(PaymentsContext context, params object[] entities)
    {
        context.AddRange(entities);
        context.SaveChanges();
    }

    public void ClearDatabase(PaymentsContext context)
    {
        context.RemoveRange(context.ChangeTracker.Entries());
        context.SaveChanges();
    }
}
