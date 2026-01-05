using AcademyIODevops.Auth.API.Controllers;
using AcademyIODevops.Auth.API.Tests.Fixtures;
using AcademyIODevops.Core.Messages.Integration;
using AcademyIODevops.WebAPI.Core.Identity;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using static AcademyIODevops.Auth.API.Models.UserViewModel;

namespace AcademyIODevops.Auth.API.Tests.Unit.Controllers;

public class AuthControllerTests : IClassFixture<AuthControllerTestFixture>
{
    private readonly AuthControllerTestFixture _fixture;
    private readonly JwtSettings _jwtSettings;

    public AuthControllerTests(AuthControllerTestFixture fixture)
    {
        _fixture = fixture;
        _jwtSettings = new JwtSettings
        {
            SecretKey = "ThisIsAVerySecretKeyThatIsLongEnoughForHS256Algorithm",
            ExpirationHours = 2,
            Issuer = "AcademyIODevops",
            Audience = "https://localhost"
        };
    }

    #region Register Tests

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var controller = new AuthController(_jwtSettings, _fixture.SignInManagerMock.Object, _fixture.UserManagerMock.Object, _fixture.MessageBusMock.Object);
        controller.ModelState.AddModelError("Email", "Email is required");

        var registerUserViewModel = new RegisterUserViewModel
        {
            Email = "",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "Test@123456"
        };

        // Act
        var result = await controller.Register(registerUserViewModel);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_ShouldReturnBadRequest_WhenUserCreationFails()
    {
        // Arrange
        var controller = new AuthController(_jwtSettings, _fixture.SignInManagerMock.Object, _fixture.UserManagerMock.Object, _fixture.MessageBusMock.Object);

        var registerUserViewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "Test@123456",
            IsAdmin = false
        };

        var identityErrors = new[]
        {
            new IdentityError { Code = "DuplicateUserName", Description = "Username already exists" }
        };

