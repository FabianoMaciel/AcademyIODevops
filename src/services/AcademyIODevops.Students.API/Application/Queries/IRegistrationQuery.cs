using AcademyIODevops.Students.API.Application.Queries.ViewModels;

namespace AcademyIODevops.Students.API.Application.Queries
{
    public interface IRegistrationQuery
    {
        List<RegistrationViewModel> GetByStudent(Guid studentId);

        List<RegistrationViewModel> GetAllRegistrations();
    }
}
