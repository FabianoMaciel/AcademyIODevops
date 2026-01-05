using AcademyIODevops.Students.API.Data;
using AcademyIODevops.Students.API.Data.Repository;
using AcademyIODevops.Students.API.Tests.Builders;
using AcademyIODevops.Students.API.Tests.Fixtures;
using FluentAssertions;

namespace AcademyIODevops.Students.API.Tests.Unit.Data
{
    public class UserRepositoryTests
    {
        private readonly RepositoryTestFixture _fixture;

        public UserRepositoryTests()
        {
            _fixture = new RepositoryTestFixture();
        }

        [Fact]
        public async Task Add_ShouldAddUser_ToDatabase()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new UserRepository(context);

            var user = new StudentUserBuilder().Build();

            // Act
            repository.Add(user);
            await context.SaveChangesAsync();

            // Assert
            var userInDb = await context.StudentUsers.FindAsync(user.Id);
            userInDb.Should().NotBeNull();
            userInDb.Id.Should().Be(user.Id);
            userInDb.Email.Should().Be(user.Email);
        }

        [Fact]
        public async Task Add_ShouldNotPersist_UntilSaveChangesIsCalled()
        {
            // Arrange
            var databaseName = Guid.NewGuid().ToString();
            using var context1 = _fixture.CreateContext(databaseName);
            var repository = new UserRepository(context1);

            var user = new StudentUserBuilder().Build();

            // Act
            repository.Add(user);

            // Assert - ao criar um novo contexto, a entidade não deve estar lá ainda
            using var context2 = _fixture.CreateContext(databaseName);
            var userInDb = await context2.StudentUsers.FindAsync(user.Id);
            userInDb.Should().BeNull();

            // Mas após SaveChanges, deve estar disponível em um novo contexto
            await context1.SaveChangesAsync();
            using var context3 = _fixture.CreateContext(databaseName);
            var userAfterSave = await context3.StudentUsers.FindAsync(user.Id);
            userAfterSave.Should().NotBeNull();
        }

        [Fact]
        public async Task Add_ShouldAllowMultipleUsers_WithDifferentEmails()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new UserRepository(context);

            var user1 = new StudentUserBuilder().WithEmail("user1@test.com").Build();
            var user2 = new StudentUserBuilder().WithEmail("user2@test.com").Build();
            var user3 = new StudentUserBuilder().WithEmail("user3@test.com").Build();

            // Act
            repository.Add(user1);
            repository.Add(user2);
            repository.Add(user3);
            await context.SaveChangesAsync();

            // Assert
            var usersInDb = context.StudentUsers.ToList();
            usersInDb.Should().HaveCount(3);
            usersInDb.Should().Contain(u => u.Email == "user1@test.com");
            usersInDb.Should().Contain(u => u.Email == "user2@test.com");
            usersInDb.Should().Contain(u => u.Email == "user3@test.com");
        }

        [Fact]
        public async Task GetById_ShouldReturnUser_WhenIdExists()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new UserRepository(context);

            var user = new StudentUserBuilder().Build();
            _fixture.SeedDatabase(context, user);

            // Act
            var result = await repository.GetById(user.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user.Id);
            result.Email.Should().Be(user.Email);
            result.UserName.Should().Be(user.UserName);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenIdDoesNotExist()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new UserRepository(context);

            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await repository.GetById(nonExistentId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmail_ShouldReturnUser_WhenEmailExists()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new UserRepository(context);

            var email = "test@example.com";
            var user = new StudentUserBuilder().WithEmail(email).Build();
            _fixture.SeedDatabase(context, user);

            // Act
            var result = await repository.GetByEmail(email);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(email);
            result.Id.Should().Be(user.Id);
        }

        [Fact]
        public async Task GetByEmail_ShouldReturnNull_WhenEmailDoesNotExist()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new UserRepository(context);

            var nonExistentEmail = "nonexistent@example.com";

            // Act
            var result = await repository.GetByEmail(nonExistentEmail);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetByEmail_ShouldReturnCorrectUser_WhenMultipleUsersExist()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new UserRepository(context);

            var user1 = new StudentUserBuilder().WithEmail("user1@test.com").Build();
            var user2 = new StudentUserBuilder().WithEmail("user2@test.com").Build();
            var user3 = new StudentUserBuilder().WithEmail("user3@test.com").Build();
            var user4 = new StudentUserBuilder().WithEmail("user4@test.com").Build();
            var user5 = new StudentUserBuilder().WithEmail("user5@test.com").Build();

            _fixture.SeedDatabase(context, user1, user2, user3, user4, user5);

            // Act
            var result = await repository.GetByEmail("user3@test.com");

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(user3.Id);
            result.Email.Should().Be("user3@test.com");
        }

        [Fact]
        public void UnitOfWork_ShouldReturnStudentsContext()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new UserRepository(context);

            // Act
            var unitOfWork = repository.UnitOfWork;

            // Assert
            unitOfWork.Should().NotBeNull();
            unitOfWork.Should().BeSameAs(context);
        }

        [Fact]
        public async Task Commit_ShouldPersistChanges_ToDatabase()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new UserRepository(context);

            var user = new StudentUserBuilder().Build();

            // Act
            repository.Add(user);
            var commitResult = await repository.UnitOfWork.Commit();

            // Assert
            commitResult.Should().BeTrue();

            var userInDb = await context.StudentUsers.FindAsync(user.Id);
            userInDb.Should().NotBeNull();
            userInDb.Id.Should().Be(user.Id);
        }
    }
}
