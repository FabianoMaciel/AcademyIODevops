using AcademyIODevops.Core.DomainObjects;

namespace AcademyIODevops.Core.Data
{
    public interface IRepository<T> : IDisposable where T : IAggregateRoot
    {
        IUnitOfWork UnitOfWork { get; }
    }
}
