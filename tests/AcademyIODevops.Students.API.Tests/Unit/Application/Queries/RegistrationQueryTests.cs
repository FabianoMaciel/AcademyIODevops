using AcademyIODevops.Core.Enums;
using AcademyIODevops.Students.API.Application.Queries;
using AcademyIODevops.Students.API.Application.Queries.ViewModels;
using AcademyIODevops.Students.API.Models;
using AcademyIODevops.Students.API.Tests.Builders;
using FluentAssertions;
using Moq;

namespace AcademyIODevops.Students.API.Tests.Unit.Application.Queries
{
    public class RegistrationQueryTests
    {
        private readonly Mock<IRegistrationRepository> _repositoryMock;
        private readonly RegistrationQuery _query;

        public RegistrationQueryTests()
        {
            _repositoryMock = new Mock<IRegistrationRepository>();
            _query = new RegistrationQuery(_repositoryMock.Object);
        }

        #region GetByStudent Tests

        [Fact]
        public void GetByStudent_ShouldReturnRegistrations_WhenStudentHasRegistrations()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var registrations = new List<Registration>
            {
                new RegistrationBuilder().WithStudentId(studentId).Build(),
                new RegistrationBuilder().WithStudentId(studentId).Build(),
                new RegistrationBuilder().WithStudentId(studentId).Build()
            };

            _repositoryMock.Setup(x => x.GetRegistrationByStudent(studentId))
                .Returns(registrations);

            // Act
            var result = _query.GetByStudent(studentId);

            // Assert
            result.Should().HaveCount(3);
            result.Should().AllBeOfType<RegistrationViewModel>();
            result.Should().OnlyContain(r => r.StudentId == studentId);
        }

        [Fact]
        public void GetByStudent_ShouldReturnEmptyList_WhenStudentHasNoRegistrations()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var registrations = new List<Registration>();

            _repositoryMock.Setup(x => x.GetRegistrationByStudent(studentId))
                .Returns(registrations);

            // Act
            var result = _query.GetByStudent(studentId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetByStudent_ShouldMapAllProperties_ToViewModel()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var registrationTime = DateTime.UtcNow;
            var registrationId = Guid.NewGuid();

            var registration = new RegistrationBuilder()
                .WithId(registrationId)
                .WithStudentId(studentId)
                .WithCourseId(courseId)
                .WithRegistrationTime(registrationTime)
                .WithStatus(EProgressLesson.InProgress)
                .Build();

            _repositoryMock.Setup(x => x.GetRegistrationByStudent(studentId))
                .Returns(new List<Registration> { registration });

            // Act
            var result = _query.GetByStudent(studentId);

            // Assert
            result.Should().HaveCount(1);
            var viewModel = result.First();
            viewModel.Id.Should().Be(registrationId);
            viewModel.StudentId.Should().Be(studentId);
            viewModel.CourseId.Should().Be(courseId);
            viewModel.RegistrationTime.Should().Be(registrationTime);
            viewModel.Status.Should().Be(EProgressLesson.InProgress);
        }

        [Fact]
        public void GetByStudent_ShouldReturnCorrectCount_WhenMultipleRegistrationsExist()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var registrations = RegistrationBuilder.BuildMany(5, studentId);

            _repositoryMock.Setup(x => x.GetRegistrationByStudent(studentId))
                .Returns(registrations);

            // Act
            var result = _query.GetByStudent(studentId);

