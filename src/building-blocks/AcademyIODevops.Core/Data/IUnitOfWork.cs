using AcademyIODevops.Core.DomainObjects;

namespace AcademyIODevops.Core.Data
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
    }
}
