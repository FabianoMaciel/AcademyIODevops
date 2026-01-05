using AcademyIODevops.Students.API.Models;
using AcademyIODevops.Students.API.Tests.Builders;

namespace AcademyIODevops.Students.API.Tests.Fixtures
{
    /// <summary>
    /// Fixture para compartilhar contexto e dados de teste entre testes.
    /// Implementa IDisposable para limpeza de recursos.
    /// </summary>
    public class StudentTestFixture : IDisposable
    {
        /// <summary>
        /// Estudantes de exemplo prontos para uso em testes
        /// </summary>
        public List<StudentUser> SampleStudents { get; private set; } = new();

        /// <summary>
        /// Matrículas de exemplo prontas para uso em testes
        /// </summary>
        public List<Registration> SampleRegistrations { get; private set; } = new();

        /// <summary>
        /// IDs pré-definidos para facilitar testes
        /// </summary>
        public Guid AdminUserId { get; private set; }
        public Guid RegularStudent1Id { get; private set; }
        public Guid RegularStudent2Id { get; private set; }
        public Guid YoungStudentId { get; private set; }

        public Guid Course1Id { get; private set; }
        public Guid Course2Id { get; private set; }
        public Guid Course3Id { get; private set; }

        public StudentTestFixture()
        {
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            // Definir IDs fixos
            AdminUserId = Guid.NewGuid();
            RegularStudent1Id = Guid.NewGuid();
            RegularStudent2Id = Guid.NewGuid();
            YoungStudentId = Guid.NewGuid();

            Course1Id = Guid.NewGuid();
            Course2Id = Guid.NewGuid();
            Course3Id = Guid.NewGuid();

            // Criar estudantes de exemplo
            SampleStudents = new List<StudentUser>
            {
                new StudentUserBuilder()
                    .WithId(AdminUserId)
                    .AsAdmin()
                    .Build(),

                new StudentUserBuilder()
                    .WithId(RegularStudent1Id)
                    .WithUserName("john.doe")
                    .WithFirstName("John")
                    .WithLastName("Doe")
                    .WithEmail("john.doe@academyio.com")
                    .AsAdult()
                    .Build(),

                new StudentUserBuilder()
                    .WithId(RegularStudent2Id)
                    .WithUserName("jane.smith")
                    .WithFirstName("Jane")
                    .WithLastName("Smith")
                    .WithEmail("jane.smith@academyio.com")
                    .AsAdult()
                    .Build(),

                new StudentUserBuilder()
                    .WithId(YoungStudentId)
                    .WithUserName("young.student")
                    .WithFirstName("Young")
                    .WithLastName("Student")
                    .WithEmail("young@academyio.com")
                    .AsMinor()
                    .Build()
            };

            // Criar matrículas de exemplo
            SampleRegistrations = new List<Registration>
            {
                // John Doe tem 2 cursos
                new RegistrationBuilder()
                    .WithStudentId(RegularStudent1Id)
                    .WithCourseId(Course1Id)
                    .AsInProgress()
                    .Build(),

                new RegistrationBuilder()
                    .WithStudentId(RegularStudent1Id)
                    .WithCourseId(Course2Id)
                    .AsCompleted()
                    .Build(),

                // Jane Smith tem 1 curso
                new RegistrationBuilder()
                    .WithStudentId(RegularStudent2Id)
                    .WithCourseId(Course1Id)
                    .AsNotStarted()
                    .Build()
            };
        }

        /// <summary>
        /// Cria um estudante limpo para cada teste
        /// </summary>
        public StudentUser CreateFreshStudent()
        {
            return new StudentUserBuilder().Build();
        }

        /// <summary>
        /// Cria uma matrícula limpa para cada teste
        /// </summary>
        public Registration CreateFreshRegistration(Guid? studentId = null, Guid? courseId = null)
        {
            return new RegistrationBuilder()
                .WithStudentId(studentId ?? Guid.NewGuid())
                .WithCourseId(courseId ?? Guid.NewGuid())
                .Build();
        }

        /// <summary>
        /// Obtém um estudante de exemplo por tipo
        /// </summary>
        public StudentUser GetAdminUser() =>
            SampleStudents.First(s => s.Id == AdminUserId);

        public StudentUser GetRegularStudent1() =>
            SampleStudents.First(s => s.Id == RegularStudent1Id);

        public StudentUser GetRegularStudent2() =>
            SampleStudents.First(s => s.Id == RegularStudent2Id);

        public StudentUser GetYoungStudent() =>
            SampleStudents.First(s => s.Id == YoungStudentId);

        /// <summary>
        /// Obtém matrículas de um estudante específico
        /// </summary>
        public List<Registration> GetRegistrationsByStudent(Guid studentId) =>
            SampleRegistrations.Where(r => r.StudentId == studentId).ToList();

        public void Dispose()
        {
            // Limpar recursos se necessário
            SampleStudents?.Clear();
            SampleRegistrations?.Clear();
        }
    }

    /// <summary>
    /// Collection definition para compartilhar fixture entre classes de teste
    /// </summary>
    [CollectionDefinition("Student Collection")]
    public class StudentCollection : ICollectionFixture<StudentTestFixture>
    {
        // Esta classe não tem código, serve apenas para definir a collection
    }
}
