using AcademyIODevops.MessageBus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace AcademyIODevops.Auth.API.Tests.Fixtures;

public class AuthControllerTestFixture : IDisposable
{
    public Mock<UserManager<IdentityUser<Guid>>> UserManagerMock { get; }
    public Mock<SignInManager<IdentityUser<Guid>>> SignInManagerMock { get; }
    public Mock<IMessageBus> MessageBusMock { get; }

    public AuthControllerTestFixture()
    {
        // Mock UserManager
        var userStoreMock = new Mock<IUserStore<IdentityUser<Guid>>>();
        UserManagerMock = new Mock<UserManager<IdentityUser<Guid>>>(
            userStoreMock.Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<IdentityUser<Guid>>>().Object,
            new IUserValidator<IdentityUser<Guid>>[0],
            new IPasswordValidator<IdentityUser<Guid>>[0],
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<IdentityUser<Guid>>>>().Object);

        // Mock SignInManager
        var contextAccessorMock = new Mock<IHttpContextAccessor>();
        var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<IdentityUser<Guid>>>();

        SignInManagerMock = new Mock<SignInManager<IdentityUser<Guid>>>(
            UserManagerMock.Object,
            contextAccessorMock.Object,
            userPrincipalFactoryMock.Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<ILogger<SignInManager<IdentityUser<Guid>>>>().Object,
            new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>().Object,
            new Mock<IUserConfirmation<IdentityUser<Guid>>>().Object);

        // Mock MessageBus
        MessageBusMock = new Mock<IMessageBus>();
    }

    public void Dispose()
    {
        // Cleanup if needed
        GC.SuppressFinalize(this);
    }
}
