using AcademyIODevops.Core.DomainObjects.DTOs;
using AcademyIODevops.Core.Messages;
using AcademyIODevops.Core.Messages.IntegrationCommands;
using FluentAssertions;
using Xunit;

namespace AcademyIODevops.Core.Tests.Messages.IntegrationCommands
{
    public class MakePaymentCourseCommandTests
    {
        private PaymentCourse CreateValidPaymentCourse()
        {
            return new PaymentCourse
            {
                CourseId = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                Total = 99.99,
                CardName = "JOHN DOE",
                CardNumber = "4111111111111111",
                CardExpirationDate = "12/25",
                CardCVV = "123"
            };
        }

        [Fact]
        public void MakePaymentCourseCommand_ShouldCreateWithValidPaymentCourse()
        {
            // Arrange
            var paymentCourse = CreateValidPaymentCourse();

            // Act
            var command = new MakePaymentCourseCommand(paymentCourse);

            // Assert
            command.Should().NotBeNull();
            command.PaymentCourse.Should().Be(paymentCourse);
        }

        [Fact]
        public void MakePaymentCourseCommand_ShouldInheritFromCommand()
        {
            // Arrange
            var paymentCourse = CreateValidPaymentCourse();

            // Act
            var command = new MakePaymentCourseCommand(paymentCourse);

            // Assert
            command.Should().BeAssignableTo<Command>();
        }

