using AcademyIODevops.Core.DomainObjects;

namespace AcademyIODevops.Students.API.Models
{
    public class Certification : Entity
    {
        public Guid CourseId { get; set; }
        public Guid StudentId { get; set; }
        public StudentUser? Student { get; private set; }
    }
}
