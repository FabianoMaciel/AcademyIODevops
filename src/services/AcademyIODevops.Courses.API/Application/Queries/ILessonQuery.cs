using AcademyIODevops.Core.Enums;
using AcademyIODevops.Courses.API.Application.Queries.ViewModels;

namespace AcademyIODevops.Courses.API.Application.Queries
{
    public interface ILessonQuery
    {
        Task<IEnumerable<LessonViewModel>> GetAll();
        Task<IEnumerable<LessonViewModel>> GetByCourseId(Guid courseId);
        Task<IEnumerable<LessonProgressViewModel>> GetProgress(Guid studentId);
        EProgressLesson GetProgressStatusLesson(Guid lessonId, Guid studentId);
        bool ExistsProgress(Guid lessonId, Guid studentId);
    }
}
