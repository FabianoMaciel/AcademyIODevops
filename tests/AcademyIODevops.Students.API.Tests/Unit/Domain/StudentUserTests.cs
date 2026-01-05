using AcademyIODevops.Students.API.Models;
using AcademyIODevops.Students.API.Tests.Builders;
using FluentAssertions;

namespace AcademyIODevops.Students.API.Tests.Unit.Domain
{
    public class StudentUserTests
    {
        [Fact]
        public void StudentUser_ShouldCreateValidStudent_WhenPropertiesAreValid()
        {
            // Arrange
            var id = Guid.NewGuid();
            var userName = "john.doe";
            var firstName = "John";
            var lastName = "Doe";
            var email = "john.doe@academyio.com";
            var dateOfBirth = new DateTime(1995, 5, 15);
            var isAdmin = false;

            // Act
            var student = new StudentUser(id, userName, firstName, lastName, email, dateOfBirth, isAdmin);

            // Assert
            student.Should().NotBeNull();
            student.Id.Should().Be(id);
            student.UserName.Should().Be(userName);
            student.FirstName.Should().Be(firstName);
            student.LastName.Should().Be(lastName);
            student.Email.Should().Be(email);
            student.DateOfBirth.Should().Be(dateOfBirth);
            student.IsAdmin.Should().Be(isAdmin);
        }

        [Fact]
        public void StudentUser_ShouldGenerateUniqueIds_WhenMultipleStudentsCreated()
        {
            // Arrange & Act
            var student1 = new StudentUserBuilder()
                .WithUserName("student1")
                .Build();

            var student2 = new StudentUserBuilder()
                .WithUserName("student2")
                .Build();

            // Assert
            student1.Id.Should().NotBe(student2.Id);
            student1.Id.Should().NotBeEmpty();
            student2.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void StudentUser_ShouldImplementIAggregateRoot()
        {
            // Arrange & Act
            var student = new StudentUserBuilder().Build();

            // Assert
            student.Should().BeAssignableTo<AcademyIODevops.Core.DomainObjects.IAggregateRoot>();
        }

        [Fact]
        public void StudentUser_ShouldHaveEmptyRegistrations_WhenCreated()
        {
            // Arrange & Act
            var student = new StudentUserBuilder().Build();

            // Assert
            student.Registrations.Should().NotBeNull();
            student.Registrations.Should().BeEmpty();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void StudentUser_ShouldAcceptIsAdminFlag(bool isAdmin)
        {
            // Arrange & Act
            var student = new StudentUserBuilder()
                .WithIsAdmin(isAdmin)
                .Build();

            // Assert
            student.IsAdmin.Should().Be(isAdmin);
        }

        [Fact]
        public void StudentUser_ShouldCreateAdminUser_WhenIsAdminIsTrue()
        {
            // Arrange & Act
            var admin = new StudentUserBuilder()
                .AsAdmin()
                .Build();

            // Assert
            admin.IsAdmin.Should().BeTrue();
            admin.UserName.Should().Be("admin.user");
            admin.Email.Should().Be("admin@academyio.com");
        }

        [Fact]
        public void StudentUser_ShouldCreateRegularStudent_WhenIsAdminIsFalse()
        {
            // Arrange & Act
            var student = new StudentUserBuilder()
                .AsRegularStudent()
                .Build();

            // Assert
            student.IsAdmin.Should().BeFalse();
            student.UserName.Should().Be("student.regular");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void StudentUser_ShouldAllowEmptyUserName_ButValidationShouldCatchIt(string invalidUserName)
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var student = new StudentUser(
                id,
                invalidUserName,
                "John",
                "Doe",
                "john@test.com",
                DateTime.Now.AddYears(-20),
                false
            );

            // Assert
            // A entidade permite, mas o Command Validator deve rejeitar
            student.UserName.Should().Be(invalidUserName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void StudentUser_ShouldAllowEmptyFirstName_ButValidationShouldCatchIt(string invalidFirstName)
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var student = new StudentUser(
                id,
                "username",
                invalidFirstName,
                "Doe",
                "john@test.com",
                DateTime.Now.AddYears(-20),
                false
            );

            // Assert
            // A entidade permite, mas o Command Validator deve rejeitar
            student.FirstName.Should().Be(invalidFirstName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void StudentUser_ShouldAllowEmptyLastName_ButValidationShouldCatchIt(string invalidLastName)
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var student = new StudentUser(
                id,
                "username",
                "John",
                invalidLastName,
                "john@test.com",
                DateTime.Now.AddYears(-20),
                false
            );

            // Assert
            // A entidade permite, mas o Command Validator deve rejeitar
            student.LastName.Should().Be(invalidLastName);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void StudentUser_ShouldAllowEmptyEmail_ButValidationShouldCatchIt(string invalidEmail)
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var student = new StudentUser(
                id,
                "username",
                "John",
                "Doe",
                invalidEmail,
                DateTime.Now.AddYears(-20),
                false
            );

            // Assert
            // A entidade permite, mas o Command Validator deve rejeitar
            student.Email.Should().Be(invalidEmail);
        }

        [Fact]
        public void StudentUser_ShouldAcceptValidDateOfBirth()
        {
            // Arrange
            var validDate = new DateTime(1990, 1, 1);

            // Act
            var student = new StudentUserBuilder()
                .WithDateOfBirth(validDate)
                .Build();

            // Assert
            student.DateOfBirth.Should().Be(validDate);
        }

        [Fact]
        public void StudentUser_ShouldCalculateAge_Correctly()
        {
            // Arrange
            var dateOfBirth = DateTime.Now.AddYears(-25);

            // Act
            var student = new StudentUserBuilder()
                .WithDateOfBirth(dateOfBirth)
                .Build();

            // Calculate age
            var age = DateTime.Now.Year - student.DateOfBirth.Year;

            // Assert
            age.Should().Be(25);
        }

        [Fact]
        public void StudentUser_ShouldAllowUpdatingProperties()
        {
            // Arrange
            var student = new StudentUserBuilder().Build();
            var newEmail = "newemail@academyio.com";
            var newFirstName = "NewFirstName";

            // Act
            student.Email = newEmail;
            student.FirstName = newFirstName;

            // Assert
            student.Email.Should().Be(newEmail);
            student.FirstName.Should().Be(newFirstName);
        }

        [Fact]
        public void StudentUser_ShouldPreserveId_WhenPropertiesAreUpdated()
        {
            // Arrange
            var originalId = Guid.NewGuid();
            var student = new StudentUserBuilder()
                .WithId(originalId)
                .Build();

            // Act
            student.Email = "new@email.com";
            student.FirstName = "NewName";

            // Assert
            student.Id.Should().Be(originalId);
        }
    }
}
