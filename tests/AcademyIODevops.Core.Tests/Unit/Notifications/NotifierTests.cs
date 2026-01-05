using AcademyIODevops.Core.Notifications;
using FluentAssertions;

namespace AcademyIODevops.Core.Tests.Unit.Notifications
{
    public class NotifierTests
    {
        private readonly Notifier _notifier;

        public NotifierTests()
        {
            _notifier = new Notifier();
        }

        [Fact]
        public void GetNotifications_ShouldReturnEmptyList_WhenNoNotificationsAdded()
        {
            // Act
            var notifications = _notifier.GetNotifications();

            // Assert
            notifications.Should().NotBeNull();
            notifications.Should().BeEmpty();
        }

        [Fact]
        public void Handle_ShouldAddNotification_WhenCalled()
        {
            // Arrange
            var notification = new Notification("Test error message");

            // Act
            _notifier.Handle(notification);

            // Assert
            var notifications = _notifier.GetNotifications();
            notifications.Should().HaveCount(1);
            notifications.Should().Contain(notification);
        }

        [Fact]
        public void Handle_ShouldAddMultipleNotifications_WhenCalledMultipleTimes()
        {
            // Arrange
            var notification1 = new Notification("Error 1");
            var notification2 = new Notification("Error 2");
            var notification3 = new Notification("Error 3");

            // Act
            _notifier.Handle(notification1);
            _notifier.Handle(notification2);
            _notifier.Handle(notification3);

            // Assert
            var notifications = _notifier.GetNotifications();
            notifications.Should().HaveCount(3);
            notifications.Should().Contain(notification1);
            notifications.Should().Contain(notification2);
            notifications.Should().Contain(notification3);
        }

        [Fact]
        public void HasNotification_ShouldReturnFalse_WhenNoNotificationsAdded()
        {
            // Act
            var hasNotification = _notifier.HasNotification();

            // Assert
            hasNotification.Should().BeFalse();
        }

        [Fact]
        public void HasNotification_ShouldReturnTrue_WhenNotificationAdded()
        {
            // Arrange
            var notification = new Notification("Test error");

            // Act
            _notifier.Handle(notification);
            var hasNotification = _notifier.HasNotification();

            // Assert
            hasNotification.Should().BeTrue();
        }

        [Theory]
        [InlineData("Validation error")]
        [InlineData("Database connection failed")]
        [InlineData("Authentication failed")]
        [InlineData("Resource not found")]
        public void Handle_ShouldStoreNotificationWithCorrectMessage_WhenCalledWithDifferentMessages(
            string errorMessage)
        {
            // Arrange
            var notification = new Notification(errorMessage);

            // Act
            _notifier.Handle(notification);

            // Assert
            var notifications = _notifier.GetNotifications();
            notifications.Should().HaveCount(1);
            notifications.First().Message.Should().Be(errorMessage);
        }

        [Fact]
        public void GetNotifications_ShouldReturnAllNotifications_InOrderAdded()
        {
            // Arrange
            var notification1 = new Notification("First error");
            var notification2 = new Notification("Second error");
            var notification3 = new Notification("Third error");

            // Act
            _notifier.Handle(notification1);
            _notifier.Handle(notification2);
            _notifier.Handle(notification3);

            // Assert
            var notifications = _notifier.GetNotifications();
            notifications.Should().HaveCount(3);
            notifications[0].Message.Should().Be("First error");
            notifications[1].Message.Should().Be("Second error");
            notifications[2].Message.Should().Be("Third error");
        }

        [Fact]
        public void GetNotifications_ShouldReturnSameListReference_WhenCalledMultipleTimes()
        {
            // Arrange
            var notification = new Notification("Test");
            _notifier.Handle(notification);

            // Act
            var notifications1 = _notifier.GetNotifications();
            var notifications2 = _notifier.GetNotifications();

            // Assert
            notifications1.Should().BeSameAs(notifications2);
        }
    }
}
