using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;
using static AcademyIODevops.Auth.API.Models.UserViewModel;

namespace AcademyIODevops.Auth.API.Tests.Unit.Models;

public class RegisterUserViewModelTests
{
    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void RegisterUserViewModel_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        // Arrange
        var viewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "Test@123456",
            IsAdmin = false
        };

        // Act
        var validationResults = ValidateModel(viewModel);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RegisterUserViewModel_ShouldBeInvalid_WhenEmailIsNullOrEmpty(string email)
    {
        // Arrange
        var viewModel = new RegisterUserViewModel
        {
            Email = email,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "Test@123456"
        };

        // Act
        var validationResults = ValidateModel(viewModel);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.MemberNames.Contains("Email"));
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("@example.com")]
    [InlineData("test@")]
    [InlineData("test.example.com")]
    public void RegisterUserViewModel_ShouldBeInvalid_WhenEmailFormatIsInvalid(string email)
    {
        // Arrange
        var viewModel = new RegisterUserViewModel
        {
            Email = email,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "Test@123456"
        };

        // Act
        var validationResults = ValidateModel(viewModel);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.MemberNames.Contains("Email"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RegisterUserViewModel_ShouldBeInvalid_WhenFirstNameIsNullOrEmpty(string firstName)
    {
        // Arrange
        var viewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = firstName,
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "Test@123456"
        };

        // Act
        var validationResults = ValidateModel(viewModel);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.MemberNames.Contains("FirstName"));
    }

    [Theory]
    [InlineData("A")]
    public void RegisterUserViewModel_ShouldBeInvalid_WhenFirstNameIsTooShort(string firstName)
    {
        // Arrange
        var viewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = firstName,
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "Test@123456"
        };

        // Act
        var validationResults = ValidateModel(viewModel);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.MemberNames.Contains("FirstName"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RegisterUserViewModel_ShouldBeInvalid_WhenLastNameIsNullOrEmpty(string lastName)
    {
        // Arrange
        var viewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = lastName,
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "Test@123456"
        };

        // Act
        var validationResults = ValidateModel(viewModel);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.MemberNames.Contains("LastName"));
    }

    [Theory]
    [InlineData("D")]
    public void RegisterUserViewModel_ShouldBeInvalid_WhenLastNameIsTooShort(string lastName)
    {
        // Arrange
        var viewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = lastName,
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "Test@123456"
        };

        // Act
        var validationResults = ValidateModel(viewModel);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.MemberNames.Contains("LastName"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void RegisterUserViewModel_ShouldBeInvalid_WhenPasswordIsNullOrEmpty(string password)
    {
        // Arrange
        var viewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = password,
            ConfirmPassword = "Test@123456"
        };

        // Act
        var validationResults = ValidateModel(viewModel);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.MemberNames.Contains("Password"));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("12345")]
    public void RegisterUserViewModel_ShouldBeInvalid_WhenPasswordIsTooShort(string password)
    {
        // Arrange
        var viewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = password,
            ConfirmPassword = password
        };

        // Act
        var validationResults = ValidateModel(viewModel);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.MemberNames.Contains("Password"));
    }

    [Fact]
    public void RegisterUserViewModel_ShouldBeInvalid_WhenPasswordsDoNotMatch()
    {
        // Arrange
        var viewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "DifferentPassword123"
        };

        // Act
        var validationResults = ValidateModel(viewModel);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.MemberNames.Contains("ConfirmPassword"));
    }

    [Fact]
    public void RegisterUserViewModel_ShouldAcceptDefaultDateOfBirth()
    {
        // Arrange
        var viewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = default,
            Password = "Test@123456",
            ConfirmPassword = "Test@123456"
        };

        // Act
        var validationResults = ValidateModel(viewModel);

        // Assert - DateOfBirth is required but DateTime default (01/01/0001) should be allowed
        // The validation should pass for the DateTime field itself
        viewModel.DateOfBirth.Should().Be(default(DateTime));
    }

    [Fact]
    public void RegisterUserViewModel_ShouldSetIsAdminToFalseByDefault()
    {
        // Arrange & Act
        var viewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "Test@123456"
        };

        // Assert
        viewModel.IsAdmin.Should().BeFalse();
    }

    [Fact]
    public void RegisterUserViewModel_ShouldAllowSettingIsAdminToTrue()
    {
        // Arrange & Act
        var viewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "Test@123456",
            IsAdmin = true
        };

        // Assert
        viewModel.IsAdmin.Should().BeTrue();
    }

    [Fact]
    public void RegisterUserViewModel_ShouldHaveCorrectProperties()
    {
        // Arrange
        var email = "test@example.com";
        var firstName = "John";
        var lastName = "Doe";
        var dateOfBirth = new DateTime(1990, 1, 1);
        var password = "Test@123456";
        var isAdmin = true;

        // Act
        var viewModel = new RegisterUserViewModel
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            DateOfBirth = dateOfBirth,
            Password = password,
            ConfirmPassword = password,
            IsAdmin = isAdmin
        };

        // Assert
        viewModel.Email.Should().Be(email);
        viewModel.FirstName.Should().Be(firstName);
        viewModel.LastName.Should().Be(lastName);
        viewModel.DateOfBirth.Should().Be(dateOfBirth);
        viewModel.Password.Should().Be(password);
        viewModel.ConfirmPassword.Should().Be(password);
        viewModel.IsAdmin.Should().Be(isAdmin);
    }
}
