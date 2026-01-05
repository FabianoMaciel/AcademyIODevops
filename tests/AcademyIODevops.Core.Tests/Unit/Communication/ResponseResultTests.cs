using AcademyIODevops.Core.Communication;
using FluentAssertions;

namespace AcademyIODevops.Core.Tests.Unit.Communication
{
    public class ResponseResultTests
    {
        [Fact]
        public void Constructor_ShouldInitializeErrorsProperty_WhenCreated()
        {
            // Act
            var responseResult = new ResponseResult();

            // Assert
            responseResult.Errors.Should().NotBeNull();
            responseResult.Errors.Messages.Should().NotBeNull();
            responseResult.Errors.Messages.Should().BeEmpty();
        }

        [Fact]
        public void Title_ShouldBeSettable_WhenAssigned()
        {
            // Arrange
            var responseResult = new ResponseResult();
            var title = "Validation Error";

            // Act
            responseResult.Title = title;

            // Assert
            responseResult.Title.Should().Be(title);
        }

        [Fact]
        public void Status_ShouldBeSettable_WhenAssigned()
        {
            // Arrange
            var responseResult = new ResponseResult();
            var status = 400;

            // Act
            responseResult.Status = status;

            // Assert
            responseResult.Status.Should().Be(status);
        }

        [Fact]
        public void Errors_ShouldAllowAddingMessages_WhenCalled()
        {
            // Arrange
            var responseResult = new ResponseResult();

            // Act
            responseResult.Errors.Messages.Add("Error 1");
            responseResult.Errors.Messages.Add("Error 2");

            // Assert
            responseResult.Errors.Messages.Should().HaveCount(2);
            responseResult.Errors.Messages.Should().Contain("Error 1");
            responseResult.Errors.Messages.Should().Contain("Error 2");
        }

        [Theory]
        [InlineData(200, "Success")]
        [InlineData(400, "Bad Request")]
        [InlineData(404, "Not Found")]
        [InlineData(500, "Internal Server Error")]
        public void ResponseResult_ShouldStoreStatusAndTitle_Correctly(int status, string title)
        {
            // Arrange
            var responseResult = new ResponseResult
            {
                Status = status,
                Title = title
            };

            // Act & Assert
            responseResult.Status.Should().Be(status);
            responseResult.Title.Should().Be(title);
        }

        [Fact]
        public void ResponseResult_ShouldAllowMultipleErrorMessages_ToBeAdded()
        {
            // Arrange
            var responseResult = new ResponseResult();
            var errors = new List<string>
            {
                "Name is required",
                "Email is invalid",
                "Price must be greater than zero"
            };

            // Act
            foreach (var error in errors)
            {
                responseResult.Errors.Messages.Add(error);
            }

            // Assert
            responseResult.Errors.Messages.Should().HaveCount(3);
            responseResult.Errors.Messages.Should().BeEquivalentTo(errors);
        }
    }

    public class ResponseErrorMessagesTests
    {
        [Fact]
        public void Constructor_ShouldInitializeMessagesList_WhenCreated()
        {
            // Act
            var errorMessages = new ResponseErrorMessages();

            // Assert
            errorMessages.Messages.Should().NotBeNull();
            errorMessages.Messages.Should().BeEmpty();
        }

        [Fact]
        public void Messages_ShouldBeSettable_WhenAssigned()
        {
            // Arrange
            var errorMessages = new ResponseErrorMessages();
            var newMessages = new List<string> { "Error 1", "Error 2" };

            // Act
            errorMessages.Messages = newMessages;

            // Assert
            errorMessages.Messages.Should().BeSameAs(newMessages);
            errorMessages.Messages.Should().HaveCount(2);
        }

        [Fact]
        public void Messages_ShouldAllowAddingAndRemoving_WhenCalled()
        {
            // Arrange
            var errorMessages = new ResponseErrorMessages();

            // Act
            errorMessages.Messages.Add("Error 1");
            errorMessages.Messages.Add("Error 2");
            errorMessages.Messages.Add("Error 3");
            errorMessages.Messages.Remove("Error 2");

            // Assert
            errorMessages.Messages.Should().HaveCount(2);
            errorMessages.Messages.Should().Contain("Error 1");
            errorMessages.Messages.Should().NotContain("Error 2");
            errorMessages.Messages.Should().Contain("Error 3");
        }

        [Fact]
        public void Messages_ShouldMaintainOrder_WhenMultipleMessagesAdded()
        {
            // Arrange
            var errorMessages = new ResponseErrorMessages();

            // Act
            errorMessages.Messages.Add("First");
            errorMessages.Messages.Add("Second");
            errorMessages.Messages.Add("Third");

            // Assert
            errorMessages.Messages.Should().HaveCount(3);
            errorMessages.Messages[0].Should().Be("First");
            errorMessages.Messages[1].Should().Be("Second");
            errorMessages.Messages[2].Should().Be("Third");
        }
    }
}
