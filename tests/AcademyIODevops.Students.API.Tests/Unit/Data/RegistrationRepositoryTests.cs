using AcademyIODevops.Core.Enums;
using AcademyIODevops.Students.API.Data;
using AcademyIODevops.Students.API.Data.Repository;
using AcademyIODevops.Students.API.Tests.Builders;
using AcademyIODevops.Students.API.Tests.Fixtures;
using FluentAssertions;

namespace AcademyIODevops.Students.API.Tests.Unit.Data
{
    public class RegistrationRepositoryTests
    {
        private readonly RepositoryTestFixture _fixture;

        public RegistrationRepositoryTests()
        {
            _fixture = new RepositoryTestFixture();
        }

        #region AddRegistration Tests

        [Fact]
        public void AddRegistration_ShouldAddRegistration_ToDatabase()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new RegistrationRepository(context);

            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            // Act
            var registration = repository.AddRegistration(studentId, courseId);
            context.SaveChanges();

            // Assert
            registration.Should().NotBeNull();
            registration.StudentId.Should().Be(studentId);
            registration.CourseId.Should().Be(courseId);

            var registrationInDb = context.Registrations.Find(registration.Id);
            registrationInDb.Should().NotBeNull();
            registrationInDb.StudentId.Should().Be(studentId);
            registrationInDb.CourseId.Should().Be(courseId);
        }

        [Fact]
        public void AddRegistration_ShouldThrowException_WhenRegistrationAlreadyExists()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new RegistrationRepository(context);

            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            // Primeira matrícula
            repository.AddRegistration(studentId, courseId);
            context.SaveChanges();

            // Act - tentar adicionar matrícula duplicada
            Action act = () => repository.AddRegistration(studentId, courseId);

