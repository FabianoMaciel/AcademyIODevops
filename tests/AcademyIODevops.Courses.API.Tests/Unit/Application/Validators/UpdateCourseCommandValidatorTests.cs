using AcademyIODevops.Courses.API.Application.Commands;
using FluentAssertions;

namespace AcademyIODevops.Courses.API.Tests.Unit.Application.Validators
{
    public class UpdateCourseCommandValidatorTests
    {
        [Fact]
        public void Validate_ShouldReturnValid_WhenAllFieldsAreValid()
        {
            // Arrange
            var command = new UpdateCourseCommand(
                name: "Docker Course",
                description: "Learn Docker containers",
                userCreationId: Guid.NewGuid(),
                price: 99.99,
                courseId: Guid.NewGuid()
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_ShouldReturnInvalid_WhenNameIsEmpty(string invalidName)
        {
            // Arrange
            var command = new UpdateCourseCommand(
                name: invalidName,
                description: "Valid description",
                userCreationId: Guid.NewGuid(),
                price: 99.99,
                courseId: Guid.NewGuid()
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == UpdateCourseCommandValidation.NameError);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_ShouldReturnInvalid_WhenDescriptionIsEmpty(string invalidDescription)
        {
            // Arrange
            var command = new UpdateCourseCommand(
                name: "Docker Course",
                description: invalidDescription,
                userCreationId: Guid.NewGuid(),
                price: 99.99,
                courseId: Guid.NewGuid()
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == UpdateCourseCommandValidation.ContextError);
        }

        [Fact]
        public void Validate_ShouldReturnInvalid_WhenUserCreationIdIsEmpty()
        {
            // Arrange
            var command = new UpdateCourseCommand(
                name: "Docker Course",
                description: "Valid description",
                userCreationId: Guid.Empty,
                price: 99.99,
                courseId: Guid.NewGuid()
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == UpdateCourseCommandValidation.UserCreationError);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100.50)]
        public void Validate_ShouldReturnInvalid_WhenPriceIsZeroOrNegative(double invalidPrice)
        {
            // Arrange
            var command = new UpdateCourseCommand(
                name: "Docker Course",
                description: "Valid description",
                userCreationId: Guid.NewGuid(),
                price: invalidPrice,
                courseId: Guid.NewGuid()
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == UpdateCourseCommandValidation.PriceErro);
        }

        [Fact]
        public void Validate_ShouldReturnInvalid_WhenCourseIdIsEmpty()
        {
            // Arrange
            var command = new UpdateCourseCommand(
                name: "Docker Course",
                description: "Valid description",
                userCreationId: Guid.NewGuid(),
                price: 99.99,
                courseId: Guid.Empty
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == UpdateCourseCommandValidation.IdError);
        }

        [Fact]
        public void Validate_ShouldReturnMultipleErrors_WhenMultipleFieldsAreInvalid()
        {
            // Arrange
            var command = new UpdateCourseCommand(
                name: "",
                description: "",
                userCreationId: Guid.Empty,
                price: -10,
                courseId: Guid.Empty
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().HaveCount(5);
        }

        [Theory]
        [InlineData("Docker Fundamentals", "Complete Docker Guide", 199.99)]
        [InlineData("Kubernetes Basics", "Introduction to K8s", 149.99)]
        [InlineData("CI/CD Pipeline", "DevOps automation", 99.99)]
        public void Validate_ShouldReturnValid_WithDifferentValidData(
            string name, string description, double price)
        {
            // Arrange
            var command = new UpdateCourseCommand(
                name: name,
                description: description,
                userCreationId: Guid.NewGuid(),
                price: price,
                courseId: Guid.NewGuid()
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }
    }
}
