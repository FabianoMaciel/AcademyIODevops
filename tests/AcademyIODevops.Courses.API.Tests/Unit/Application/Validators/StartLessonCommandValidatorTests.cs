using AcademyIODevops.Courses.API.Application.Commands;
using FluentAssertions;

namespace AcademyIODevops.Courses.API.Tests.Unit.Application.Validators
{
    public class StartLessonCommandValidatorTests
    {
        [Fact]
        public void Validate_ShouldReturnValid_WhenAllFieldsAreValid()
        {
            // Arrange
            var command = new StartLessonCommand(
                lessonId: Guid.NewGuid(),
                studentId: Guid.NewGuid()
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_ShouldReturnInvalid_WhenLessonIdIsEmpty()
        {
            // Arrange
            var command = new StartLessonCommand(
                lessonId: Guid.Empty,
                studentId: Guid.NewGuid()
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == StartLessonCommandValidation.LessonIdError);
        }

        [Fact]
        public void Validate_ShouldReturnInvalid_WhenStudentIdIsEmpty()
        {
            // Arrange
            var command = new StartLessonCommand(
                lessonId: Guid.NewGuid(),
                studentId: Guid.Empty
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == StartLessonCommandValidation.StudentIdError);
        }

        [Fact]
        public void Validate_ShouldReturnMultipleErrors_WhenBothFieldsAreInvalid()
        {
            // Arrange
            var command = new StartLessonCommand(
                lessonId: Guid.Empty,
                studentId: Guid.Empty
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().HaveCount(2);
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == StartLessonCommandValidation.LessonIdError);
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == StartLessonCommandValidation.StudentIdError);
        }

        [Fact]
        public void Validate_ShouldReturnValid_WithDifferentValidGuids()
        {
            // Arrange
            var lessonId = Guid.Parse("550e8400-e29b-41d4-a716-446655440000");
            var studentId = Guid.Parse("6ba7b810-9dad-11d1-80b4-00c04fd430c8");
            var command = new StartLessonCommand(lessonId, studentId);

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }
    }
}