        [Fact]
        public void IsValid_ShouldReturnTrue_WhenPaymentCourseIsValid()
        {
            // Arrange
            var paymentCourse = CreateValidPaymentCourse();
            var command = new MakePaymentCourseCommand(paymentCourse);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeTrue();
            command.ValidationResult.Should().NotBeNull();
            command.ValidationResult.IsValid.Should().BeTrue();
            command.ValidationResult.Errors.Should().BeEmpty();
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenPaymentCourseIsNull()
        {
            // Arrange
            var command = new MakePaymentCourseCommand(null);

            // Act
            var isValid = command.IsValid();

            // Assert
            isValid.Should().BeFalse();
            command.ValidationResult.Should().NotBeNull();
            command.ValidationResult.IsValid.Should().BeFalse();
            command.ValidationResult.Errors.Should().HaveCount(1);
            command.ValidationResult.Errors[0].ErrorMessage.Should().Be(MakePaymentCourseCommandValidation.PaymentCourseError);
        }

        [Fact]
        public void MakePaymentCourseCommand_ShouldAllowSettingPaymentCourse()
        {
            // Arrange
            var initialPaymentCourse = CreateValidPaymentCourse();
            var newPaymentCourse = CreateValidPaymentCourse();
            var command = new MakePaymentCourseCommand(initialPaymentCourse);

            // Act
            command.PaymentCourse = newPaymentCourse;

            // Assert
            command.PaymentCourse.Should().Be(newPaymentCourse);
        }

        [Fact]
        public void IsValid_ShouldBeIdempotent()
        {
            // Arrange
            var paymentCourse = CreateValidPaymentCourse();
            var command = new MakePaymentCourseCommand(paymentCourse);

            // Act
            var isValid1 = command.IsValid();
            var isValid2 = command.IsValid();
            var isValid3 = command.IsValid();

            // Assert
            isValid1.Should().Be(isValid2);
            isValid2.Should().Be(isValid3);
            isValid1.Should().BeTrue();
        }

        [Fact]
        public void IsValid_ShouldUpdateValidationResult_AfterPropertyChange()
        {
            // Arrange
            var command = new MakePaymentCourseCommand(null);

            // Act
            var isValidBefore = command.IsValid();
            command.PaymentCourse = CreateValidPaymentCourse();
            var isValidAfter = command.IsValid();

            // Assert
            isValidBefore.Should().BeFalse();
            isValidAfter.Should().BeTrue();
        }

        [Theory]
        [InlineData(10.50)]
        [InlineData(99.99)]
        [InlineData(150.00)]
        [InlineData(1000.00)]
        public void MakePaymentCourseCommand_ShouldAcceptDifferentTotalValues(double total)
        {
            // Arrange
            var paymentCourse = CreateValidPaymentCourse();
            paymentCourse.Total = total;

            // Act
            var command = new MakePaymentCourseCommand(paymentCourse);

            // Assert
            command.PaymentCourse.Total.Should().Be(total);
            command.IsValid().Should().BeTrue();
        }

        [Theory]
        [InlineData("JOHN DOE")]
        [InlineData("Jane Smith")]
        [InlineData("María García")]
        [InlineData("A")]
        public void MakePaymentCourseCommand_ShouldAcceptDifferentCardNames(string cardName)
        {
            // Arrange
            var paymentCourse = CreateValidPaymentCourse();
            paymentCourse.CardName = cardName;

            // Act
            var command = new MakePaymentCourseCommand(paymentCourse);

            // Assert
            command.PaymentCourse.CardName.Should().Be(cardName);
            command.IsValid().Should().BeTrue();
        }

        [Theory]
        [InlineData("4111111111111111")]
        [InlineData("5555555555554444")]
        [InlineData("378282246310005")]
        public void MakePaymentCourseCommand_ShouldAcceptDifferentCardNumbers(string cardNumber)
        {
            // Arrange
            var paymentCourse = CreateValidPaymentCourse();
            paymentCourse.CardNumber = cardNumber;

            // Act
            var command = new MakePaymentCourseCommand(paymentCourse);

            // Assert
            command.PaymentCourse.CardNumber.Should().Be(cardNumber);
            command.IsValid().Should().BeTrue();
        }

        [Fact]
        public void MakePaymentCourseCommand_ShouldPreserveAllPaymentCourseProperties()
        {
            // Arrange
            var expectedCourseId = Guid.NewGuid();
            var expectedStudentId = Guid.NewGuid();
            var expectedTotal = 123.45;
            var expectedCardName = "TEST USER";
            var expectedCardNumber = "4111111111111111";
            var expectedExpirationDate = "06/26";
            var expectedCVV = "456";

            var paymentCourse = new PaymentCourse
            {
                CourseId = expectedCourseId,
                StudentId = expectedStudentId,
                Total = expectedTotal,
                CardName = expectedCardName,
                CardNumber = expectedCardNumber,
                CardExpirationDate = expectedExpirationDate,
                CardCVV = expectedCVV
            };

            // Act
            var command = new MakePaymentCourseCommand(paymentCourse);

            // Assert
            command.PaymentCourse.CourseId.Should().Be(expectedCourseId);
            command.PaymentCourse.StudentId.Should().Be(expectedStudentId);
            command.PaymentCourse.Total.Should().Be(expectedTotal);
            command.PaymentCourse.CardName.Should().Be(expectedCardName);
            command.PaymentCourse.CardNumber.Should().Be(expectedCardNumber);
            command.PaymentCourse.CardExpirationDate.Should().Be(expectedExpirationDate);
            command.PaymentCourse.CardCVV.Should().Be(expectedCVV);
        }

        [Fact]
        public void MakePaymentCourseCommand_ShouldAcceptPaymentCourseWithEmptyGuids()
        {
            // Arrange
            var paymentCourse = CreateValidPaymentCourse();
            paymentCourse.CourseId = Guid.Empty;
            paymentCourse.StudentId = Guid.Empty;

            // Act
            var command = new MakePaymentCourseCommand(paymentCourse);

            // Assert
            command.PaymentCourse.CourseId.Should().Be(Guid.Empty);
            command.PaymentCourse.StudentId.Should().Be(Guid.Empty);
            command.IsValid().Should().BeTrue("validation only checks if PaymentCourse is not null");
        }

        [Fact]
        public void MakePaymentCourseCommand_ShouldAcceptPaymentCourseWithZeroTotal()
        {
            // Arrange
            var paymentCourse = CreateValidPaymentCourse();
            paymentCourse.Total = 0;

            // Act
            var command = new MakePaymentCourseCommand(paymentCourse);

            // Assert
            command.PaymentCourse.Total.Should().Be(0);
            command.IsValid().Should().BeTrue("validation only checks if PaymentCourse is not null");
        }

        [Fact]
        public void MakePaymentCourseCommand_ShouldAcceptPaymentCourseWithEmptyCardDetails()
        {
            // Arrange
            var paymentCourse = new PaymentCourse
            {
                CourseId = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                Total = 99.99,
                CardName = "",
                CardNumber = "",
                CardExpirationDate = "",
                CardCVV = ""
            };

            // Act
            var command = new MakePaymentCourseCommand(paymentCourse);

            // Assert
            command.IsValid().Should().BeTrue("validation only checks if PaymentCourse is not null");
        }
    }

