using AcademyIODevops.Core.Data;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Courses.API.Models
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<IEnumerable<Course>> GetAll();

        Task<Course> GetById(Guid courseId);

        void Add(Course course);

        bool CourseExists(Guid courseI);

        void Update(Course course);
        void Delete(Course course);
    }
}
