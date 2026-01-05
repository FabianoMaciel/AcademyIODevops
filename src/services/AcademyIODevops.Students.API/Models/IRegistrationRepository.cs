using AcademyIODevops.Core.Data;
using AcademyIODevops.Core.DomainObjects;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Students.API.Models
{
    public interface IRegistrationRepository : IRepository<User>
    {
        Task<Registration> FinishCourse(Guid studentId, Guid courseId);
        Registration AddRegistration(Guid studentId, Guid courseId);
        List<Registration> GetRegistrationByStudent(Guid studentId);
        List<Registration> GetAllRegistrations();
    }
}
