using Bogus;
using static AcademyIODevops.Auth.API.Models.UserViewModel;

namespace AcademyIODevops.Auth.API.Tests.Builders;

public class RegisterUserViewModelBuilder
{
    private string _email;
    private string _firstName;
    private string _lastName;
    private DateTime _dateOfBirth;
    private string _password;
    private string _confirmPassword;
    private bool _isAdmin;

    public RegisterUserViewModelBuilder()
    {
        // Default values using Bogus
        var faker = new Faker();
        _email = faker.Internet.Email();
        _firstName = faker.Name.FirstName();
        _lastName = faker.Name.LastName();
        _dateOfBirth = faker.Date.Past(30, DateTime.Now.AddYears(-18));
        _password = "Test@123456";
        _confirmPassword = "Test@123456";
        _isAdmin = false;
    }

    public RegisterUserViewModelBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public RegisterUserViewModelBuilder WithFirstName(string firstName)
    {
        _firstName = firstName;
        return this;
    }

    public RegisterUserViewModelBuilder WithLastName(string lastName)
    {
        _lastName = lastName;
        return this;
    }

    public RegisterUserViewModelBuilder WithDateOfBirth(DateTime dateOfBirth)
    {
        _dateOfBirth = dateOfBirth;
        return this;
    }

    public RegisterUserViewModelBuilder WithPassword(string password)
    {
        _password = password;
        return this;
    }

    public RegisterUserViewModelBuilder WithConfirmPassword(string confirmPassword)
    {
        _confirmPassword = confirmPassword;
        return this;
    }

    public RegisterUserViewModelBuilder WithMatchingPasswords(string password)
    {
        _password = password;
        _confirmPassword = password;
        return this;
    }

    public RegisterUserViewModelBuilder WithMismatchedPasswords()
    {
        _password = "Password123!";
        _confirmPassword = "DifferentPassword456!";
        return this;
    }

    public RegisterUserViewModelBuilder AsAdmin()
    {
        _isAdmin = true;
        return this;
    }

    public RegisterUserViewModelBuilder AsStudent()
    {
        _isAdmin = false;
        return this;
    }

    public RegisterUserViewModelBuilder AsJohnDoe()
    {
        _email = "john.doe@example.com";
        _firstName = "John";
        _lastName = "Doe";
        _dateOfBirth = new DateTime(1990, 1, 1);
        _password = "JohnDoe@123";
        _confirmPassword = "JohnDoe@123";
        _isAdmin = false;
        return this;
    }

    public RegisterUserViewModelBuilder AsAdminUser()
    {
        _email = "admin@example.com";
        _firstName = "Admin";
        _lastName = "User";
        _dateOfBirth = new DateTime(1985, 5, 15);
        _password = "Admin@123456";
        _confirmPassword = "Admin@123456";
        _isAdmin = true;
        return this;
    }

    public RegisterUserViewModel Build()
    {
        return new RegisterUserViewModel
        {
            Email = _email,
            FirstName = _firstName,
            LastName = _lastName,
            DateOfBirth = _dateOfBirth,
            Password = _password,
            ConfirmPassword = _confirmPassword,
            IsAdmin = _isAdmin
        };
    }

    public static RegisterUserViewModelBuilder Create() => new RegisterUserViewModelBuilder();
}
