using AcademyIODevops.Courses.API.Application.Commands;
using FluentAssertions;

namespace AcademyIODevops.Courses.API.Tests.Unit.Application.Validators
{
    public class AddLessonCommandValidatorTests
    {
        [Fact]
        public void Validate_ShouldReturnValid_WhenAllFieldsAreValid()
        {
            // Arrange
            var command = new AddLessonCommand(
                name: "Docker Introduction",
                subject: "Containers",
                courseId: Guid.NewGuid(),
                totalHours: 2.5
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_ShouldReturnInvalid_WhenNameIsEmpty(string invalidName)
        {
            // Arrange
            var command = new AddLessonCommand(
                name: invalidName,
                subject: "Containers",
                courseId: Guid.NewGuid(),
                totalHours: 2.5
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == AddLessonCommandValidation.NameError);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_ShouldReturnInvalid_WhenSubjectIsEmpty(string invalidSubject)
        {
            // Arrange
            var command = new AddLessonCommand(
                name: "Docker Introduction",
                subject: invalidSubject,
                courseId: Guid.NewGuid(),
                totalHours: 2.5
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == AddLessonCommandValidation.SubjectError);
        }

        [Fact]
        public void Validate_ShouldReturnInvalid_WhenCourseIdIsEmpty()
        {
            // Arrange
            var command = new AddLessonCommand(
                name: "Docker Introduction",
                subject: "Containers",
                courseId: Guid.Empty,
                totalHours: 2.5
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == AddLessonCommandValidation.UserCreationError);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-5.5)]
        public void Validate_ShouldReturnInvalid_WhenTotalHoursIsZeroOrNegative(double invalidHours)
        {
            // Arrange
            var command = new AddLessonCommand(
                name: "Docker Introduction",
                subject: "Containers",
                courseId: Guid.NewGuid(),
                totalHours: invalidHours
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == AddLessonCommandValidation.PriceErro);
        }

        [Theory]
        [InlineData("Docker Basics", "DevOps", 1.5)]
        [InlineData("Kubernetes Fundamentals", "Orchestration", 3.0)]
        [InlineData("CI/CD with Jenkins", "Automation", 4.5)]
        public void Validate_ShouldReturnValid_WithDifferentValidData(
            string name, string subject, double totalHours)
        {
            // Arrange
            var command = new AddLessonCommand(
                name: name,
                subject: subject,
                courseId: Guid.NewGuid(),
                totalHours: totalHours
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_ShouldReturnMultipleErrors_WhenMultipleFieldsAreInvalid()
        {
            // Arrange
            var command = new AddLessonCommand(
                name: "",
                subject: "",
                courseId: Guid.Empty,
                totalHours: -1
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().HaveCount(4);
        }
    }
}
