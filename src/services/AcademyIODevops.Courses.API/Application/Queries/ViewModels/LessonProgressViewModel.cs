using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Courses.API.Application.Queries.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class LessonProgressViewModel(string lessonName, string progressLesson)
    {
        public string LessonName { get; } = lessonName;
        public string ProgressLesson { get; } = progressLesson;
    }
}
