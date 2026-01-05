using AcademyIODevops.WebAPI.Core.DatabaseFlavor;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace AcademyIODevops.WebAPI.Core.Tests.DatabaseFlavor
{
    public class ProviderSelectorTests
    {
        private class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
            {
            }
        }

        [Fact]
        public void ConfigureProviderForContext_ShouldConfigureSqlServer_WhenDatabaseTypeIsSqlServer()
        {
            // Arrange
            var services = new ServiceCollection();
            var connectionString = "Server=localhost;Database=TestDb;";
            var options = (DatabaseType.SqlServer, connectionString);

            // Act
            var result = services.ConfigureProviderForContext<TestDbContext>(options);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(services);

            var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetService<TestDbContext>();

            // Verify that the context was registered
            services.Should().Contain(s => s.ServiceType == typeof(TestDbContext));
        }

        [Fact]
        public void ConfigureProviderForContext_ShouldConfigureSqlite_WhenDatabaseTypeIsSqlite()
        {
            // Arrange
            var services = new ServiceCollection();
            var connectionString = "Data Source=test.db";
            var options = (DatabaseType.Sqlite, connectionString);

            // Act
            var result = services.ConfigureProviderForContext<TestDbContext>(options);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(services);

            // Verify that the context was registered
            services.Should().Contain(s => s.ServiceType == typeof(TestDbContext));
        }

        [Fact]
        public void ConfigureProviderForContext_ShouldThrowArgumentOutOfRangeException_WhenDatabaseTypeIsNone()
        {
            // Arrange
            var services = new ServiceCollection();
            var connectionString = "Server=localhost;Database=TestDb;";
            var options = (DatabaseType.None, connectionString);

            // Act & Assert
            var act = () => services.ConfigureProviderForContext<TestDbContext>(options);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("database")
                .And.ActualValue.Should().Be(DatabaseType.None);
        }

        [Fact]
        public void ConfigureProviderForContext_ShouldPassMigrationAssembly_WhenProvided()
        {
            // Arrange
            var services = new ServiceCollection();
            var connectionString = "Server=localhost;Database=TestDb;";
            var options = (DatabaseType.SqlServer, connectionString);
            var migrationAssembly = "CustomMigrationAssembly";

            // Act
            var result = services.ConfigureProviderForContext<TestDbContext>(options, migrationAssembly);

            // Assert
            result.Should().NotBeNull();
            services.Should().Contain(s => s.ServiceType == typeof(TestDbContext));
        }

        [Fact]
        public void ConfigureProviderForContext_ShouldWorkWithoutMigrationAssembly()
        {
            // Arrange
            var services = new ServiceCollection();
            var connectionString = "Server=localhost;Database=TestDb;";
            var options = (DatabaseType.SqlServer, connectionString);

            // Act
            var result = services.ConfigureProviderForContext<TestDbContext>(options);

            // Assert
            result.Should().NotBeNull();
            services.Should().Contain(s => s.ServiceType == typeof(TestDbContext));
        }

        [Fact]
        public void WithProviderAutoSelection_ShouldReturnSqlServerAction_WhenDatabaseTypeIsSqlServer()
        {
            // Arrange
            var connectionString = "Server=localhost;Database=TestDb;";
            var options = (DatabaseType.SqlServer, connectionString);

            // Act
            var action = ProviderSelector.WithProviderAutoSelection(options);

            // Assert
            action.Should().NotBeNull();

            // Verify that the action can be invoked without throwing
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            var act = () => action(optionsBuilder);
            act.Should().NotThrow();
        }

        [Fact]
        public void WithProviderAutoSelection_ShouldReturnSqliteAction_WhenDatabaseTypeIsSqlite()
        {
            // Arrange
            var connectionString = "Data Source=test.db";
            var options = (DatabaseType.Sqlite, connectionString);

            // Act
            var action = ProviderSelector.WithProviderAutoSelection(options);

            // Assert
            action.Should().NotBeNull();

            // Verify that the action can be invoked without throwing
            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            var act = () => action(optionsBuilder);
            act.Should().NotThrow();
        }

        [Fact]
        public void WithProviderAutoSelection_ShouldThrowArgumentOutOfRangeException_WhenDatabaseTypeIsNone()
        {
            // Arrange
            var connectionString = "Server=localhost;Database=TestDb;";
            var options = (DatabaseType.None, connectionString);

            // Act & Assert
            var act = () => ProviderSelector.WithProviderAutoSelection(options);

            act.Should().Throw<ArgumentOutOfRangeException>()
                .WithParameterName("database")
                .And.ActualValue.Should().Be(DatabaseType.None);
        }

        [Theory]
        [InlineData("Server=localhost;Database=TestDb1;")]
        [InlineData("Server=localhost;Database=TestDb2;Integrated Security=true;")]
        [InlineData("Server=192.168.1.1;Database=TestDb3;User Id=sa;Password=pass;")]
        public void ConfigureProviderForContext_ShouldAcceptDifferentConnectionStrings_ForSqlServer(string connectionString)
        {
            // Arrange
            var services = new ServiceCollection();
            var options = (DatabaseType.SqlServer, connectionString);

            // Act
            var result = services.ConfigureProviderForContext<TestDbContext>(options);

            // Assert
            result.Should().NotBeNull();
            services.Should().Contain(s => s.ServiceType == typeof(TestDbContext));
        }

        [Theory]
        [InlineData("Data Source=test1.db")]
        [InlineData("Data Source=:memory:")]
        [InlineData("Data Source=./data/test.db")]
        public void ConfigureProviderForContext_ShouldAcceptDifferentConnectionStrings_ForSqlite(string connectionString)
        {
            // Arrange
            var services = new ServiceCollection();
            var options = (DatabaseType.Sqlite, connectionString);

            // Act
            var result = services.ConfigureProviderForContext<TestDbContext>(options);

            // Assert
            result.Should().NotBeNull();
            services.Should().Contain(s => s.ServiceType == typeof(TestDbContext));
        }

        [Theory]
        [InlineData("Server=localhost;Database=TestDb;")]
        [InlineData("Server=192.168.1.1;Database=TestDb;User Id=sa;Password=pass;")]
        public void WithProviderAutoSelection_ShouldAcceptDifferentConnectionStrings_ForSqlServer(string connectionString)
        {
            // Arrange
            var options = (DatabaseType.SqlServer, connectionString);

            // Act
            var action = ProviderSelector.WithProviderAutoSelection(options);

            // Assert
            action.Should().NotBeNull();

            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            var act = () => action(optionsBuilder);
            act.Should().NotThrow();
        }

        [Theory]
        [InlineData("Data Source=test.db")]
        [InlineData("Data Source=:memory:")]
        public void WithProviderAutoSelection_ShouldAcceptDifferentConnectionStrings_ForSqlite(string connectionString)
        {
            // Arrange
            var options = (DatabaseType.Sqlite, connectionString);

            // Act
            var action = ProviderSelector.WithProviderAutoSelection(options);

            // Assert
            action.Should().NotBeNull();

            var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
            var act = () => action(optionsBuilder);
            act.Should().NotThrow();
        }

        [Fact]
        public void ConfigureProviderForContext_ShouldReturnSameServiceCollection_ThatWasPassedIn()
        {
            // Arrange
            var services = new ServiceCollection();
            var connectionString = "Server=localhost;Database=TestDb;";
            var options = (DatabaseType.SqlServer, connectionString);

            // Act
            var result = services.ConfigureProviderForContext<TestDbContext>(options);

            // Assert
            result.Should().BeSameAs(services, "the method should return the same IServiceCollection instance for fluent chaining");
        }

        [Fact]
        public void ConfigureProviderForContext_ShouldAllowMultipleContextRegistrations()
        {
            // Arrange
            var services = new ServiceCollection();
            var connectionString1 = "Server=localhost;Database=TestDb1;";
            var connectionString2 = "Server=localhost;Database=TestDb2;";
            var options1 = (DatabaseType.SqlServer, connectionString1);
            var options2 = (DatabaseType.SqlServer, connectionString2);

            // Act
            services.ConfigureProviderForContext<TestDbContext>(options1);
            var act = () => services.ConfigureProviderForContext<TestDbContext>(options2);

            // Assert - should not throw, allowing multiple registrations
            act.Should().NotThrow();
        }
    }
}
