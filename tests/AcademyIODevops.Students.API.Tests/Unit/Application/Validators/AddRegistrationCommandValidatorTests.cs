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

        [Theory]
        [InlineData("a1b2c3d4-e5f6-4a5b-8c7d-9e0f1a2b3c4d", "b2c3d4e5-f6a7-4b5c-8d7e-9f0a1b2c3d4e")]
        [InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479", "6ba7b811-9dad-11d1-80b4-00c04fd430c8")]
        [InlineData("550e8400-e29b-41d4-a716-446655440000", "6ba7b810-9dad-11d1-80b4-00c04fd430c8")]
        public void Validate_ShouldBeValid_WithSpecificGuidPairs(string studentIdStr, string courseIdStr)
        {
            // Arrange
            var studentId = Guid.Parse(studentIdStr);
            var courseId = Guid.Parse(courseIdStr);
            var command = new AddRegistrationCommand(studentId, courseId);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
            command.StudentId.Should().Be(studentId);
            command.CourseId.Should().Be(courseId);
        }

        [Fact]
        public void Validate_ShouldHaveCorrectErrorMessage_WhenStudentIdIsEmpty()
        {
            // Arrange
            var command = new AddRegistrationCommand(Guid.Empty, Guid.NewGuid());

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            var error = command.ValidationResult.Errors.First(e =>
                e.ErrorMessage == AddRegistrationCommandValidation.StudentIdError);
            error.Should().NotBeNull();
            error.PropertyName.Should().Be("StudentId");
        }

        [Fact]
        public void Validate_ShouldHaveCorrectErrorMessage_WhenCourseIdIsEmpty()
        {
            // Arrange
            var command = new AddRegistrationCommand(Guid.NewGuid(), Guid.Empty);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            var error = command.ValidationResult.Errors.First(e =>
                e.ErrorMessage == AddRegistrationCommandValidation.CourseIdError);
            error.Should().NotBeNull();
            error.PropertyName.Should().Be("CourseId");
        }

        [Fact]
        public void Validate_ShouldFailValidation_WhenBothIdsAreInvalid()
        {
            // Arrange
            var command = new AddRegistrationCommand(Guid.Empty, Guid.Empty);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.ValidationResult.IsValid.Should().BeFalse();
            command.ValidationResult.Errors.Should().HaveCount(2);
        }

        [Fact]
        public void Validate_ShouldSucceed_WhenAllIdsAreValid()
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
        public void Validate_ShouldAllowSameStudentInMultipleCourses()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var commands = new[]
            {
                new AddRegistrationCommand(studentId, Guid.NewGuid()),
                new AddRegistrationCommand(studentId, Guid.NewGuid()),
                new AddRegistrationCommand(studentId, Guid.NewGuid())
            };

            // Act & Assert
            foreach (var command in commands)
            {
                var isValid = command.IsValid();
                isValid.Should().BeTrue();
                command.StudentId.Should().Be(studentId);
            }
        }

        [Fact]
        public void Validate_ShouldAllowMultipleStudentsInSameCourse()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var commands = new[]
            {
                new AddRegistrationCommand(Guid.NewGuid(), courseId),
                new AddRegistrationCommand(Guid.NewGuid(), courseId),
                new AddRegistrationCommand(Guid.NewGuid(), courseId)
            };

            // Act & Assert
            foreach (var command in commands)
            {
                var isValid = command.IsValid();
                isValid.Should().BeTrue();
                command.CourseId.Should().Be(courseId);
            }
        }

        [Fact]
        public void Validate_ShouldNotAllowEmptyStudentId_EvenWithValidCourseId()
        {
            // Arrange
            var validCourseId = Guid.NewGuid();
            var command = new AddRegistrationCommand(Guid.Empty, validCourseId);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.CourseId.Should().Be(validCourseId);
            command.ValidationResult.Errors.Should().ContainSingle();
        }

        [Fact]
        public void Validate_ShouldNotAllowEmptyCourseId_EvenWithValidStudentId()
        {
            // Arrange
            var validStudentId = Guid.NewGuid();
            var command = new AddRegistrationCommand(validStudentId, Guid.Empty);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.StudentId.Should().Be(validStudentId);
            command.ValidationResult.Errors.Should().ContainSingle();
        }

        [Fact]
        public void Validate_ShouldMaintainValidationState_AfterMultipleValidations()
        {
            // Arrange
            var command = new AddRegistrationCommand(Guid.Empty, Guid.Empty);

            // Act
            var isValid1 = command.IsValid();
            var errorCount1 = command.ValidationResult.Errors.Count;

            var isValid2 = command.IsValid();
            var errorCount2 = command.ValidationResult.Errors.Count;

            // Assert
            isValid1.Should().BeFalse();
            isValid2.Should().BeFalse();
            errorCount1.Should().Be(2);
            errorCount2.Should().Be(2);
        }

        [Fact]
        public void Validate_ShouldAcceptAnyValidGuid_ForStudentId()
        {
            // Arrange
            var studentIds = new[]
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };
            var courseId = Guid.NewGuid();

            // Act & Assert
            foreach (var studentId in studentIds)
            {
                var command = new AddRegistrationCommand(studentId, courseId);
                var isValid = command.IsValid();
                isValid.Should().BeTrue();
            }
        }

        [Fact]
        public void Validate_ShouldAcceptAnyValidGuid_ForCourseId()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseIds = new[]
            {
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            // Act & Assert
            foreach (var courseId in courseIds)
            {
                var command = new AddRegistrationCommand(studentId, courseId);
                var isValid = command.IsValid();
                isValid.Should().BeTrue();
            }
        }
    }
}
