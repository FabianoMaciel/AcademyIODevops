using AcademyIODevops.Core.DomainObjects;
using AcademyIODevops.Core.Messages;
using FluentAssertions;
using Xunit;

namespace AcademyIODevops.Core.Tests.DomainObjects
{
    public class EntityTests
    {
        // Concrete implementation for testing purposes
        private class TestEntity : Entity
        {
            public TestEntity() : base()
            {
            }

            public TestEntity(Guid id) : base(id)
            {
            }
        }

        private class TestEvent : Event
        {
            public string Name { get; set; }
        }

        [Fact]
        public void Entity_ShouldGenerateNewGuid_WhenCreatedWithParameterlessConstructor()
        {
            // Act
            var entity = new TestEntity();

            // Assert
            entity.Id.Should().NotBeEmpty();
            entity.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void Entity_ShouldUseProvidedGuid_WhenCreatedWithGuidConstructor()
        {
            // Arrange
            var expectedId = Guid.NewGuid();

            // Act
            var entity = new TestEntity(expectedId);

            // Assert
            entity.Id.Should().Be(expectedId);
        }

        [Fact]
        public void Entity_ShouldGenerateUniqueIds_ForMultipleInstances()
        {
            // Act
            var entity1 = new TestEntity();
            var entity2 = new TestEntity();
            var entity3 = new TestEntity();

            // Assert
            entity1.Id.Should().NotBe(entity2.Id);
            entity1.Id.Should().NotBe(entity3.Id);
            entity2.Id.Should().NotBe(entity3.Id);
        }

        [Fact]
        public void Notifications_ShouldBeNull_WhenNoEventsAdded()
        {
            // Arrange
            var entity = new TestEntity();

            // Assert
            entity.Notifications.Should().BeNull();
        }

        [Fact]
        public void AddEvent_ShouldAddEventToNotifications()
        {
            // Arrange
            var entity = new TestEntity();
            var testEvent = new TestEvent { Name = "Test" };

            // Act
            entity.AddEvent(testEvent);

            // Assert
            entity.Notifications.Should().NotBeNull();
            entity.Notifications.Should().HaveCount(1);
            entity.Notifications.Should().Contain(testEvent);
        }

        [Fact]
        public void AddEvent_ShouldAddMultipleEvents()
        {
            // Arrange
            var entity = new TestEntity();
            var event1 = new TestEvent { Name = "Event1" };
            var event2 = new TestEvent { Name = "Event2" };
            var event3 = new TestEvent { Name = "Event3" };

            // Act
            entity.AddEvent(event1);
            entity.AddEvent(event2);
            entity.AddEvent(event3);

            // Assert
            entity.Notifications.Should().HaveCount(3);
            entity.Notifications.Should().Contain(event1);
            entity.Notifications.Should().Contain(event2);
            entity.Notifications.Should().Contain(event3);
        }

        [Fact]
        public void RemoveEvent_ShouldRemoveEventFromNotifications()
        {
            // Arrange
            var entity = new TestEntity();
            var event1 = new TestEvent { Name = "Event1" };
            var event2 = new TestEvent { Name = "Event2" };
            entity.AddEvent(event1);
            entity.AddEvent(event2);

            // Act
            entity.RemoveEvent(event1);

            // Assert
            entity.Notifications.Should().HaveCount(1);
            entity.Notifications.Should().NotContain(event1);
            entity.Notifications.Should().Contain(event2);
        }

        [Fact]
        public void RemoveEvent_ShouldDoNothing_WhenEventDoesNotExist()
        {
            // Arrange
            var entity = new TestEntity();
            var event1 = new TestEvent { Name = "Event1" };
            var event2 = new TestEvent { Name = "Event2" };
            entity.AddEvent(event1);

            // Act
            entity.RemoveEvent(event2);

            // Assert
            entity.Notifications.Should().HaveCount(1);
            entity.Notifications.Should().Contain(event1);
        }

        [Fact]
        public void RemoveEvent_ShouldDoNothing_WhenNotificationsAreNull()
        {
            // Arrange
            var entity = new TestEntity();
            var testEvent = new TestEvent { Name = "Test" };

            // Act
            var act = () => entity.RemoveEvent(testEvent);

            // Assert
            act.Should().NotThrow();
            entity.Notifications.Should().BeNull();
        }

        [Fact]
        public void CleanEvents_ShouldClearAllNotifications()
        {
            // Arrange
            var entity = new TestEntity();
            entity.AddEvent(new TestEvent { Name = "Event1" });
            entity.AddEvent(new TestEvent { Name = "Event2" });
            entity.AddEvent(new TestEvent { Name = "Event3" });

            // Act
            entity.CleanEvents();

            // Assert
            entity.Notifications.Should().NotBeNull();
            entity.Notifications.Should().BeEmpty();
        }

        [Fact]
        public void CleanEvents_ShouldDoNothing_WhenNotificationsAreNull()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            var act = () => entity.CleanEvents();

            // Assert
            act.Should().NotThrow();
            entity.Notifications.Should().BeNull();
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenComparingWithSameReference()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            var result = entity.Equals(entity);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenComparingWithNull()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            var result = entity.Equals(null);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Equals_ShouldReturnTrue_WhenEntitiesHaveSameId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity1 = new TestEntity(id);
            var entity2 = new TestEntity(id);

            // Act
            var result = entity1.Equals(entity2);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Equals_ShouldReturnFalse_WhenEntitiesHaveDifferentIds()
        {
            // Arrange
            var entity1 = new TestEntity();
            var entity2 = new TestEntity();

            // Act
            var result = entity1.Equals(entity2);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void OperatorEquals_ShouldReturnTrue_WhenBothAreNull()
        {
            // Arrange
            TestEntity entity1 = null;
            TestEntity entity2 = null;

            // Act
            var result = entity1 == entity2;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void OperatorEquals_ShouldReturnFalse_WhenOneIsNull()
        {
            // Arrange
            var entity1 = new TestEntity();
            TestEntity entity2 = null;

            // Act
            var result1 = entity1 == entity2;
            var result2 = entity2 == entity1;

            // Assert
            result1.Should().BeFalse();
            result2.Should().BeFalse();
        }

        [Fact]
        public void OperatorEquals_ShouldReturnTrue_WhenEntitiesHaveSameId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity1 = new TestEntity(id);
            var entity2 = new TestEntity(id);

            // Act
            var result = entity1 == entity2;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void OperatorNotEquals_ShouldReturnFalse_WhenBothAreNull()
        {
            // Arrange
            TestEntity entity1 = null;
            TestEntity entity2 = null;

            // Act
            var result = entity1 != entity2;

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void OperatorNotEquals_ShouldReturnTrue_WhenEntitiesHaveDifferentIds()
        {
            // Arrange
            var entity1 = new TestEntity();
            var entity2 = new TestEntity();

            // Act
            var result = entity1 != entity2;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void GetHashCode_ShouldReturnSameValue_ForSameEntity()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            var hash1 = entity.GetHashCode();
            var hash2 = entity.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }

        [Fact]
        public void GetHashCode_ShouldReturnSameValue_ForEntitiesWithSameId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity1 = new TestEntity(id);
            var entity2 = new TestEntity(id);

            // Act
            var hash1 = entity1.GetHashCode();
            var hash2 = entity2.GetHashCode();

            // Assert
            hash1.Should().Be(hash2);
        }

        [Fact]
        public void GetHashCode_ShouldReturnDifferentValues_ForEntitiesWithDifferentIds()
        {
            // Arrange
            var entity1 = new TestEntity();
            var entity2 = new TestEntity();

            // Act
            var hash1 = entity1.GetHashCode();
            var hash2 = entity2.GetHashCode();

            // Assert
            hash1.Should().NotBe(hash2);
        }

        [Fact]
        public void ToString_ShouldReturnClassNameAndId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var entity = new TestEntity(id);

            // Act
            var result = entity.ToString();

            // Assert
            result.Should().Contain("TestEntity");
            result.Should().Contain(id.ToString());
            result.Should().MatchRegex(@"TestEntity \[Id=.*\]");
        }

        [Fact]
        public void Entity_ShouldHaveDeletedPropertyAsDefault()
        {
            // Arrange & Act
            var entity = new TestEntity();

            // Assert
            entity.Deleted.Should().BeFalse();
        }

        [Fact]
        public void Entity_ShouldAllowSettingDeletedProperty()
        {
            // Arrange
            var entity = new TestEntity();

            // Act
            entity.Deleted = true;

            // Assert
            entity.Deleted.Should().BeTrue();
        }

        [Fact]
        public void Notifications_ShouldBeReadOnly()
        {
            // Arrange
            var entity = new TestEntity();
            entity.AddEvent(new TestEvent { Name = "Test" });

            // Act
            var notifications = entity.Notifications;

            // Assert
            notifications.Should().BeAssignableTo<IReadOnlyCollection<Event>>();
        }

        [Fact]
        public void AddEvent_ShouldMaintainEventOrder()
        {
            // Arrange
            var entity = new TestEntity();
            var event1 = new TestEvent { Name = "First" };
            var event2 = new TestEvent { Name = "Second" };
            var event3 = new TestEvent { Name = "Third" };

            // Act
            entity.AddEvent(event1);
            entity.AddEvent(event2);
            entity.AddEvent(event3);

            // Assert
            var notificationsList = entity.Notifications.ToList();
            notificationsList[0].Should().Be(event1);
            notificationsList[1].Should().Be(event2);
            notificationsList[2].Should().Be(event3);
        }

        [Fact]
        public void CreatedDate_ShouldBeSettable()
        {
            // Arrange
            var entity = new TestEntity();
            var expectedDate = new DateTime(2023, 1, 15);

            // Act
            entity.CreatedDate = expectedDate;

            // Assert
            entity.CreatedDate.Should().Be(expectedDate);
        }

        [Fact]
        public void UpdatedDate_ShouldBeSettable()
        {
            // Arrange
            var entity = new TestEntity();
            var expectedDate = new DateTime(2023, 12, 31);

            // Act
            entity.UpdatedDate = expectedDate;

            // Assert
            entity.UpdatedDate.Should().Be(expectedDate);
        }
    }
}
