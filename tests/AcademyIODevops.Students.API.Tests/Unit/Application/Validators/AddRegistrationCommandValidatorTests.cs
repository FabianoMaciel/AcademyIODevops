using AcademyIODevops.Students.API.Application.Commands;
using FluentAssertions;

namespace AcademyIODevops.Students.API.Tests.Unit.Application.Validators
{
    public class AddRegistrationCommandValidatorTests
    {
        [Fact]
        public void Validate_ShouldBeValid_WhenAllPropertiesAreValid()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var command = new AddRegistrationCommand(studentId, courseId);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
            command.ValidationResult.IsValid.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_ShouldBeInvalid_WhenStudentIdIsEmpty()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var command = new AddRegistrationCommand(Guid.Empty, courseId);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.ValidationResult.Errors
                .Should().Contain(e => e.ErrorMessage == AddRegistrationCommandValidation.StudentIdError);
        }

        [Fact]
        public void Validate_ShouldBeInvalid_WhenCourseIdIsEmpty()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var command = new AddRegistrationCommand(studentId, Guid.Empty);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.ValidationResult.Errors
                .Should().Contain(e => e.ErrorMessage == AddRegistrationCommandValidation.CourseIdError);
        }

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenBothIdsAreEmpty()
        {
            // Arrange
            var command = new AddRegistrationCommand(Guid.Empty, Guid.Empty);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.ValidationResult.Errors.Should().HaveCount(2);
            command.ValidationResult.Errors
                .Should().Contain(e => e.ErrorMessage == AddRegistrationCommandValidation.StudentIdError);
            command.ValidationResult.Errors
                .Should().Contain(e => e.ErrorMessage == AddRegistrationCommandValidation.CourseIdError);
        }

        [Fact]
        public void Validate_ShouldBeValid_WithDifferentValidGuids()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var command = new AddRegistrationCommand(studentId, courseId);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
            command.StudentId.Should().Be(studentId);
            command.CourseId.Should().Be(courseId);
        }

        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012", "87654321-4321-4321-4321-210987654321")]
        [InlineData("11111111-1111-1111-1111-111111111111", "22222222-2222-2222-2222-222222222222")]
        [InlineData("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa", "bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb")]
        public void Validate_ShouldBeValid_WithVariousGuidFormats(string studentIdStr, string courseIdStr)
        {
            // Arrange
            var studentId = Guid.Parse(studentIdStr);
            var courseId = Guid.Parse(courseIdStr);
            var command = new AddRegistrationCommand(studentId, courseId);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldPreserveProperties_WhenValidationFails()
        {
            // Arrange
            var studentId = Guid.Empty; // Inválido
            var courseId = Guid.Empty; // Inválido
            var command = new AddRegistrationCommand(studentId, courseId);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.StudentId.Should().Be(studentId);
            command.CourseId.Should().Be(courseId);
        }

        [Fact]
        public void Validate_ShouldBeValid_WhenSameStudentEnrollsInMultipleCourses()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var courseId2 = Guid.NewGuid();

            var command1 = new AddRegistrationCommand(studentId, courseId1);
            var command2 = new AddRegistrationCommand(studentId, courseId2);

            // Act
            var isValid1 = command1.IsValid();
            var isValid2 = command2.IsValid();

            // Assert
            isValid1.Should().BeTrue();
            isValid2.Should().BeTrue();
            command1.StudentId.Should().Be(command2.StudentId);
            command1.CourseId.Should().NotBe(command2.CourseId);
        }

        [Fact]
        public void Validate_ShouldBeValid_WhenMultipleStudentsEnrollInSameCourse()
        {
            // Arrange
            var studentId1 = Guid.NewGuid();
            var studentId2 = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var command1 = new AddRegistrationCommand(studentId1, courseId);
            var command2 = new AddRegistrationCommand(studentId2, courseId);

            // Act
            var isValid1 = command1.IsValid();
            var isValid2 = command2.IsValid();

            // Assert
            isValid1.Should().BeTrue();
            isValid2.Should().BeTrue();
            command1.CourseId.Should().Be(command2.CourseId);
            command1.StudentId.Should().NotBe(command2.StudentId);
        }

        [Fact]
        public void Validate_ShouldReturnConsistentResults_WhenCalledMultipleTimes()
        {
            // Arrange
            var command = new AddRegistrationCommand(Guid.NewGuid(), Guid.NewGuid());

            // Act
            var isValid1 = command.IsValid();
            var isValid2 = command.IsValid();
            var isValid3 = command.IsValid();

            // Assert
            isValid1.Should().BeTrue();
            isValid2.Should().BeTrue();
            isValid3.Should().BeTrue();
        }
    }
}
