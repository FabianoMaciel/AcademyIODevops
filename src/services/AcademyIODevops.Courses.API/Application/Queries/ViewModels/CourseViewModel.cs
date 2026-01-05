using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Courses.API.Application.Queries.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class CourseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}
