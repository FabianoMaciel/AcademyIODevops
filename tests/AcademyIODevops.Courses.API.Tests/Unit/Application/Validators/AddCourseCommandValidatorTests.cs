using AcademyIODevops.Courses.API.Application.Commands;
using FluentAssertions;

namespace AcademyIODevops.Courses.API.Tests.Unit.Application.Validators
{
    public class AddCourseCommandValidatorTests
    {
        private readonly AddCourseCommandValidation _validator;

        public AddCourseCommandValidatorTests()
        {
            _validator = new AddCourseCommandValidation();
        }

        [Fact]
        public void Validate_ShouldPass_WhenAllPropertiesAreValid()
        {
            // Arrange
            var command = new AddCourseCommand(
                name: "DevOps Fundamentals",
                description: "Learn Docker, Kubernetes and CI/CD",
                userCreationId: Guid.NewGuid(),
                price: 99.99
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_ShouldFail_WhenNameIsEmpty()
        {
            // Arrange
            var command = new AddCourseCommand(
                name: string.Empty,
                description: "Valid Description",
                userCreationId: Guid.NewGuid(),
                price: 99.99
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors[0].PropertyName.Should().Be("Name");
            result.Errors[0].ErrorMessage.Should().Be(AddCourseCommandValidation.NameError);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_ShouldFail_WhenNameIsNullOrWhitespace(string invalidName)
        {
            // Arrange
            var command = new AddCourseCommand(
                name: invalidName,
                description: "Valid Description",
                userCreationId: Guid.NewGuid(),
                price: 99.99
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
        }

        [Fact]
        public void Validate_ShouldFail_WhenDescriptionIsEmpty()
        {
            // Arrange
            var command = new AddCourseCommand(
                name: "Valid Course Name",
                description: string.Empty,
                userCreationId: Guid.NewGuid(),
                price: 99.99
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors[0].PropertyName.Should().Be("Description");
            result.Errors[0].ErrorMessage.Should().Be(AddCourseCommandValidation.ContextError);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_ShouldFail_WhenDescriptionIsNullOrWhitespace(string invalidDescription)
        {
            // Arrange
            var command = new AddCourseCommand(
                name: "Valid Course Name",
                description: invalidDescription,
                userCreationId: Guid.NewGuid(),
                price: 99.99
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "Description");
        }

        [Fact]
        public void Validate_ShouldFail_WhenUserCreationIdIsEmpty()
        {
            // Arrange
            var command = new AddCourseCommand(
                name: "Valid Course",
                description: "Valid Description",
                userCreationId: Guid.Empty,
                price: 99.99
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors[0].PropertyName.Should().Be("UserCreationId");
            result.Errors[0].ErrorMessage.Should().Be(AddCourseCommandValidation.UserCreationError);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10.50)]
        [InlineData(-999.99)]
        public void Validate_ShouldFail_WhenPriceIsZeroOrNegative(double invalidPrice)
        {
            // Arrange
            var command = new AddCourseCommand(
                name: "Valid Course",
                description: "Valid Description",
                userCreationId: Guid.NewGuid(),
                price: invalidPrice
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().ContainSingle();
            result.Errors[0].PropertyName.Should().Be("Price");
            result.Errors[0].ErrorMessage.Should().Be(AddCourseCommandValidation.PriceErro);
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(1.00)]
        [InlineData(50.50)]
        [InlineData(99.99)]
        [InlineData(999.99)]
        [InlineData(9999.99)]
        public void Validate_ShouldPass_WhenPriceIsPositive(double validPrice)
        {
            // Arrange
            var command = new AddCourseCommand(
                name: "Valid Course",
                description: "Valid Description",
                userCreationId: Guid.NewGuid(),
                price: validPrice
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }

        [Fact]
        public void Validate_ShouldFail_WhenMultiplePropertiesAreInvalid()
        {
            // Arrange
            var command = new AddCourseCommand(
                name: string.Empty,
                description: string.Empty,
                userCreationId: Guid.Empty,
                price: -10
            );

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(4);
            result.Errors.Should().Contain(e => e.PropertyName == "Name");
            result.Errors.Should().Contain(e => e.PropertyName == "Description");
            result.Errors.Should().Contain(e => e.PropertyName == "UserCreationId");
            result.Errors.Should().Contain(e => e.PropertyName == "Price");
        }

        [Fact]
        public void IsValid_ShouldReturnTrue_WhenCommandIsValid()
        {
            // Arrange
            var command = new AddCourseCommand(
                name: "Kubernetes Deep Dive",
                description: "Advanced Kubernetes course",
                userCreationId: Guid.NewGuid(),
                price: 199.99
            );

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
            command.ValidationResult.Should().NotBeNull();
            command.ValidationResult.IsValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenCommandIsInvalid()
        {
            // Arrange
            var command = new AddCourseCommand(
                name: string.Empty,
                description: "Valid Description",
                userCreationId: Guid.NewGuid(),
                price: 99.99
            );

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.ValidationResult.Should().NotBeNull();
            command.ValidationResult.IsValid.Should().BeFalse();
            command.ValidationResult.Errors.Should().NotBeEmpty();
        }
    }
}
