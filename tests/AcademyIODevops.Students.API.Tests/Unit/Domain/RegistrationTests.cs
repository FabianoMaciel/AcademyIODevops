using AcademyIODevops.Core.Enums;
using AcademyIODevops.Students.API.Models;
using AcademyIODevops.Students.API.Tests.Builders;
using FluentAssertions;

namespace AcademyIODevops.Students.API.Tests.Unit.Domain
{
    public class RegistrationTests
    {
        [Fact]
        public void Registration_ShouldCreateValidRegistration_WhenPropertiesAreValid()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var registrationTime = DateTime.UtcNow;

            // Act
            var registration = new Registration(studentId, courseId, registrationTime);

            // Assert
            registration.Should().NotBeNull();
            registration.StudentId.Should().Be(studentId);
            registration.CourseId.Should().Be(courseId);
            registration.RegistrationTime.Should().Be(registrationTime);
            registration.Status.Should().Be(EProgressLesson.NotStarted);
        }

        [Fact]
        public void Registration_ShouldHaveNotStartedStatus_WhenCreated()
        {
            // Arrange & Act
            var registration = new RegistrationBuilder().Build();

            // Assert
            registration.Status.Should().Be(EProgressLesson.NotStarted);
        }

        [Fact]
        public void Registration_ShouldGenerateUniqueIds_WhenMultipleRegistrationsCreated()
        {
            // Arrange & Act
            var registration1 = new RegistrationBuilder().Build();
            var registration2 = new RegistrationBuilder().Build();

            // Assert
            registration1.Id.Should().NotBe(registration2.Id);
            registration1.Id.Should().NotBeEmpty();
            registration2.Id.Should().NotBeEmpty();
        }

        [Theory]
        [InlineData(EProgressLesson.NotStarted)]
        [InlineData(EProgressLesson.InProgress)]
        [InlineData(EProgressLesson.Completed)]
        public void Registration_ShouldAcceptAllStatusValues(EProgressLesson status)
        {
            // Arrange & Act
            var registration = new RegistrationBuilder()
                .WithStatus(status)
                .Build();

            // Assert
            registration.Status.Should().Be(status);
        }

        [Fact]
        public void Registration_ShouldAllowStatusUpdate_FromNotStartedToInProgress()
        {
            // Arrange
            var registration = new RegistrationBuilder()
                .AsNotStarted()
                .Build();

            // Act
            registration.Status = EProgressLesson.InProgress;

            // Assert
            registration.Status.Should().Be(EProgressLesson.InProgress);
        }

        [Fact]
        public void Registration_ShouldAllowStatusUpdate_FromInProgressToCompleted()
        {
            // Arrange
            var registration = new RegistrationBuilder()
                .AsInProgress()
                .Build();

            // Act
            registration.Status = EProgressLesson.Completed;

            // Assert
            registration.Status.Should().Be(EProgressLesson.Completed);
        }

        [Fact]
        public void Registration_ShouldAcceptValidStudentId()
        {
            // Arrange
            var studentId = Guid.NewGuid();

            // Act
            var registration = new RegistrationBuilder()
                .WithStudentId(studentId)
                .Build();

            // Assert
            registration.StudentId.Should().Be(studentId);
        }

        [Fact]
        public void Registration_ShouldAcceptValidCourseId()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Act
            var registration = new RegistrationBuilder()
                .WithCourseId(courseId)
                .Build();

            // Assert
            registration.CourseId.Should().Be(courseId);
        }

        [Fact]
        public void Registration_ShouldAllowEmptyStudentId_ButValidationShouldCatchIt()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var registrationTime = DateTime.UtcNow;

            // Act
            var registration = new Registration(Guid.Empty, courseId, registrationTime);

            // Assert
            // A entidade permite, mas o Command Validator deve rejeitar
            registration.StudentId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void Registration_ShouldAllowEmptyCourseId_ButValidationShouldCatchIt()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var registrationTime = DateTime.UtcNow;

            // Act
            var registration = new Registration(studentId, Guid.Empty, registrationTime);

            // Assert
            // A entidade permite, mas o Command Validator deve rejeitar
            registration.CourseId.Should().Be(Guid.Empty);
        }

        [Fact]
        public void Registration_ShouldAcceptPastRegistrationTime()
        {
            // Arrange
            var pastDate = DateTime.UtcNow.AddMonths(-6);

            // Act
            var registration = new RegistrationBuilder()
                .WithRegistrationTime(pastDate)
                .Build();

            // Assert
            registration.RegistrationTime.Should().Be(pastDate);
        }

        [Fact]
        public void Registration_ShouldAcceptRecentRegistrationTime()
        {
            // Arrange
            var recentDate = DateTime.UtcNow.AddDays(-1);

            // Act
            var registration = new RegistrationBuilder()
                .WithRegistrationTime(recentDate)
                .Build();

            // Assert
            registration.RegistrationTime.Should().Be(recentDate);
        }

        [Fact]
        public void Registration_ShouldAllowAssociationWithStudent()
        {
            // Arrange
            var student = new StudentUserBuilder().Build();

            // Act
            var registration = new RegistrationBuilder()
                .WithStudent(student)
                .Build();

            // Assert
            registration.Student.Should().NotBeNull();
            registration.Student.Should().Be(student);
            registration.StudentId.Should().Be(student.Id);
        }

        [Fact]
        public void Registration_ShouldPreserveIds_WhenStatusIsUpdated()
        {
            // Arrange
            var originalStudentId = Guid.NewGuid();
            var originalCourseId = Guid.NewGuid();
            var registration = new RegistrationBuilder()
                .WithStudentId(originalStudentId)
                .WithCourseId(originalCourseId)
                .Build();

            // Act
            registration.Status = EProgressLesson.Completed;

            // Assert
            registration.StudentId.Should().Be(originalStudentId);
            registration.CourseId.Should().Be(originalCourseId);
        }

        [Fact]
        public void Registration_ShouldAllowMultipleRegistrations_ForSameStudent()
        {
            // Arrange
            var studentId = Guid.NewGuid();

            // Act
            var registrations = RegistrationBuilder.BuildMany(3, studentId);

            // Assert
            registrations.Should().HaveCount(3);
            registrations.Should().OnlyContain(r => r.StudentId == studentId);
            registrations.Select(r => r.CourseId).Distinct().Should().HaveCount(3);
        }

        [Fact]
        public void Registration_ShouldCreateWithDifferentStatuses()
        {
            // Arrange
            var studentId = Guid.NewGuid();

            // Act
            var registrations = RegistrationBuilder.BuildWithDifferentStatuses(studentId);

            // Assert
            registrations.Should().HaveCount(3);
            registrations.Should().Contain(r => r.Status == EProgressLesson.NotStarted);
            registrations.Should().Contain(r => r.Status == EProgressLesson.InProgress);
            registrations.Should().Contain(r => r.Status == EProgressLesson.Completed);
        }
    }
}
