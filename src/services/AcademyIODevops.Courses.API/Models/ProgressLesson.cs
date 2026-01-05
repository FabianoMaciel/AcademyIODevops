using AcademyIODevops.Core.DomainObjects;
using AcademyIODevops.Core.Enums;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Courses.API.Models
{
    [ExcludeFromCodeCoverage]
    public class ProgressLesson : Entity, IAggregateRoot
    {
        public ProgressLesson(Guid lessonId, Guid studentId, EProgressLesson progressLesson)
        {
            this.LessonId = lessonId;
            this.StudentId = studentId;
            this.ProgressionStatus = progressLesson;
        }

        public ProgressLesson() { }

        public Guid StudentId { get; set; }
        public Guid LessonId { get; set; }
        public EProgressLesson ProgressionStatus { get; set; }
        public virtual Lesson Lesson { get; set; }
    }
}
