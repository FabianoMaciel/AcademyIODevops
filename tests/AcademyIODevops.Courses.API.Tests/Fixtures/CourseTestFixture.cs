using AcademyIODevops.Courses.API.Tests.Builders;
using AcademyIODevops.Courses.API.Models;

namespace AcademyIODevops.Courses.API.Tests.Fixtures
{
    /// <summary>
    /// Fixture para compartilhar contexto e dados de teste entre testes.
    /// Implementa IDisposable para limpeza de recursos.
    /// </summary>
    public class CourseTestFixture : IDisposable
    {
        /// <summary>
        /// Cursos de exemplo prontos para uso em testes
        /// </summary>
        public List<Course> SampleCourses { get; private set; } = new();

        /// <summary>
        /// Lessons de exemplo prontas para uso em testes
        /// </summary>
        public List<Lesson> SampleLessons { get; private set; } = new();

        /// <summary>
        /// IDs pré-definidos para facilitar testes
        /// </summary>
        public Guid DevOpsCourseId { get; private set; }
        public Guid DockerCourseId { get; private set; }
        public Guid KubernetesCourseId { get; private set; }

        public CourseTestFixture()
        {
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            // Definir IDs fixos
            DevOpsCourseId = Guid.NewGuid();
            DockerCourseId = Guid.NewGuid();
            KubernetesCourseId = Guid.NewGuid();

            // Criar cursos de exemplo
            SampleCourses = new List<Course>
            {
                new CourseBuilder()
                    .WithId(DevOpsCourseId)
                    .AsDevOpsCourse()
                    .Build(),

                new CourseBuilder()
                    .WithId(DockerCourseId)
                    .AsDockerCourse()
                    .Build(),

                new CourseBuilder()
                    .WithId(KubernetesCourseId)
                    .AsKubernetesCourse()
                    .Build()
            };

            // Criar lessons de exemplo para o curso de DevOps
            SampleLessons = LessonBuilder.BuildDevOpsCourseLessons(DevOpsCourseId);
        }

        /// <summary>
        /// Cria um curso limpo para cada teste
        /// </summary>
        public Course CreateFreshCourse()
        {
            return new CourseBuilder().Build();
        }

        /// <summary>
        /// Cria uma lesson limpa para cada teste
        /// </summary>
        public Lesson CreateFreshLesson(Guid? courseId = null)
        {
            return new LessonBuilder()
                .WithCourseId(courseId ?? Guid.NewGuid())
                .Build();
        }

        /// <summary>
        /// Obtém um curso de exemplo por tipo
        /// </summary>
        public Course GetDevOpsCourse() =>
            SampleCourses.First(c => c.Id == DevOpsCourseId);

        public Course GetDockerCourse() =>
            SampleCourses.First(c => c.Id == DockerCourseId);

        public Course GetKubernetesCourse() =>
            SampleCourses.First(c => c.Id == KubernetesCourseId);

        public void Dispose()
        {
            // Limpar recursos se necessário
            SampleCourses?.Clear();
            SampleLessons?.Clear();
        }
    }

    /// <summary>
    /// Collection definition para compartilhar fixture entre classes de teste
    /// </summary>
    [CollectionDefinition("Course Collection")]
    public class CourseCollection : ICollectionFixture<CourseTestFixture>
    {
        // Esta classe não tem código, serve apenas para definir a collection
    }
}