            // Assert
            result.Should().HaveCount(5);
        }

        [Fact]
        public void GetByStudent_ShouldOnlyReturnRegistrationsForSpecificStudent()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var otherStudentId = Guid.NewGuid();

            // Apenas as registrations do student correto devem ser retornadas pelo repository
            var registrations = new List<Registration>
            {
                new RegistrationBuilder().WithStudentId(studentId).Build(),
                new RegistrationBuilder().WithStudentId(studentId).Build()
            };

            _repositoryMock.Setup(x => x.GetRegistrationByStudent(studentId))
                .Returns(registrations);

            // Act
            var result = _query.GetByStudent(studentId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(r => r.StudentId == studentId);
            result.Should().NotContain(r => r.StudentId == otherStudentId);
        }

        [Fact]
        public void GetByStudent_ShouldPreserveStatus_WhenMapping()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var registrations = RegistrationBuilder.BuildWithDifferentStatuses(studentId);

            _repositoryMock.Setup(x => x.GetRegistrationByStudent(studentId))
                .Returns(registrations);

            // Act
            var result = _query.GetByStudent(studentId);

            // Assert
            result.Should().HaveCount(3);
            result.Should().Contain(r => r.Status == EProgressLesson.NotStarted);
            result.Should().Contain(r => r.Status == EProgressLesson.InProgress);
            result.Should().Contain(r => r.Status == EProgressLesson.Completed);
        }

        #endregion

        #region GetAllRegistrations Tests

        [Fact]
        public void GetAllRegistrations_ShouldReturnAllRegistrations_WhenMultipleExist()
        {
            // Arrange
            var student1Id = Guid.NewGuid();
            var student2Id = Guid.NewGuid();
            var student3Id = Guid.NewGuid();

            var registrations = new List<Registration>();
            registrations.AddRange(RegistrationBuilder.BuildMany(3, student1Id));
            registrations.AddRange(RegistrationBuilder.BuildMany(4, student2Id));
            registrations.AddRange(RegistrationBuilder.BuildMany(3, student3Id));

            _repositoryMock.Setup(x => x.GetAllRegistrations())
                .Returns(registrations);

            // Act
            var result = _query.GetAllRegistrations();

            // Assert
            result.Should().HaveCount(10);
            result.Should().AllBeOfType<RegistrationViewModel>();
        }

        [Fact]
        public void GetAllRegistrations_ShouldReturnEmptyList_WhenNoRegistrationsExist()
        {
            // Arrange
            var registrations = new List<Registration>();

            _repositoryMock.Setup(x => x.GetAllRegistrations())
                .Returns(registrations);

            // Act
            var result = _query.GetAllRegistrations();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAllRegistrations_ShouldMapAllProperties_ToViewModel()
        {
            // Arrange
            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var registrationTime = DateTime.UtcNow;
            var registrationId = Guid.NewGuid();

            var registration = new RegistrationBuilder()
                .WithId(registrationId)
                .WithStudentId(studentId)
                .WithCourseId(courseId)
                .WithRegistrationTime(registrationTime)
                .WithStatus(EProgressLesson.Completed)
                .Build();

            _repositoryMock.Setup(x => x.GetAllRegistrations())
                .Returns(new List<Registration> { registration });

            // Act
            var result = _query.GetAllRegistrations();

            // Assert
            result.Should().HaveCount(1);
            var viewModel = result.First();
            viewModel.Id.Should().Be(registrationId);
            viewModel.StudentId.Should().Be(studentId);
            viewModel.CourseId.Should().Be(courseId);
            viewModel.RegistrationTime.Should().Be(registrationTime);
            viewModel.Status.Should().Be(EProgressLesson.Completed);
        }

        [Fact]
        public void GetAllRegistrations_ShouldIncludeRegistrationsFromDifferentStudents()
        {
            // Arrange
            var student1Id = Guid.NewGuid();
            var student2Id = Guid.NewGuid();
            var student3Id = Guid.NewGuid();

            var registrations = new List<Registration>
            {
                new RegistrationBuilder().WithStudentId(student1Id).Build(),
                new RegistrationBuilder().WithStudentId(student2Id).Build(),
                new RegistrationBuilder().WithStudentId(student3Id).Build()
            };

            _repositoryMock.Setup(x => x.GetAllRegistrations())
                .Returns(registrations);

            // Act
            var result = _query.GetAllRegistrations();

            // Assert
            result.Should().HaveCount(3);
            result.Select(r => r.StudentId).Distinct().Should().HaveCount(3);
            result.Should().Contain(r => r.StudentId == student1Id);
            result.Should().Contain(r => r.StudentId == student2Id);
            result.Should().Contain(r => r.StudentId == student3Id);
        }

        [Fact]
        public void GetAllRegistrations_ShouldCallRepository_ExactlyOnce()
        {
            // Arrange
            var registrations = new List<Registration>
            {
                new RegistrationBuilder().Build()
            };

            _repositoryMock.Setup(x => x.GetAllRegistrations())
                .Returns(registrations);

            // Act
            var result = _query.GetAllRegistrations();

            // Assert
            _repositoryMock.Verify(x => x.GetAllRegistrations(), Times.Once);
        }

        [Fact]
        public void GetAllRegistrations_ShouldReturnCorrectViewModelStructure()
        {
            // Arrange
            var registrations = new List<Registration>
            {
                new RegistrationBuilder().Build()
            };

            _repositoryMock.Setup(x => x.GetAllRegistrations())
                .Returns(registrations);

            // Act
            var result = _query.GetAllRegistrations();

            // Assert
            result.Should().HaveCount(1);
            var viewModel = result.First();
            viewModel.Should().BeOfType<RegistrationViewModel>();
            viewModel.Id.Should().NotBeEmpty();
            viewModel.StudentId.Should().NotBeEmpty();
            viewModel.CourseId.Should().NotBeEmpty();
            viewModel.RegistrationTime.Should().NotBe(default(DateTime));
        }

        #endregion
    }
}
