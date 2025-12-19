using AcademyIODevops.Courses.API.Models;

namespace AcademyIODevops.Courses.API.Tests.Builders
{
    /// <summary>
    /// Builder pattern para facilitar a criação de objetos Lesson em testes.
    /// Fornece valores padrão razoáveis e permite customização fluente.
    /// </summary>
    public class LessonBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _name = "Default Lesson";
        private string _subject = "Default Subject";
        private double _totalHours = 2.0;
        private Guid _courseId = Guid.NewGuid();

        /// <summary>
        /// Define um ID específico para a lesson
        /// </summary>
        public LessonBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        /// Define o nome da lesson
        /// </summary>
        public LessonBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        /// <summary>
        /// Define o assunto da lesson
        /// </summary>
        public LessonBuilder WithSubject(string subject)
        {
            _subject = subject;
            return this;
        }

        /// <summary>
        /// Define a duração total em horas
        /// </summary>
        public LessonBuilder WithTotalHours(double hours)
        {
            _totalHours = hours;
            return this;
        }

        /// <summary>
        /// Define o ID do curso ao qual a lesson pertence
        /// </summary>
        public LessonBuilder WithCourseId(Guid courseId)
        {
            _courseId = courseId;
            return this;
        }

        /// <summary>
        /// Cria uma lesson de introdução ao Docker
        /// </summary>
        public LessonBuilder AsDockerIntroduction()
        {
            _name = "Introduction to Docker";
            _subject = "Container Basics";
            _totalHours = 2.5;
            return this;
        }

        /// <summary>
        /// Cria uma lesson sobre Kubernetes
        /// </summary>
        public LessonBuilder AsKubernetesLesson()
        {
            _name = "Kubernetes Fundamentals";
            _subject = "Container Orchestration";
            _totalHours = 4.0;
            return this;
        }

        /// <summary>
        /// Cria uma lesson sobre CI/CD
        /// </summary>
        public LessonBuilder AsCiCdLesson()
        {
            _name = "CI/CD Pipeline Setup";
            _subject = "Continuous Integration";
            _totalHours = 3.5;
            return this;
        }

        /// <summary>
        /// Configura a lesson com duração inválida (para testes de validação)
        /// </summary>
        public LessonBuilder WithInvalidHours()
        {
            _totalHours = -1.0;
            return this;
        }

        /// <summary>
        /// Configura a lesson com nome vazio (para testes de validação)
        /// </summary>
        public LessonBuilder WithEmptyName()
        {
            _name = string.Empty;
            return this;
        }

        /// <summary>
        /// Configura a lesson com assunto vazio (para testes de validação)
        /// </summary>
        public LessonBuilder WithEmptySubject()
        {
            _subject = string.Empty;
            return this;
        }

        /// <summary>
        /// Configura a lesson com CourseId vazio (para testes de validação)
        /// </summary>
        public LessonBuilder WithEmptyCourseId()
        {
            _courseId = Guid.Empty;
            return this;
        }

        /// <summary>
        /// Constrói o objeto Lesson com as configurações definidas
        /// </summary>
        public Lesson Build()
        {
            var lesson = new Lesson(_name, _subject, _totalHours, _courseId)
            {
                Id = _id
            };

            return lesson;
        }

        /// <summary>
        /// Cria um builder com valores padrão
        /// </summary>
        public static LessonBuilder Default() => new();

        /// <summary>
        /// Cria múltiplas lessons de uma vez para um curso específico
        /// </summary>
        public static List<Lesson> BuildMany(int count, Guid courseId)
        {
            var lessons = new List<Lesson>();
            for (int i = 1; i <= count; i++)
            {
                lessons.Add(new LessonBuilder()
                    .WithName($"Lesson {i}")
                    .WithSubject($"Subject {i}")
                    .WithTotalHours(i * 1.5)
                    .WithCourseId(courseId)
                    .Build());
            }
            return lessons;
        }

        /// <summary>
        /// Cria uma sequência completa de lessons para um curso de DevOps
        /// </summary>
        public static List<Lesson> BuildDevOpsCourseLessons(Guid courseId)
        {
            return new List<Lesson>
            {
                new LessonBuilder()
                    .WithName("Introduction to DevOps")
                    .WithSubject("DevOps Fundamentals")
                    .WithTotalHours(2.0)
                    .WithCourseId(courseId)
                    .Build(),

                new LessonBuilder()
                    .WithName("Docker Basics")
                    .WithSubject("Containerization")
                    .WithTotalHours(3.5)
                    .WithCourseId(courseId)
                    .Build(),

                new LessonBuilder()
                    .WithName("Kubernetes Essentials")
                    .WithSubject("Orchestration")
                    .WithTotalHours(5.0)
                    .WithCourseId(courseId)
                    .Build(),

                new LessonBuilder()
                    .WithName("CI/CD Pipelines")
                    .WithSubject("Automation")
                    .WithTotalHours(4.0)
                    .WithCourseId(courseId)
                    .Build()
            };
        }
    }
}
