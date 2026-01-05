using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Bff.Models
{
    [ExcludeFromCodeCoverage]
    public class LessonProgressViewModel
    {
        public string LessonName { get; set; }
        public string ProgressLesson { get; set; }
    }
}