    public class MakePaymentCourseCommandValidationTests
    {
        private PaymentCourse CreateValidPaymentCourse()
        {
            return new PaymentCourse
            {
                CourseId = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                Total = 99.99,
                CardName = "JOHN DOE",
                CardNumber = "4111111111111111",
                CardExpirationDate = "12/25",
                CardCVV = "123"
            };
        }

        [Fact]
        public void Validate_ShouldPass_WhenPaymentCourseIsNotNull()
        {
            // Arrange
            var validator = new MakePaymentCourseCommandValidation();
            var paymentCourse = CreateValidPaymentCourse();
            var command = new MakePaymentCourseCommand(paymentCourse);

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_ShouldFail_WhenPaymentCourseIsNull()
        {
            // Arrange
            var validator = new MakePaymentCourseCommandValidation();
            var command = new MakePaymentCourseCommand(null);

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(1);
            result.Errors[0].PropertyName.Should().Be(nameof(MakePaymentCourseCommand.PaymentCourse));
            result.Errors[0].ErrorMessage.Should().Be(MakePaymentCourseCommandValidation.PaymentCourseError);
        }

        [Fact]
        public void Validation_ShouldHaveCorrectErrorMessage()
        {
            // Assert
            MakePaymentCourseCommandValidation.PaymentCourseError.Should().Be("O pagamento do curso não pode ser vazio.");
        }

        [Fact]
        public void Validator_ShouldBeReusable()
        {
            // Arrange
            var validator = new MakePaymentCourseCommandValidation();
            var command1 = new MakePaymentCourseCommand(CreateValidPaymentCourse());
            var command2 = new MakePaymentCourseCommand(null);

            // Act
            var result1 = validator.Validate(command1);
            var result2 = validator.Validate(command2);

            // Assert
            result1.IsValid.Should().BeTrue();
            result2.IsValid.Should().BeFalse();
        }

        [Fact]
        public void Validate_ShouldPass_WhenPaymentCourseHasEmptyStrings()
        {
            // Arrange
            var validator = new MakePaymentCourseCommandValidation();
            var paymentCourse = new PaymentCourse
            {
                CourseId = Guid.NewGuid(),
                StudentId = Guid.NewGuid(),
                Total = 0,
                CardName = "",
                CardNumber = "",
                CardExpirationDate = "",
                CardCVV = ""
            };
            var command = new MakePaymentCourseCommand(paymentCourse);

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue("validation only checks if PaymentCourse object is not null, not its properties");
        }

        [Fact]
        public void Validate_ShouldPass_WhenPaymentCourseHasEmptyGuids()
        {
            // Arrange
            var validator = new MakePaymentCourseCommandValidation();
            var paymentCourse = new PaymentCourse
            {
                CourseId = Guid.Empty,
                StudentId = Guid.Empty,
                Total = 99.99,
                CardName = "JOHN DOE",
                CardNumber = "4111111111111111",
                CardExpirationDate = "12/25",
                CardCVV = "123"
            };
            var command = new MakePaymentCourseCommand(paymentCourse);

            // Act
            var result = validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue("validation only checks if PaymentCourse object is not null, not its Guid properties");
        }

        [Fact]
        public void Validate_ShouldPass_WithDifferentPaymentAmounts()
        {
            // Arrange
            var validator = new MakePaymentCourseCommandValidation();

            var amounts = new[] { 0.01, 10.50, 99.99, 1000.00, 9999.99 };

            foreach (var amount in amounts)
            {
                var paymentCourse = CreateValidPaymentCourse();
                paymentCourse.Total = amount;
                var command = new MakePaymentCourseCommand(paymentCourse);

                // Act
                var result = validator.Validate(command);

                // Assert
                result.IsValid.Should().BeTrue($"payment with amount {amount} should be valid");
            }
        }

        [Fact]
        public void Validator_ShouldValidateMultipleCommands_Independently()
        {
            // Arrange
            var validator = new MakePaymentCourseCommandValidation();
            var validCommand = new MakePaymentCourseCommand(CreateValidPaymentCourse());
            var invalidCommand = new MakePaymentCourseCommand(null);

            // Act
            var validResult1 = validator.Validate(validCommand);
            var invalidResult = validator.Validate(invalidCommand);
            var validResult2 = validator.Validate(validCommand);

            // Assert
            validResult1.IsValid.Should().BeTrue();
            invalidResult.IsValid.Should().BeFalse();
            validResult2.IsValid.Should().BeTrue();
        }
    }
}
