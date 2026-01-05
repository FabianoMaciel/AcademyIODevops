using AcademyIODevops.Courses.API.Application.Commands;
using FluentAssertions;

namespace AcademyIODevops.Courses.API.Tests.Unit.Application.Validators
{
    public class CreateProgressByCourseCommandValidatorTests
    {
        [Fact]
        public void Validate_ShouldReturnValid_WhenAllFieldsAreValid()
        {
            // Arrange
            var command = new CreateProgressByCourseCommand(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid()
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_ShouldReturnInvalid_WhenCourseIdIsEmpty()
        {
            // Arrange
            var command = new CreateProgressByCourseCommand(
                courseId: Guid.Empty,
                studentId: Guid.NewGuid()
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == CreateProgressByCourseCommandValidation.CourseIdError);
        }

        [Fact]
        public void Validate_ShouldReturnInvalid_WhenStudentIdIsEmpty()
        {
            // Arrange
            var command = new CreateProgressByCourseCommand(
                courseId: Guid.NewGuid(),
                studentId: Guid.Empty
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == CreateProgressByCourseCommandValidation.StudentIdError);
        }

        [Fact]
        public void Validate_ShouldReturnMultipleErrors_WhenBothFieldsAreInvalid()
        {
            // Arrange
            var command = new CreateProgressByCourseCommand(
                courseId: Guid.Empty,
                studentId: Guid.Empty
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().HaveCount(2);
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == CreateProgressByCourseCommandValidation.CourseIdError);
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == CreateProgressByCourseCommandValidation.StudentIdError);
        }

        [Theory]
        [InlineData("550e8400-e29b-41d4-a716-446655440000", "6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        [InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479", "6ba7b811-9dad-11d1-80b4-00c04fd430c8")]
        public void Validate_ShouldReturnValid_WithDifferentValidGuids(string courseIdStr, string studentIdStr)
        {
            // Arrange
            var courseId = Guid.Parse(courseIdStr);
            var studentId = Guid.Parse(studentIdStr);
            var command = new CreateProgressByCourseCommand(courseId, studentId);

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }
    }
}
