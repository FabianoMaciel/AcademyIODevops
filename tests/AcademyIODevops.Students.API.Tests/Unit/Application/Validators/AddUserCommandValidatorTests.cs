using AcademyIODevops.Students.API.Application.Commands;
using FluentAssertions;

namespace AcademyIODevops.Students.API.Tests.Unit.Application.Validators
{
    public class AddUserCommandValidatorTests
    {
        [Fact]
        public void Validate_ShouldBeValid_WhenAllPropertiesAreValid()
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: "john.doe",
                isAdmin: false,
                name: "John",
                lastName: "Doe",
                dateOfBirth: new DateTime(1995, 5, 15),
                email: "john.doe@academyio.com"
            );

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
            command.ValidationResult.IsValid.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_ShouldBeInvalid_WhenUserNameIsEmptyOrNull(string? invalidUserName)
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: invalidUserName!,
                isAdmin: false,
                name: "John",
                lastName: "Doe",
                dateOfBirth: DateTime.Now.AddYears(-20),
                email: "john@test.com"
            );

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.ValidationResult.Errors
                .Should().Contain(e => e.ErrorMessage == AddUserCommandValidation.UserNameError);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_ShouldBeInvalid_WhenNameIsEmptyOrNull(string? invalidName)
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: "test.user",
                isAdmin: false,
                name: invalidName!,
                lastName: "Doe",
                dateOfBirth: DateTime.Now.AddYears(-20),
                email: "test@test.com"
            );

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.ValidationResult.Errors
                .Should().Contain(e => e.ErrorMessage == AddUserCommandValidation.NameError);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_ShouldBeInvalid_WhenLastNameIsEmptyOrNull(string? invalidLastName)
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: "test.user",
                isAdmin: false,
                name: "Test",
                lastName: invalidLastName!,
                dateOfBirth: DateTime.Now.AddYears(-20),
                email: "test@test.com"
            );

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.ValidationResult.Errors
                .Should().Contain(e => e.ErrorMessage == AddUserCommandValidation.LastNameError);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Validate_ShouldBeInvalid_WhenEmailIsEmptyOrNull(string? invalidEmail)
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: "test.user",
                isAdmin: false,
                name: "Test",
                lastName: "User",
                dateOfBirth: DateTime.Now.AddYears(-20),
                email: invalidEmail!
            );

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.ValidationResult.Errors
                .Should().Contain(e => e.ErrorMessage == AddUserCommandValidation.EmailError);
        }

        [Fact]
        public void Validate_ShouldHaveMultipleErrors_WhenMultiplePropertiesAreInvalid()
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: string.Empty,  // Inválido
                isAdmin: false,
                name: string.Empty,      // Inválido
                lastName: string.Empty,  // Inválido
                dateOfBirth: DateTime.Now,
                email: string.Empty      // Inválido
            );

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.ValidationResult.Errors.Should().HaveCount(4);
            command.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == AddUserCommandValidation.UserNameError);
            command.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == AddUserCommandValidation.NameError);
            command.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == AddUserCommandValidation.LastNameError);
            command.ValidationResult.Errors.Should().Contain(e => e.ErrorMessage == AddUserCommandValidation.EmailError);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Validate_ShouldBeValid_WithDifferentIsAdminValues(bool isAdmin)
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: "test.user",
                isAdmin: isAdmin,
                name: "Test",
                lastName: "User",
                dateOfBirth: DateTime.Now.AddYears(-20),
                email: "test@test.com"
            );

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldBeValid_WithValidDateOfBirth()
        {
            // Arrange
            var command = new AddUserCommand(
                userId: Guid.NewGuid(),
                userName: "test.user",
                isAdmin: false,
                name: "Test",
                lastName: "User",
                dateOfBirth: new DateTime(1990, 1, 1),
                email: "test@test.com"
            );

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldBeValid_WithValidUserId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new AddUserCommand(
                userId: userId,
                userName: "test.user",
                isAdmin: false,
                name: "Test",
                lastName: "User",
                dateOfBirth: DateTime.Now.AddYears(-20),
                email: "test@test.com"
            );

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
            command.UserId.Should().Be(userId);
        }

        [Fact]
        public void Validate_ShouldPreserveAllProperties_WhenValidationFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userName = string.Empty; // Inválido
            var name = "Test";
            var lastName = "User";
            var email = "test@test.com";
            var dateOfBirth = DateTime.Now.AddYears(-20);

            var command = new AddUserCommand(
                userId: userId,
                userName: userName,
                isAdmin: false,
                name: name,
                lastName: lastName,
                dateOfBirth: dateOfBirth,
                email: email
            );

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.UserId.Should().Be(userId);
            command.UserName.Should().Be(userName);
            command.Name.Should().Be(name);
            command.LastName.Should().Be(lastName);
            command.Email.Should().Be(email);
            command.DateOfBirth.Should().Be(dateOfBirth);
        }
    }
}
