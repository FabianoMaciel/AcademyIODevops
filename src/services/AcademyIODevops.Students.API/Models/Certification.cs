using AcademyIODevops.Core.DomainObjects;
using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Students.API.Models
{
    [ExcludeFromCodeCoverage]
    public class Certification : Entity
    {
        public Guid CourseId { get; set; }
        public Guid StudentId { get; set; }
        public StudentUser? Student { get; private set; }
    }
}
