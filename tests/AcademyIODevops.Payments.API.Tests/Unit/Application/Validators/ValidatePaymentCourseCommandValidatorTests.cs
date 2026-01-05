using AcademyIODevops.Payments.API.Services;
using FluentAssertions;

namespace AcademyIODevops.Payments.API.Tests.Unit.Application.Validators
{
    public class ValidatePaymentCourseCommandValidatorTests
    {
        [Fact]
        public void Validate_ShouldReturnValid_WhenAllFieldsAreValid()
        {
            // Arrange
            var command = new ValidatePaymentCourseCommand(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4532015112830366", // Valid test card number
                cardExpirationDate: "12/2025",
                cardCVV: "123"
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_ShouldReturnInvalid_WhenCourseIdIsEmpty()
        {
            // Arrange
            var command = new ValidatePaymentCourseCommand(
                courseId: Guid.Empty,
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4532015112830366",
                cardExpirationDate: "12/2025",
                cardCVV: "123"
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == ValidatePaymentCourseCommandValidation.CourseIdError);
        }

        [Fact]
        public void Validate_ShouldReturnInvalid_WhenStudentIdIsEmpty()
        {
            // Arrange
            var command = new ValidatePaymentCourseCommand(
                courseId: Guid.NewGuid(),
                studentId: Guid.Empty,
                cardName: "JOHN DOE",
                cardNumber: "4532015112830366",
                cardExpirationDate: "12/2025",
                cardCVV: "123"
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == ValidatePaymentCourseCommandValidation.StudentIdError);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_ShouldReturnInvalid_WhenCardNameIsEmpty(string invalidCardName)
        {
            // Arrange
            var command = new ValidatePaymentCourseCommand(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: invalidCardName,
                cardNumber: "4532015112830366",
                cardExpirationDate: "12/2025",
                cardCVV: "123"
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == ValidatePaymentCourseCommandValidation.CardNameError);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_ShouldReturnInvalid_WhenCardNumberIsEmpty(string invalidCardNumber)
        {
            // Arrange
            var command = new ValidatePaymentCourseCommand(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: invalidCardNumber,
                cardExpirationDate: "12/2025",
                cardCVV: "123"
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == ValidatePaymentCourseCommandValidation.CardNumberError);
        }

        [Theory]
        [InlineData("1234567890123456")] // Invalid Luhn check
        [InlineData("1111111111111111")]
        public void Validate_ShouldReturnInvalid_WhenCardNumberIsInvalid(string invalidCardNumber)
        {
            // Arrange
            var command = new ValidatePaymentCourseCommand(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: invalidCardNumber,
                cardExpirationDate: "12/2025",
                cardCVV: "123"
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == ValidatePaymentCourseCommandValidation.InvalidCard);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_ShouldReturnInvalid_WhenCardExpirationDateIsEmpty(string invalidDate)
        {
            // Arrange
            var command = new ValidatePaymentCourseCommand(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4532015112830366",
                cardExpirationDate: invalidDate,
                cardCVV: "123"
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == ValidatePaymentCourseCommandValidation.CardExpirationDateError);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Validate_ShouldReturnInvalid_WhenCardCVVIsEmpty(string invalidCVV)
        {
            // Arrange
            var command = new ValidatePaymentCourseCommand(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4532015112830366",
                cardExpirationDate: "12/2025",
                cardCVV: invalidCVV
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().Contain(e =>
                e.ErrorMessage == ValidatePaymentCourseCommandValidation.CardCVVError);
        }

        [Theory]
        [InlineData("5555555555554444")] // Mastercard
        [InlineData("378282246310005")]  // American Express
        [InlineData("6011111111111117")] // Discover
        public void Validate_ShouldReturnValid_WithDifferentValidCardNumbers(string validCardNumber)
        {
            // Arrange
            var command = new ValidatePaymentCourseCommand(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: validCardNumber,
                cardExpirationDate: "12/2025",
                cardCVV: "123"
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_ShouldReturnMultipleErrors_WhenMultipleFieldsAreInvalid()
        {
            // Arrange
            var command = new ValidatePaymentCourseCommand(
                courseId: Guid.Empty,
                studentId: Guid.Empty,
                cardName: "",
                cardNumber: "",
                cardExpirationDate: "",
                cardCVV: ""
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeFalse();
            command.ValidationResult.Errors.Should().HaveCountGreaterThanOrEqualTo(6);
        }

        [Fact]
        public void Validate_ShouldPreserveAllProperties_WhenValidationSucceeds()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var cardName = "JOHN DOE";
            var cardNumber = "4532015112830366";
            var cardExpirationDate = "12/2025";
            var cardCVV = "123";

            var command = new ValidatePaymentCourseCommand(
                courseId: courseId,
                studentId: studentId,
                cardName: cardName,
                cardNumber: cardNumber,
                cardExpirationDate: cardExpirationDate,
                cardCVV: cardCVV
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
            command.CourseId.Should().Be(courseId);
            command.StudentId.Should().Be(studentId);
            command.CardName.Should().Be(cardName);
            command.CardNumber.Should().Be(cardNumber);
            command.CardExpirationDate.Should().Be(cardExpirationDate);
            command.CardCVV.Should().Be(cardCVV);
        }

        [Fact]
        public void Validate_ShouldBeValid_WithLowercaseCardName()
        {
            // Arrange
            var command = new ValidatePaymentCourseCommand(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "john doe",
                cardNumber: "4532015112830366",
                cardExpirationDate: "12/2025",
                cardCVV: "123"
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("123")]
        [InlineData("456")]
        [InlineData("789")]
        [InlineData("000")]
        public void Validate_ShouldBeValid_WithDifferentCVVValues(string cvv)
        {
            // Arrange
            var command = new ValidatePaymentCourseCommand(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4532015112830366",
                cardExpirationDate: "12/2025",
                cardCVV: cvv
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("01/2026")]
        [InlineData("12/2030")]
        [InlineData("06/2025")]
        public void Validate_ShouldBeValid_WithDifferentExpirationDates(string expirationDate)
        {
            // Arrange
            var command = new ValidatePaymentCourseCommand(
                courseId: Guid.NewGuid(),
                studentId: Guid.NewGuid(),
                cardName: "JOHN DOE",
                cardNumber: "4532015112830366",
                cardExpirationDate: expirationDate,
                cardCVV: "123"
            );

            // Act
            var result = command.IsValid();

            // Assert
            result.Should().BeTrue();
        }
    }
}
