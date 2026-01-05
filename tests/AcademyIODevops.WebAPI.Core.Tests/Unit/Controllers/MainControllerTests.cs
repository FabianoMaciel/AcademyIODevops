using AcademyIODevops.Core.Communication;
using AcademyIODevops.WebAPI.Core.Controllers;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AcademyIODevops.WebAPI.Core.Tests.Unit.Controllers
{
    // Concrete implementation for testing abstract MainController
    public class TestController : MainController
    {
        public IActionResult TestCustomResponse(object? result = null)
        {
            return CustomResponse(result);
        }

        public IActionResult TestCustomResponseWithModelState(ModelStateDictionary modelState)
        {
            return CustomResponse(modelState);
        }

        public IActionResult TestCustomResponseWithValidationResult(ValidationResult validationResult)
        {
            return CustomResponse(validationResult);
        }

        public IActionResult TestCustomResponseWithResponseResult(ResponseResult responseResult)
        {
            return CustomResponse(responseResult);
        }

        public bool TestResponseHasErrors(ResponseResult responseResult)
        {
            return ResponseHasErrors(responseResult);
        }

        public bool TestValidOperation()
        {
            return ValidOperation();
        }

        public void TestAddErrorToStack(string error)
        {
            AddErrorToStack(error);
        }

        public void TestCleanErrors()
        {
            CleanErrors();
        }

        public ICollection<string> GetErrors()
        {
            return Errors;
        }
    }

    public class MainControllerTests
    {
        private readonly TestController _controller;

        public MainControllerTests()
        {
            _controller = new TestController();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
        }

        [Fact]
        public void CustomResponse_ShouldReturnOk_WhenNoErrors()
        {
            // Arrange
            var result = new { Message = "Success" };

            // Act
            var response = _controller.TestCustomResponse(result);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            var okResult = response as OkObjectResult;
            okResult?.Value.Should().Be(result);
        }

        [Fact]
        public void CustomResponse_ShouldReturnBadRequest_WhenHasErrors()
        {
            // Arrange
            _controller.TestAddErrorToStack("Error 1");
            _controller.TestAddErrorToStack("Error 2");

            // Act
            var response = _controller.TestCustomResponse();

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = response as BadRequestObjectResult;
            badRequestResult?.Value.Should().BeOfType<ValidationProblemDetails>();

            var problemDetails = badRequestResult?.Value as ValidationProblemDetails;
            problemDetails?.Errors.Should().ContainKey("Messages");
            problemDetails?.Errors["Messages"].Should().Contain("Error 1");
            problemDetails?.Errors["Messages"].Should().Contain("Error 2");
        }

        [Fact]
        public void CustomResponseWithModelState_ShouldReturnBadRequest_WhenModelStateInvalid()
        {
            // Arrange
            var modelState = new ModelStateDictionary();
            modelState.AddModelError("Name", "Name is required");
            modelState.AddModelError("Email", "Email is invalid");

            // Act
            var response = _controller.TestCustomResponseWithModelState(modelState);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = response as BadRequestObjectResult;
            badRequestResult?.Value.Should().BeOfType<ValidationProblemDetails>();

            var problemDetails = badRequestResult?.Value as ValidationProblemDetails;
            problemDetails?.Errors["Messages"].Should().Contain("Name is required");
            problemDetails?.Errors["Messages"].Should().Contain("Email is invalid");
        }

        [Fact]
        public void CustomResponseWithValidationResult_ShouldReturnBadRequest_WhenValidationFails()
        {
            // Arrange
            var validationResult = new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Name is required"),
                new ValidationFailure("Price", "Price must be greater than zero")
            });

            // Act
            var response = _controller.TestCustomResponseWithValidationResult(validationResult);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = response as BadRequestObjectResult;
            badRequestResult?.Value.Should().BeOfType<ValidationProblemDetails>();

            var problemDetails = badRequestResult?.Value as ValidationProblemDetails;
            problemDetails?.Errors["Messages"].Should().Contain("Name is required");
            problemDetails?.Errors["Messages"].Should().Contain("Price must be greater than zero");
        }

        [Fact]
        public void CustomResponseWithResponseResult_ShouldReturnBadRequest_WhenResponseHasErrors()
        {
            // Arrange
            var responseResult = new ResponseResult();
            responseResult.Errors.Messages.Add("Service error 1");
            responseResult.Errors.Messages.Add("Service error 2");

            // Act
            var response = _controller.TestCustomResponseWithResponseResult(responseResult);

            // Assert
            response.Should().BeOfType<BadRequestObjectResult>();
            var badRequestResult = response as BadRequestObjectResult;
            badRequestResult?.Value.Should().BeOfType<ValidationProblemDetails>();

            var problemDetails = badRequestResult?.Value as ValidationProblemDetails;
            problemDetails?.Errors["Messages"].Should().Contain("Service error 1");
            problemDetails?.Errors["Messages"].Should().Contain("Service error 2");
        }

        [Fact]
        public void ResponseHasErrors_ShouldReturnFalse_WhenResponseIsNull()
        {
            // Act
            var result = _controller.TestResponseHasErrors(null);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ResponseHasErrors_ShouldReturnFalse_WhenResponseHasNoErrors()
        {
            // Arrange
            var responseResult = new ResponseResult();

            // Act
            var result = _controller.TestResponseHasErrors(responseResult);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void ResponseHasErrors_ShouldReturnTrue_WhenResponseHasErrors()
        {
            // Arrange
            var responseResult = new ResponseResult();
            responseResult.Errors.Messages.Add("Error message");

            // Act
            var result = _controller.TestResponseHasErrors(responseResult);

            // Assert
            result.Should().BeTrue();
            _controller.GetErrors().Should().Contain("Error message");
        }

        [Fact]
        public void ValidOperation_ShouldReturnTrue_WhenNoErrors()
        {
            // Act
            var result = _controller.TestValidOperation();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void ValidOperation_ShouldReturnFalse_WhenHasErrors()
        {
            // Arrange
            _controller.TestAddErrorToStack("Some error");

            // Act
            var result = _controller.TestValidOperation();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void AddErrorToStack_ShouldAddError_WhenCalled()
        {
            // Arrange
            var errorMessage = "Test error message";

            // Act
            _controller.TestAddErrorToStack(errorMessage);

            // Assert
            _controller.GetErrors().Should().Contain(errorMessage);
            _controller.GetErrors().Should().HaveCount(1);
        }

        [Fact]
        public void AddErrorToStack_ShouldAddMultipleErrors_WhenCalledMultipleTimes()
        {
            // Arrange
            var error1 = "Error 1";
            var error2 = "Error 2";
            var error3 = "Error 3";

            // Act
            _controller.TestAddErrorToStack(error1);
            _controller.TestAddErrorToStack(error2);
            _controller.TestAddErrorToStack(error3);

            // Assert
            _controller.GetErrors().Should().HaveCount(3);
            _controller.GetErrors().Should().Contain(new[] { error1, error2, error3 });
        }

        [Fact]
        public void CleanErrors_ShouldClearAllErrors_WhenCalled()
        {
            // Arrange
            _controller.TestAddErrorToStack("Error 1");
            _controller.TestAddErrorToStack("Error 2");
            _controller.GetErrors().Should().HaveCount(2);

            // Act
            _controller.TestCleanErrors();

            // Assert
            _controller.GetErrors().Should().BeEmpty();
        }

        [Theory]
        [InlineData("Validation error")]
        [InlineData("Database connection failed")]
        [InlineData("Authentication failed")]
        public void AddErrorToStack_ShouldHandleDifferentErrorMessages_Correctly(string errorMessage)
        {
            // Act
            _controller.TestAddErrorToStack(errorMessage);

            // Assert
            _controller.GetErrors().Should().Contain(errorMessage);
        }

        [Fact]
        public void CustomResponse_ShouldReturnOk_WithNullResult_WhenNoErrors()
        {
            // Act
            var response = _controller.TestCustomResponse(null);

            // Assert
            response.Should().BeOfType<OkObjectResult>();
            var okResult = response as OkObjectResult;
            okResult?.Value.Should().BeNull();
        }
    }
}
