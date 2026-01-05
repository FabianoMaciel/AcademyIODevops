using Bogus;
using static AcademyIODevops.Auth.API.Models.UserViewModel;

namespace AcademyIODevops.Auth.API.Tests.Builders;

public class LoginUserViewModelBuilder
{
    private string _email;
    private string _password;

    public LoginUserViewModelBuilder()
    {
        // Default values using Bogus
        var faker = new Faker();
        _email = faker.Internet.Email();
        _password = "Test@123456";
    }

    public LoginUserViewModelBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public LoginUserViewModelBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public LoginUserViewModelBuilder AsJohnDoe()
    {
        _email = "john.doe@example.com";
        _password = "JohnDoe@123";
        return this;
    }

    public LoginUserViewModelBuilder AsAdminUser()
    {
        _email = "admin@example.com";
        _password = "Admin@123456";
        return this;
    }

    public LoginUserViewModelBuilder WithInvalidCredentials()
    {
        _email = "invalid@example.com";
        _password = "WrongPassword123!";
        return this;
    }

    public LoginUserViewModel Build()
    {
        return new LoginUserViewModel
        {
            Email = _email,
            Password = _password
        };
    }

    public static LoginUserViewModelBuilder Create() => new LoginUserViewModelBuilder();
}