        _fixture.UserManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<IdentityUser<Guid>>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(identityErrors));

        // Act
        var result = await controller.Register(registerUserViewModel);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Register_ShouldCallAddToRoleAsync_WhenUserIsNotAdmin()
    {
        // Arrange
        var controller = new AuthController(_jwtSettings, _fixture.SignInManagerMock.Object, _fixture.UserManagerMock.Object, _fixture.MessageBusMock.Object);

        var registerUserViewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "Test@123456",
            IsAdmin = false
        };

        var user = new IdentityUser<Guid>
        {
            Id = Guid.NewGuid(),
            UserName = registerUserViewModel.Email,
            Email = registerUserViewModel.Email,
            EmailConfirmed = true
        };

        _fixture.UserManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<IdentityUser<Guid>>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _fixture.UserManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser<Guid>>(), "STUDENT"))
            .ReturnsAsync(IdentityResult.Success);

        _fixture.UserManagerMock
            .Setup(x => x.FindByEmailAsync(registerUserViewModel.Email))
            .ReturnsAsync(user);

        _fixture.UserManagerMock
            .Setup(x => x.GetClaimsAsync(It.IsAny<IdentityUser<Guid>>()))
            .ReturnsAsync(new List<System.Security.Claims.Claim>());

        _fixture.UserManagerMock
            .Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser<Guid>>()))
            .ReturnsAsync(new List<string> { "STUDENT" });

        var responseMessage = new ResponseMessage(new ValidationResult());
        _fixture.MessageBusMock
            .Setup(x => x.RequestAsync<UserRegisteredIntegrationEvent, ResponseMessage>(It.IsAny<UserRegisteredIntegrationEvent>()))
            .ReturnsAsync(responseMessage);

        // Act
        var result = await controller.Register(registerUserViewModel);

        // Assert
        _fixture.UserManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<IdentityUser<Guid>>(), "STUDENT"), Times.Once);
    }

    [Fact]
    public async Task Register_ShouldCallAddToRoleAsync_WhenUserIsAdmin()
    {
        // Arrange
        var controller = new AuthController(_jwtSettings, _fixture.SignInManagerMock.Object, _fixture.UserManagerMock.Object, _fixture.MessageBusMock.Object);

        var registerUserViewModel = new RegisterUserViewModel
        {
            Email = "admin@example.com",
            FirstName = "Admin",
            LastName = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Admin@123456",
            ConfirmPassword = "Admin@123456",
            IsAdmin = true
        };

        var user = new IdentityUser<Guid>
        {
            Id = Guid.NewGuid(),
            UserName = registerUserViewModel.Email,
            Email = registerUserViewModel.Email,
            EmailConfirmed = true
        };

        _fixture.UserManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<IdentityUser<Guid>>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _fixture.UserManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser<Guid>>(), "ADMIN"))
            .ReturnsAsync(IdentityResult.Success);

        _fixture.UserManagerMock
            .Setup(x => x.FindByEmailAsync(registerUserViewModel.Email))
            .ReturnsAsync(user);

        _fixture.UserManagerMock
            .Setup(x => x.GetClaimsAsync(It.IsAny<IdentityUser<Guid>>()))
            .ReturnsAsync(new List<System.Security.Claims.Claim>());

        _fixture.UserManagerMock
            .Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser<Guid>>()))
            .ReturnsAsync(new List<string> { "ADMIN" });

        var responseMessage = new ResponseMessage(new ValidationResult());
        _fixture.MessageBusMock
            .Setup(x => x.RequestAsync<UserRegisteredIntegrationEvent, ResponseMessage>(It.IsAny<UserRegisteredIntegrationEvent>()))
            .ReturnsAsync(responseMessage);

        // Act
        var result = await controller.Register(registerUserViewModel);

        // Assert
        _fixture.UserManagerMock.Verify(x => x.AddToRoleAsync(It.IsAny<IdentityUser<Guid>>(), "ADMIN"), Times.Once);
    }

    [Fact]
    public async Task Register_ShouldPublishUserRegisteredIntegrationEvent_WhenUserIsCreatedSuccessfully()
    {
        // Arrange
        var controller = new AuthController(_jwtSettings, _fixture.SignInManagerMock.Object, _fixture.UserManagerMock.Object, _fixture.MessageBusMock.Object);

        var registerUserViewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "Test@123456",
            IsAdmin = false
        };

        var userId = Guid.NewGuid();
        var user = new IdentityUser<Guid>
        {
            Id = userId,
            UserName = registerUserViewModel.Email,
            Email = registerUserViewModel.Email,
            EmailConfirmed = true
        };

        _fixture.UserManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<IdentityUser<Guid>>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _fixture.UserManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser<Guid>>(), "STUDENT"))
            .ReturnsAsync(IdentityResult.Success);

        _fixture.UserManagerMock
            .Setup(x => x.FindByEmailAsync(registerUserViewModel.Email))
            .ReturnsAsync(user);

        _fixture.UserManagerMock
            .Setup(x => x.GetClaimsAsync(It.IsAny<IdentityUser<Guid>>()))
            .ReturnsAsync(new List<System.Security.Claims.Claim>());

        _fixture.UserManagerMock
            .Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser<Guid>>()))
            .ReturnsAsync(new List<string> { "STUDENT" });

        var responseMessage = new ResponseMessage(new ValidationResult());
        _fixture.MessageBusMock
            .Setup(x => x.RequestAsync<UserRegisteredIntegrationEvent, ResponseMessage>(It.IsAny<UserRegisteredIntegrationEvent>()))
            .ReturnsAsync(responseMessage);

        // Act
        var result = await controller.Register(registerUserViewModel);

        // Assert
        _fixture.MessageBusMock.Verify(
            x => x.RequestAsync<UserRegisteredIntegrationEvent, ResponseMessage>(
                It.Is<UserRegisteredIntegrationEvent>(e =>
                    e.Id == userId &&
                    e.FirstName == registerUserViewModel.FirstName &&
                    e.LastName == registerUserViewModel.LastName &&
                    e.UserName == registerUserViewModel.Email)),
            Times.Once);
    }

    [Fact]
    public async Task Register_ShouldDeleteUser_WhenIntegrationEventFails()
    {
        // Arrange
        var controller = new AuthController(_jwtSettings, _fixture.SignInManagerMock.Object, _fixture.UserManagerMock.Object, _fixture.MessageBusMock.Object);

        var registerUserViewModel = new RegisterUserViewModel
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "Test@123456",
            ConfirmPassword = "Test@123456",
            IsAdmin = false
        };

        var user = new IdentityUser<Guid>
        {
            Id = Guid.NewGuid(),
            UserName = registerUserViewModel.Email,
            Email = registerUserViewModel.Email,
            EmailConfirmed = true
        };

        _fixture.UserManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<IdentityUser<Guid>>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _fixture.UserManagerMock
            .Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser<Guid>>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Success);

        _fixture.UserManagerMock
            .Setup(x => x.FindByEmailAsync(registerUserViewModel.Email))
            .ReturnsAsync(user);

        _fixture.UserManagerMock
            .Setup(x => x.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        _fixture.MessageBusMock
            .Setup(x => x.RequestAsync<UserRegisteredIntegrationEvent, ResponseMessage>(It.IsAny<UserRegisteredIntegrationEvent>()))
            .ThrowsAsync(new Exception("Message bus error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => controller.Register(registerUserViewModel));
        _fixture.UserManagerMock.Verify(x => x.DeleteAsync(user), Times.Once);
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenModelStateIsInvalid()
    {
        // Arrange
        var controller = new AuthController(_jwtSettings, _fixture.SignInManagerMock.Object, _fixture.UserManagerMock.Object, _fixture.MessageBusMock.Object);
        controller.ModelState.AddModelError("Email", "Email is required");

        var loginUserViewModel = new LoginUserViewModel
        {
            Email = "",
            Password = "Test@123456"
        };

        // Act
        var result = await controller.Login(loginUserViewModel);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_ShouldReturnOkWithToken_WhenCredentialsAreValid()
    {
        // Arrange
        var controller = new AuthController(_jwtSettings, _fixture.SignInManagerMock.Object, _fixture.UserManagerMock.Object, _fixture.MessageBusMock.Object);

        var loginUserViewModel = new LoginUserViewModel
        {
            Email = "test@example.com",
            Password = "Test@123456"
        };

        var user = new IdentityUser<Guid>
        {
            Id = Guid.NewGuid(),
            UserName = loginUserViewModel.Email,
            Email = loginUserViewModel.Email
        };

        _fixture.SignInManagerMock
            .Setup(x => x.PasswordSignInAsync(loginUserViewModel.Email, loginUserViewModel.Password, false, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _fixture.UserManagerMock
            .Setup(x => x.FindByEmailAsync(loginUserViewModel.Email))
            .ReturnsAsync(user);

        _fixture.UserManagerMock
            .Setup(x => x.GetClaimsAsync(user))
            .ReturnsAsync(new List<System.Security.Claims.Claim>());

        _fixture.UserManagerMock
            .Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "STUDENT" });

        // Act
        var result = await controller.Login(loginUserViewModel);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenCredentialsAreInvalid()
    {
        // Arrange
        var controller = new AuthController(_jwtSettings, _fixture.SignInManagerMock.Object, _fixture.UserManagerMock.Object, _fixture.MessageBusMock.Object);

        var loginUserViewModel = new LoginUserViewModel
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        _fixture.SignInManagerMock
            .Setup(x => x.PasswordSignInAsync(loginUserViewModel.Email, loginUserViewModel.Password, false, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        var result = await controller.Login(loginUserViewModel);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequest_WhenUserIsLockedOut()
    {
        // Arrange
        var controller = new AuthController(_jwtSettings, _fixture.SignInManagerMock.Object, _fixture.UserManagerMock.Object, _fixture.MessageBusMock.Object);

        var loginUserViewModel = new LoginUserViewModel
        {
            Email = "test@example.com",
            Password = "Test@123456"
        };

        _fixture.SignInManagerMock
            .Setup(x => x.PasswordSignInAsync(loginUserViewModel.Email, loginUserViewModel.Password, false, true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

        // Act
        var result = await controller.Login(loginUserViewModel);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    #endregion
}
