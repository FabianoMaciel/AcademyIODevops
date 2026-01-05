using AcademyIODevops.Core.Enums;
using AcademyIODevops.Core.DomainObjects;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Students.API.Models
{
    [ExcludeFromCodeCoverage]
    public class Registration : Entity
    {
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime RegistrationTime { get; set; }
        public EProgressLesson Status { get; set; }
        public StudentUser Student { get; set; }

        public Registration(Guid studentId, Guid courseId, DateTime registrationTime)
        {
            StudentId = studentId;
            CourseId = courseId;
            RegistrationTime = registrationTime;
            Status = EProgressLesson.NotStarted;
        }
    }
}
