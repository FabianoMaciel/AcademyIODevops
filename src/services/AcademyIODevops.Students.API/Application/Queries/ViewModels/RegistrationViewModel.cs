using AcademyIODevops.Core.Enums;

namespace AcademyIODevops.Students.API.Application.Queries.ViewModels
{
    public class RegistrationViewModel
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime RegistrationTime { get; set; }
        public EProgressLesson Status { get; set; }
    }
}
