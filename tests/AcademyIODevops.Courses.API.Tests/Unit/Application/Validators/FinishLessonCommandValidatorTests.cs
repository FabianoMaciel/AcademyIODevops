using AcademyIODevops.Courses.API.Application.Commands;
using FluentAssertions;

namespace AcademyIODevops.Courses.API.Tests.Unit.Application.Validators
{
    public class FinishLessonCommandValidatorTests
    {
        [Fact]
        public void Validate_ShouldReturnValid_WhenAllFieldsAreValid()
        {
            // Arrange
            var command = new FinishLessonCommand(
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
            var command = new FinishLessonCommand(
                lessonId: Guid.Empty,
                studentId: Guid.NewGuid()
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == FinishLessonCommandValidation.LessonIdError);
        }

        [Fact]
        public void Validate_ShouldReturnInvalid_WhenStudentIdIsEmpty()
        {
            // Arrange
            var command = new FinishLessonCommand(
                lessonId: Guid.NewGuid(),
                studentId: Guid.Empty
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == FinishLessonCommandValidation.StudentIdError);
        }

        [Fact]
        public void Validate_ShouldReturnMultipleErrors_WhenBothFieldsAreInvalid()
        {
            // Arrange
            var command = new FinishLessonCommand(
                lessonId: Guid.Empty,
                studentId: Guid.Empty
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().HaveCount(2);
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == FinishLessonCommandValidation.LessonIdError);
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == FinishLessonCommandValidation.StudentIdError);
        }

        [Fact]
        public void Validate_ShouldReturnValid_WithDifferentValidGuids()
        {
            // Arrange
            var lessonId = Guid.Parse("a1b2c3d4-e5f6-4a5b-8c7d-9e0f1a2b3c4d");
            var studentId = Guid.Parse("b2c3d4e5-f6a7-4b5c-8d7e-9f0a1b2c3d4e");
            var command = new FinishLessonCommand(lessonId, studentId);

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }
    }
}
