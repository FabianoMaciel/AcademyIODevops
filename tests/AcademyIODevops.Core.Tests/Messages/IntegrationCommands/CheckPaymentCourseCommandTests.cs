using AcademyIODevops.Core.Messages;
using AcademyIODevops.Core.Messages.IntegrationCommands;
using FluentAssertions;
using Xunit;

namespace AcademyIODevops.Core.Tests.Messages.IntegrationCommands
{
    public class CheckPaymentCourseCommandTests
    {
        [Fact]
        public void CheckPaymentCourseCommand_ShouldCreateWithValidParameters()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            // Act
            var command = new CheckPaymentCourseCommand(studentId, courseId);

            // Assert
            command.Should().NotBeNull();
            command.StundentId.Should().Be(studentId);
            command.CourseId.Should().Be(courseId);
        }

        [Fact]
        public void CheckPaymentCourseCommand_ShouldInheritFromCommand()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            // Act
            var command = new CheckPaymentCourseCommand(studentId, courseId);

            // Assert
            command.Should().BeAssignableTo<Command>();
        }

        [Fact]
        public void IsValid_ShouldReturnTrue_WhenAllPropertiesAreValid()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var command = new CheckPaymentCourseCommand(studentId, courseId);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
            command.ValidationResult.Should().NotBeNull();
            command.ValidationResult.IsValid.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenStudentIdIsEmpty()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var command = new CheckPaymentCourseCommand(Guid.Empty, courseId);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.ValidationResult.Should().NotBeNull();
            command.ValidationResult.IsValid.Should().BeFalse();
            command.ValidationResult.Errors.Should().HaveCount(1);
            command.ValidationResult.Errors[0].ErrorMessage.Should().Be(CheckPaymentCourseCommandValidation.StudentIdError);
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenCourseIdIsEmpty()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var command = new CheckPaymentCourseCommand(studentId, Guid.Empty);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.ValidationResult.Should().NotBeNull();
            command.ValidationResult.IsValid.Should().BeFalse();
            command.ValidationResult.Errors.Should().HaveCount(1);
            command.ValidationResult.Errors[0].ErrorMessage.Should().Be(CheckPaymentCourseCommandValidation.CourseIdError);
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenBothIdsAreEmpty()
        {
            // Arrange
            var command = new CheckPaymentCourseCommand(Guid.Empty, Guid.Empty);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.ValidationResult.Should().NotBeNull();
            command.ValidationResult.IsValid.Should().BeFalse();
            command.ValidationResult.Errors.Should().HaveCount(2);
            command.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == CheckPaymentCourseCommandValidation.StudentIdError);
            command.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == CheckPaymentCourseCommandValidation.CourseIdError);
        }

        [Fact]
        public void CheckPaymentCourseCommand_ShouldAllowSettingStudentId()
        {
            // Arrange
            var initialStudentId = Guid.NewGuid();
            var newStudentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var command = new CheckPaymentCourseCommand(initialStudentId, courseId);

            // Act
            command.StundentId = newStudentId;

            // Assert
            command.StundentId.Should().Be(newStudentId);
        }

        [Fact]
        public void CheckPaymentCourseCommand_ShouldAllowSettingCourseId()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var initialCourseId = Guid.NewGuid();
            var newCourseId = Guid.NewGuid();
            var command = new CheckPaymentCourseCommand(studentId, initialCourseId);

            // Act
            command.CourseId = newCourseId;

            // Assert
            command.CourseId.Should().Be(newCourseId);
        }

        [Theory]
        [InlineData("11111111-1111-1111-1111-111111111111", "22222222-2222-2222-2222-222222222222")]
        [InlineData("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")]
        [InlineData("12345678-1234-1234-1234-123456789012", "87654321-4321-4321-4321-210987654321")]
        public void IsValid_ShouldReturnTrue_ForDifferentValidGuids(string studentIdStr, string courseIdStr)
        {
            // Arrange
            var studentId = Guid.Parse(studentIdStr);
            var courseId = Guid.Parse(courseIdStr);
            var command = new CheckPaymentCourseCommand(studentId, courseId);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_ShouldBeIdempotent()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var command = new CheckPaymentCourseCommand(studentId, courseId);

            // Act
            var isValid1 = command.IsValid();
            var isValid2 = command.IsValid();
            var isValid3 = command.IsValid();

            // Assert
            isValid1.Should().Be(isValid2);
            isValid2.Should().Be(isValid3);
        }

        [Fact]
        public void IsValid_ShouldUpdateValidationResult_AfterPropertyChange()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var command = new CheckPaymentCourseCommand(studentId, Guid.Empty);

            // Act
            var isValidBefore = command.IsValid();
            command.CourseId = Guid.NewGuid();
            var isValidAfter = command.IsValid();

            // Assert
            isValidBefore.Should().BeFalse();
            isValidAfter.Should().BeTrue();
        }

        [Fact]
        public void CheckPaymentCourseCommand_ShouldAcceptSameGuidForBothIds()
        {
            // Arrange
            var sameId = Guid.NewGuid();

            // Act
            var command = new CheckPaymentCourseCommand(sameId, sameId);

            // Assert
            command.StundentId.Should().Be(sameId);
            command.CourseId.Should().Be(sameId);
            command.IsValid().Should().BeTrue();
        }
    }

    public class CheckPaymentCourseCommandValidationTests
    {
        [Fact]
        public void Validate_ShouldPass_WhenAllPropertiesAreValid()
        {
            // Arrange
            var validator = new CheckPaymentCourseCommandValidation();
            var command = new CheckPaymentCourseCommand(Guid.NewGuid(), Guid.NewGuid());

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_ShouldFail_WhenStudentIdIsEmpty()
        {
            // Arrange
            var validator = new CheckPaymentCourseCommandValidation();
            var command = new CheckPaymentCourseCommand(Guid.Empty, Guid.NewGuid());

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].PropertyName.Should().Be(nameof(CheckPaymentCourseCommand.StundentId));
            result.Errors[0].ErrorMessage.Should().Be(CheckPaymentCourseCommandValidation.StudentIdError);
        }

        [Fact]
        public void Validate_ShouldFail_WhenCourseIdIsEmpty()
        {
            // Arrange
            var validator = new CheckPaymentCourseCommandValidation();
            var command = new CheckPaymentCourseCommand(Guid.NewGuid(), Guid.Empty);

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].PropertyName.Should().Be(nameof(CheckPaymentCourseCommand.CourseId));
            result.Errors[0].ErrorMessage.Should().Be(CheckPaymentCourseCommandValidation.CourseIdError);
        }

        [Fact]
        public void Validate_ShouldFail_WhenBothIdsAreEmpty()
        {
            // Arrange
            var validator = new CheckPaymentCourseCommandValidation();
            var command = new CheckPaymentCourseCommand(Guid.Empty, Guid.Empty);

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(2);
        }

        [Fact]
        public void Validation_ShouldHaveCorrectErrorMessages()
        {
            // Assert
            CheckPaymentCourseCommandValidation.StudentIdError.Should().Be("O campo AlunoId não pode ser vazio.");
            CheckPaymentCourseCommandValidation.CourseIdError.Should().Be("O campo CursoId não pode ser vazio.");
        }

        [Fact]
        public void Validator_ShouldBeReusable()
        {
            // Arrange
            var validator = new CheckPaymentCourseCommandValidation();
            var command1 = new CheckPaymentCourseCommand(Guid.NewGuid(), Guid.NewGuid());
            var command2 = new CheckPaymentCourseCommand(Guid.Empty, Guid.NewGuid());

            // Act
            var result1 = validator.Validate(command1);
            var result2 = validator.Validate(command2);

            // Assert
            result1.IsValid.Should().BeTrue();
            result2.IsValid.Should().BeFalse();
        }

        [Theory]
        [InlineData("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")]
        [InlineData("12345678-90ab-cdef-1234-567890abcdef", "fedcba09-8765-4321-fedc-ba0987654321")]
        public void Validate_ShouldPass_ForDifferentValidGuids(string studentIdStr, string courseIdStr)
        {
            // Arrange
            var validator = new CheckPaymentCourseCommandValidation();
            var studentId = Guid.Parse(studentIdStr);
            var courseId = Guid.Parse(courseIdStr);
            var command = new CheckPaymentCourseCommand(studentId, courseId);

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
