using System.ComponentModel.DataAnnotations;
using FluentAssertions;
using Xunit;
using static AcademyIODevops.Auth.API.Models.UserViewModel;

namespace AcademyIODevops.Auth.API.Tests.Unit.Models;

public class LoginUserViewModelTests
{
    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }

    [Fact]
    public void LoginUserViewModel_ShouldBeValid_WhenAllPropertiesAreValid()
    {
        // Arrange
        var viewModel = new LoginUserViewModel
        {
            Email = "test@example.com",
            Password = "Test@123456"
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
    public void LoginUserViewModel_ShouldBeInvalid_WhenEmailIsNullOrEmpty(string email)
    {
        // Arrange
        var viewModel = new LoginUserViewModel
        {
            Email = email,
            Password = "Test@123456"
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
    public void LoginUserViewModel_ShouldBeInvalid_WhenEmailFormatIsInvalid(string email)
    {
        // Arrange
        var viewModel = new LoginUserViewModel
        {
            Email = email,
            Password = "Test@123456"
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
    public void LoginUserViewModel_ShouldBeInvalid_WhenPasswordIsNullOrEmpty(string password)
    {
        // Arrange
        var viewModel = new LoginUserViewModel
        {
            Email = "test@example.com",
            Password = password
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
    public void LoginUserViewModel_ShouldBeInvalid_WhenPasswordIsTooShort(string password)
    {
        // Arrange
        var viewModel = new LoginUserViewModel
        {
            Email = "test@example.com",
            Password = password
        };

        // Act
        var validationResults = ValidateModel(viewModel);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(v => v.MemberNames.Contains("Password"));
    }

    [Fact]
    public void LoginUserViewModel_ShouldAcceptValidEmail()
    {
        // Arrange
        var viewModel = new LoginUserViewModel
        {
            Email = "valid.email@example.com",
            Password = "Test@123456"
        };

        // Act
        var validationResults = ValidateModel(viewModel);

        // Assert
        validationResults.Should().BeEmpty();
        viewModel.Email.Should().Be("valid.email@example.com");
    }

    [Fact]
    public void LoginUserViewModel_ShouldAcceptValidPassword()
    {
        // Arrange
        var viewModel = new LoginUserViewModel
        {
            Email = "test@example.com",
            Password = "ValidPassword123!"
        };

        // Act
        var validationResults = ValidateModel(viewModel);

        // Assert
        validationResults.Should().BeEmpty();
        viewModel.Password.Should().Be("ValidPassword123!");
    }

    [Fact]
    public void LoginUserViewModel_ShouldHaveCorrectProperties()
    {
        // Arrange
        var email = "test@example.com";
        var password = "Test@123456";

        // Act
        var viewModel = new LoginUserViewModel
        {
            Email = email,
            Password = password
        };

        // Assert
        viewModel.Email.Should().Be(email);
        viewModel.Password.Should().Be(password);
    }
}
