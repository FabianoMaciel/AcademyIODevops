using AcademyIODevops.Core.DomainObjects;
using FluentAssertions;
using Xunit;

namespace AcademyIODevops.Core.Tests.DomainObjects
{
    public class UserTests
    {
        [Fact]
        public void User_ShouldCreateWithParameterlessConstructor()
        {
            // Act
            var user = new User();

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void User_ShouldCreateWithParameterizedConstructor()
        {
            // Arrange
            var id = Guid.NewGuid();
            var firstName = "John";
            var lastName = "Doe";
            var email = "john.doe@example.com";
            var birthdate = new DateTime(1990, 1, 1);

            // Act
            var user = new User(id, firstName, lastName, email, birthdate);

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().Be(id);
            user.FistName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
            user.Email.Should().Be(email);
            user.Birthdate.Should().Be(birthdate);
        }

        [Fact]
        public void Create_ShouldCreateUserWithCorrectProperties()
        {
            // Arrange
            var id = Guid.NewGuid();
            var firstName = "Jane";
            var lastName = "Smith";
            var email = "jane.smith@example.com";
            var birthdate = new DateTime(1985, 5, 15);

            // Act
            var user = User.Create(id, email, firstName, lastName, birthdate);

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().Be(id);
            user.FistName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
            user.Email.Should().Be(email);
            user.Birthdate.Should().Be(birthdate);
        }

        [Fact]
        public void User_ShouldImplementIAggregateRoot()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            user.Should().BeAssignableTo<IAggregateRoot>();
        }

        [Fact]
        public void User_ShouldInheritFromEntity()
        {
            // Arrange & Act
            var user = new User();

            // Assert
            user.Should().BeAssignableTo<Entity>();
        }

        [Theory]
        [InlineData("john@example.com")]
        [InlineData("user.name@domain.co.uk")]
        [InlineData("test+tag@example.com")]
        [InlineData("firstname.lastname@company.org")]
        public void User_ShouldAcceptVariousEmailFormats(string email)
        {
            // Arrange
            var id = Guid.NewGuid();
            var firstName = "Test";
            var lastName = "User";
            var birthdate = new DateTime(1990, 1, 1);

            // Act
            var user = new User(id, firstName, lastName, email, birthdate);

            // Assert
            user.Email.Should().Be(email);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("Jo")]
        [InlineData("John")]
        [InlineData("VeryLongFirstNameThatIsStillValid")]
        public void User_ShouldAcceptVariousFirstNameLengths(string firstName)
        {
            // Arrange
            var id = Guid.NewGuid();
            var lastName = "Doe";
            var email = "test@example.com";
            var birthdate = new DateTime(1990, 1, 1);

            // Act
            var user = new User(id, firstName, lastName, email, birthdate);

            // Assert
            user.FistName.Should().Be(firstName);
        }

        [Theory]
        [InlineData(1950, 1, 1)]
        [InlineData(1990, 6, 15)]
        [InlineData(2000, 12, 31)]
        [InlineData(2010, 2, 28)]
        public void User_ShouldAcceptVariousBirthdates(int year, int month, int day)
        {
            // Arrange
            var id = Guid.NewGuid();
            var firstName = "John";
            var lastName = "Doe";
            var email = "john@example.com";
            var birthdate = new DateTime(year, month, day);

            // Act
            var user = new User(id, firstName, lastName, email, birthdate);

            // Assert
            user.Birthdate.Should().Be(birthdate);
        }

        [Fact]
        public void User_ShouldPreserveIdThroughEntityBaseClass()
        {
            // Arrange
            var id = Guid.NewGuid();
            var firstName = "John";
            var lastName = "Doe";
            var email = "john@example.com";
            var birthdate = new DateTime(1990, 1, 1);

            // Act
            var user = new User(id, firstName, lastName, email, birthdate);

            // Assert
            user.Id.Should().Be(id);
            (user as Entity).Id.Should().Be(id);
        }

        [Fact]
        public void Create_ShouldReturnSamePropertiesAsConstructor()
        {
            // Arrange
            var id = Guid.NewGuid();
            var firstName = "John";
            var lastName = "Doe";
            var email = "john@example.com";
            var birthdate = new DateTime(1990, 1, 1);

            // Act
            var userFromCreate = User.Create(id, email, firstName, lastName, birthdate);
            var userFromConstructor = new User(id, firstName, lastName, email, birthdate);

            // Assert
            userFromCreate.Id.Should().Be(userFromConstructor.Id);
            userFromCreate.FistName.Should().Be(userFromConstructor.FistName);
            userFromCreate.LastName.Should().Be(userFromConstructor.LastName);
            userFromCreate.Email.Should().Be(userFromConstructor.Email);
            userFromCreate.Birthdate.Should().Be(userFromConstructor.Birthdate);
        }