            // Assert
            act.Should().Throw<Exception>()
                .WithMessage("Matrícula já existente.");
        }

        [Fact]
        public void AddRegistration_ShouldReturnRegistration_WithGeneratedId()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new RegistrationRepository(context);

            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            // Act
            var registration = repository.AddRegistration(studentId, courseId);

            // Assert
            registration.Should().NotBeNull();
            registration.Id.Should().NotBeEmpty();
            registration.Id.Should().NotBe(Guid.Empty);
        }

        [Fact]
        public void AddRegistration_ShouldAllowSameStudent_DifferentCourses()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new RegistrationRepository(context);

            var studentId = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var courseId2 = Guid.NewGuid();
            var courseId3 = Guid.NewGuid();

            // Act
            var registration1 = repository.AddRegistration(studentId, courseId1);
            var registration2 = repository.AddRegistration(studentId, courseId2);
            var registration3 = repository.AddRegistration(studentId, courseId3);
            context.SaveChanges();

            // Assert
            registration1.Should().NotBeNull();
            registration2.Should().NotBeNull();
            registration3.Should().NotBeNull();

            var registrationsInDb = context.Registrations
                .Where(r => r.StudentId == studentId)
                .ToList();

            registrationsInDb.Should().HaveCount(3);
            registrationsInDb.Should().Contain(r => r.CourseId == courseId1);
            registrationsInDb.Should().Contain(r => r.CourseId == courseId2);
            registrationsInDb.Should().Contain(r => r.CourseId == courseId3);
        }

        [Fact]
        public void AddRegistration_ShouldAllowDifferentStudents_SameCourse()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new RegistrationRepository(context);

            var studentId1 = Guid.NewGuid();
            var studentId2 = Guid.NewGuid();
            var studentId3 = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            // Act
            var registration1 = repository.AddRegistration(studentId1, courseId);
            var registration2 = repository.AddRegistration(studentId2, courseId);
            var registration3 = repository.AddRegistration(studentId3, courseId);
            context.SaveChanges();

            // Assert
            registration1.Should().NotBeNull();
            registration2.Should().NotBeNull();
            registration3.Should().NotBeNull();

            var registrationsInDb = context.Registrations
                .Where(r => r.CourseId == courseId)
                .ToList();

            registrationsInDb.Should().HaveCount(3);
            registrationsInDb.Should().Contain(r => r.StudentId == studentId1);
            registrationsInDb.Should().Contain(r => r.StudentId == studentId2);
            registrationsInDb.Should().Contain(r => r.StudentId == studentId3);
        }

        #endregion

        #region GetRegistrationByStudent Tests

        [Fact]
        public void GetRegistrationByStudent_ShouldReturnRegistrations_ForSpecificStudent()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new RegistrationRepository(context);

            var student1Id = Guid.NewGuid();
            var student2Id = Guid.NewGuid();
            var student3Id = Guid.NewGuid();

            var registrations1 = RegistrationBuilder.BuildMany(3, student1Id);
            var registrations2 = RegistrationBuilder.BuildMany(2, student2Id);
            var registrations3 = RegistrationBuilder.BuildMany(4, student3Id);

            _fixture.SeedDatabase(context, registrations1.Concat(registrations2).Concat(registrations3).ToArray());

            // Act
            var result = repository.GetRegistrationByStudent(student1Id);

            // Assert
            result.Should().HaveCount(3);
            result.Should().OnlyContain(r => r.StudentId == student1Id);
            result.Should().NotContain(r => r.StudentId == student2Id);
            result.Should().NotContain(r => r.StudentId == student3Id);
        }

        [Fact]
        public void GetRegistrationByStudent_ShouldReturnEmptyList_WhenStudentHasNoRegistrations()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new RegistrationRepository(context);

            var studentId = Guid.NewGuid();
            var otherStudentId = Guid.NewGuid();

            // Adiciona registrations apenas para outro student
            var registrations = RegistrationBuilder.BuildMany(3, otherStudentId);
            _fixture.SeedDatabase(context, registrations.ToArray());

            // Act
            var result = repository.GetRegistrationByStudent(studentId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetRegistrationByStudent_ShouldNotReturnRegistrations_FromOtherStudents()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new RegistrationRepository(context);

            var targetStudentId = Guid.NewGuid();
            var otherStudent1Id = Guid.NewGuid();
            var otherStudent2Id = Guid.NewGuid();

            var targetRegistrations = RegistrationBuilder.BuildMany(2, targetStudentId);
            var otherRegistrations1 = RegistrationBuilder.BuildMany(5, otherStudent1Id);
            var otherRegistrations2 = RegistrationBuilder.BuildMany(3, otherStudent2Id);

            _fixture.SeedDatabase(context,
                targetRegistrations.Concat(otherRegistrations1).Concat(otherRegistrations2).ToArray());

            // Act
            var result = repository.GetRegistrationByStudent(targetStudentId);

            // Assert
            result.Should().HaveCount(2);
            result.Should().OnlyContain(r => r.StudentId == targetStudentId);
        }

        #endregion

        #region GetAllRegistrations Tests

        [Fact]
        public void GetAllRegistrations_ShouldReturnAllRegistrations_FromDatabase()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new RegistrationRepository(context);

            var student1Id = Guid.NewGuid();
            var student2Id = Guid.NewGuid();
            var student3Id = Guid.NewGuid();

            var registrations1 = RegistrationBuilder.BuildMany(3, student1Id);
            var registrations2 = RegistrationBuilder.BuildMany(4, student2Id);
            var registrations3 = RegistrationBuilder.BuildMany(2, student3Id);

            _fixture.SeedDatabase(context, registrations1.Concat(registrations2).Concat(registrations3).ToArray());

            // Act
            var result = repository.GetAllRegistrations();

            // Assert
            result.Should().HaveCount(9);
            result.Should().Contain(r => r.StudentId == student1Id);
            result.Should().Contain(r => r.StudentId == student2Id);
            result.Should().Contain(r => r.StudentId == student3Id);
        }

        [Fact]
        public void GetAllRegistrations_ShouldReturnEmptyList_WhenNoRegistrationsExist()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new RegistrationRepository(context);

            // Act
            var result = repository.GetAllRegistrations();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region FinishCourse Tests

        [Fact]
        public async Task FinishCourse_ShouldUpdateStatus_ToCompleted()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new RegistrationRepository(context);

            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var registration = new RegistrationBuilder()
                .WithStudentId(studentId)
                .WithCourseId(courseId)
                .WithStatus(EProgressLesson.InProgress)
                .Build();

            _fixture.SeedDatabase(context, registration);

            // Act
            var result = await repository.FinishCourse(studentId, courseId);
            await context.SaveChangesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Status.Should().Be(EProgressLesson.Completed);

            var registrationInDb = await context.Registrations.FindAsync(registration.Id);
            registrationInDb.Status.Should().Be(EProgressLesson.Completed);
        }

        [Fact]
        public async Task FinishCourse_ShouldReturnNull_WhenRegistrationDoesNotExist()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new RegistrationRepository(context);

            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            // Act
            var result = await repository.FinishCourse(studentId, courseId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task FinishCourse_ShouldOnlyUpdateSpecificRegistration()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new RegistrationRepository(context);

            var studentId = Guid.NewGuid();
            var courseId1 = Guid.NewGuid();
            var courseId2 = Guid.NewGuid();
            var courseId3 = Guid.NewGuid();

            var registration1 = new RegistrationBuilder()
                .WithStudentId(studentId)
                .WithCourseId(courseId1)
                .WithStatus(EProgressLesson.InProgress)
                .Build();

            var registration2 = new RegistrationBuilder()
                .WithStudentId(studentId)
                .WithCourseId(courseId2)
                .WithStatus(EProgressLesson.InProgress)
                .Build();

            var registration3 = new RegistrationBuilder()
                .WithStudentId(studentId)
                .WithCourseId(courseId3)
                .WithStatus(EProgressLesson.NotStarted)
                .Build();

            _fixture.SeedDatabase(context, registration1, registration2, registration3);

            // Act
            var result = await repository.FinishCourse(studentId, courseId1);
            await context.SaveChangesAsync();

            // Assert
            result.Should().NotBeNull();
            result.CourseId.Should().Be(courseId1);
            result.Status.Should().Be(EProgressLesson.Completed);

            var reg1InDb = await context.Registrations.FindAsync(registration1.Id);
            var reg2InDb = await context.Registrations.FindAsync(registration2.Id);
            var reg3InDb = await context.Registrations.FindAsync(registration3.Id);

            reg1InDb.Status.Should().Be(EProgressLesson.Completed);
            reg2InDb.Status.Should().Be(EProgressLesson.InProgress); // não mudou
            reg3InDb.Status.Should().Be(EProgressLesson.NotStarted); // não mudou
        }

        [Fact]
        public async Task FinishCourse_ShouldNotCreateNewRegistration()
        {
            // Arrange
            using var context = _fixture.CreateContext();
            var repository = new RegistrationRepository(context);

            var studentId = Guid.NewGuid();
            var courseId = Guid.NewGuid();

            var initialCount = context.Registrations.Count();

            // Act
            var result = await repository.FinishCourse(studentId, courseId);
            await context.SaveChangesAsync();

            // Assert
            result.Should().BeNull();

            var finalCount = context.Registrations.Count();
            finalCount.Should().Be(initialCount);
        }

        #endregion
    }
}
