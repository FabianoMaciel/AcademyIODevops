using AcademyIODevops.Core.DomainObjects;
using AcademyIODevops.Students.API.Models;
using AcademyIODevops.Students.API.Tests.Builders;
using FluentAssertions;

namespace AcademyIODevops.Students.API.Tests.Unit.Domain
{
    public class CertificationTests
    {
        [Fact]
        public void Certification_ShouldCreateWithValidProperties()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            // Act
            var certification = new CertificationBuilder()
                .WithCourseId(courseId)
                .WithStudentId(studentId)
                .Build();

            // Assert
            certification.Should().NotBeNull();
            certification.CourseId.Should().Be(courseId);
            certification.StudentId.Should().Be(studentId);
        }

        [Fact]
        public void Certification_ShouldInheritFromEntity()
        {
            // Arrange & Act
            var certification = new CertificationBuilder().Build();

            // Assert
            certification.Should().BeAssignableTo<Entity>();
            certification.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Certification_ShouldHaveStudentNavigation_AsNullable()
        {
            // Arrange & Act
            var certification = new CertificationBuilder().Build();

            // Assert
            certification.Student.Should().BeNull();
        }

        [Fact]
        public void Certification_ShouldAcceptValidCourseId()
        {
            // Arrange
            var courseId = Guid.Parse("12345678-1234-1234-1234-123456789012");

            // Act
            var certification = new CertificationBuilder()
                .WithCourseId(courseId)
                .Build();

            // Assert
            certification.CourseId.Should().Be(courseId);
        }

        [Fact]
        public void Certification_ShouldAcceptValidStudentId()
        {
            // Arrange
            var studentId = Guid.Parse("87654321-4321-4321-4321-210987654321");

            // Act
            var certification = new CertificationBuilder()
                .WithStudentId(studentId)
                .Build();

            // Assert
            certification.StudentId.Should().Be(studentId);
        }

        [Fact]
        public void Certification_ShouldGenerateUniqueIds_ForMultipleCertifications()
        {
            // Arrange & Act
            var certification1 = new CertificationBuilder().Build();
            var certification2 = new CertificationBuilder().Build();
            var certification3 = new CertificationBuilder().Build();

            // Assert
            var ids = new[] { certification1.Id, certification2.Id, certification3.Id };
            ids.Distinct().Should().HaveCount(3);
            ids.Should().OnlyContain(id => id != Guid.Empty);
        }

        [Fact]
        public void Certification_ShouldAllowEmptyIds_ButShouldBeValidated()
        {
            // Arrange & Act
            var certification = new CertificationBuilder()
                .WithEmptyCourseId()
                .WithEmptyStudentId()
                .Build();

            // Assert - domínio permite, mas validação de negócio deve prevenir
            certification.CourseId.Should().Be(Guid.Empty);
            certification.StudentId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void Certification_ShouldSetAllPropertiesCorrectly()
        {
            // Arrange
            var certificationId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            // Act
            var certification = new CertificationBuilder()
                .WithId(certificationId)
                .WithCourseId(courseId)
                .WithStudentId(studentId)
                .Build();

            // Assert
            certification.Id.Should().Be(certificationId);
            certification.CourseId.Should().Be(courseId);
            certification.StudentId.Should().Be(studentId);
        }
    }
}