        [Fact]
        public void User_ShouldAllowEmptyStrings_ForNames()
        {
            // Arrange
            var id = Guid.NewGuid();
            var firstName = "";
            var lastName = "";
            var email = "test@example.com";
            var birthdate = new DateTime(1990, 1, 1);

            // Act
            var user = new User(id, firstName, lastName, email, birthdate);

            // Assert
            user.FistName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
        }

        [Fact]
        public void User_ShouldAllowEmptyString_ForEmail()
        {
            // Arrange
            var id = Guid.NewGuid();
            var firstName = "John";
            var lastName = "Doe";
            var email = "";
            var birthdate = new DateTime(1990, 1, 1);

            // Act
            var user = new User(id, firstName, lastName, email, birthdate);

            // Assert
            user.Email.Should().Be(email);
        }

        [Fact]
        public void User_ShouldBeEqual_WhenSameId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user1 = new User(id, "John", "Doe", "john@example.com", new DateTime(1990, 1, 1));
            var user2 = new User(id, "Jane", "Smith", "jane@example.com", new DateTime(1985, 5, 15));

            // Act
            var areEqual = user1.Equals(user2);

            // Assert
            areEqual.Should().BeTrue("users with the same Id should be considered equal");
        }

        [Fact]
        public void User_ShouldNotBeEqual_WhenDifferentIds()
        {
            // Arrange
            var user1 = new User(Guid.NewGuid(), "John", "Doe", "john@example.com", new DateTime(1990, 1, 1));
            var user2 = new User(Guid.NewGuid(), "John", "Doe", "john@example.com", new DateTime(1990, 1, 1));

            // Act
            var areEqual = user1.Equals(user2);

            // Assert
            areEqual.Should().BeFalse("users with different Ids should not be considered equal");
        }

        [Fact]
        public void User_ShouldHavePrivateSetters_ForAllProperties()
        {
            // Arrange
            var userType = typeof(User);

            // Act
            var fistNameProperty = userType.GetProperty(nameof(User.FistName));
            var lastNameProperty = userType.GetProperty(nameof(User.LastName));
            var emailProperty = userType.GetProperty(nameof(User.Email));
            var birthdateProperty = userType.GetProperty(nameof(User.Birthdate));

            // Assert
            fistNameProperty.SetMethod.Should().NotBeNull();
            fistNameProperty.SetMethod.IsPrivate.Should().BeTrue();

            lastNameProperty.SetMethod.Should().NotBeNull();
            lastNameProperty.SetMethod.IsPrivate.Should().BeTrue();

            emailProperty.SetMethod.Should().NotBeNull();
            emailProperty.SetMethod.IsPrivate.Should().BeTrue();

            birthdateProperty.SetMethod.Should().NotBeNull();
            birthdateProperty.SetMethod.IsPrivate.Should().BeTrue();
        }

        [Fact]
        public void Create_ShouldAcceptAllParametersInCorrectOrder()
        {
            // Arrange
            var id = Guid.NewGuid();
            var email = "john@example.com";
            var firstName = "John";
            var lastName = "Doe";
            var birthdate = new DateTime(1990, 1, 1);

            // Act
            var user = User.Create(id, email, firstName, lastName, birthdate);

            // Assert
            user.Id.Should().Be(id);
            user.Email.Should().Be(email);
            user.FistName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
            user.Birthdate.Should().Be(birthdate);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        public void User_ShouldAcceptWhitespaceStrings_ForNames(string name)
        {
            // Arrange
            var id = Guid.NewGuid();
            var email = "test@example.com";
            var birthdate = new DateTime(1990, 1, 1);

            // Act
            var user = new User(id, name, name, email, birthdate);

            // Assert
            user.FistName.Should().Be(name);
            user.LastName.Should().Be(name);
        }

        [Fact]
        public void User_ShouldGenerateUniqueIds_WhenUsingParameterlessConstructor()
        {
            // Act
            var user1 = new User();
            var user2 = new User();
            var user3 = new User();

            // Assert
            user1.Id.Should().NotBe(user2.Id);
            user1.Id.Should().NotBe(user3.Id);
            user2.Id.Should().NotBe(user3.Id);
        }

        [Fact]
        public void User_ShouldInheritEntityBehavior_ForNotifications()
        {
            // Arrange
            var user = new User();

            // Assert
            user.Notifications.Should().BeNull("no events have been added yet");
        }
    }
}
