using AcademyIODevops.Courses.API.Application.Queries.ViewModels;

namespace AcademyIODevops.Courses.API.Application.Queries
{
    public interface ICourseQuery
    {
        Task<IEnumerable<CourseViewModel>> GetAll();

        Task<CourseViewModel> GetById(Guid courseId);
    }
}
