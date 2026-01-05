using AcademyIODevops.Core.Data;

namespace AcademyIODevops.Students.API.Models
{
    public interface IUserRepository : IRepository<StudentUser>
    {
        Task<IEnumerable<StudentUser>> GetStudents();
        Task<IEnumerable<StudentUser>> GetAllUsers();
        Task<StudentUser> GetById(Guid id);
        void Add(StudentUser user);
        Task<StudentUser> GetByEmail(string email);
    }
}
