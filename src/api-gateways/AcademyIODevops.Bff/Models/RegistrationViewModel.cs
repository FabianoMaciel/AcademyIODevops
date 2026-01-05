using AcademyIODevops.Core.Enums;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Bff.Models
{
    [ExcludeFromCodeCoverage]
    public class RegistrationViewModel
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime RegistrationTime { get; set; }
        public EProgressLesson Status { get; set; }
    }
}
