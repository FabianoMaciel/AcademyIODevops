using FluentAssertions;

namespace AcademyIODevops.MessageBus.Tests.Unit
{
    public class MessageBusTests
    {
        [Fact]
        public void Constructor_ShouldAcceptConnectionString_WhenCalled()
        {
            // Arrange
            var connectionString = "host=localhost;username=guest;password=guest";

            // Act
            Action act = () => new MessageBus(connectionString);

            // Assert
            // The constructor should not throw an exception even if connection fails
            // as the TryConnect uses Polly retry policy
            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_ShouldHandleNullConnectionString_Gracefully()
        {
            // Arrange
            string connectionString = null;

            // Act
            Action act = () => new MessageBus(connectionString);

            // Assert
            // Should throw ArgumentNullException or similar
            act.Should().Throw<Exception>();
        }

        [Fact]
        public void Constructor_ShouldHandleEmptyConnectionString_Gracefully()
        {
            // Arrange
            var connectionString = string.Empty;

            // Act
            Action act = () => new MessageBus(connectionString);

            // Assert
            // Should throw an exception for empty connection string
            act.Should().Throw<Exception>();
        }

        [Theory]
        [InlineData("host=localhost")]
        [InlineData("host=localhost;username=guest")]
        [InlineData("host=localhost;username=guest;password=guest")]
        public void Constructor_ShouldAcceptDifferentConnectionStrings_WhenCalled(string connectionString)
        {
            // Act
            Action act = () => new MessageBus(connectionString);

            // Assert
            // The constructor should handle different connection string formats
            act.Should().NotThrow();
        }
    }
}
