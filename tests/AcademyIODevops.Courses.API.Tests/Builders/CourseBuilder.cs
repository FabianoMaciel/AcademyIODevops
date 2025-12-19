using AcademyIODevops.Courses.API.Models;

namespace AcademyIODevops.Courses.API.Tests.Builders
{
    /// <summary>
    /// Builder pattern para facilitar a criação de objetos Course em testes.
    /// Fornece valores padrão razoáveis e permite customização fluente.
    /// </summary>
    public class CourseBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _name = "Default Course Name";
        private string _description = "Default course description";
        private double _price = 99.99;
        private readonly List<Lesson> _lessons = new();

        /// <summary>
        /// Define um ID específico para o curso
        /// </summary>
        public CourseBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        /// Define o nome do curso
        /// </summary>
        public CourseBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        /// <summary>
        /// Define a descrição do curso
        /// </summary>
        public CourseBuilder WithDescription(string description)
        {
            _description = description;
            return this;
        }

        /// <summary>
        /// Define o preço do curso
        /// </summary>
        public CourseBuilder WithPrice(double price)
        {
            _price = price;
            return this;
        }

        /// <summary>
        /// Adiciona uma lesson específica ao curso
        /// </summary>
        public CourseBuilder WithLesson(Lesson lesson)
        {
            _lessons.Add(lesson);
            return this;
        }

        /// <summary>
        /// Cria N lessons genéricas para o curso
        /// </summary>
        public CourseBuilder WithLessons(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                var lesson = new Lesson(
                    name: $"Lesson {i}",
                    subject: $"Subject {i}",
                    totalHours: i * 1.5,
                    courseId: _id
                );
                _lessons.Add(lesson);
            }
            return this;
        }

        /// <summary>
        /// Cria um curso de DevOps pré-configurado
        /// </summary>
        public CourseBuilder AsDevOpsCourse()
        {
            _name = "DevOps Fundamentals";
            _description = "Learn Docker, Kubernetes, CI/CD and more";
            _price = 149.99;
            return this;
        }

        /// <summary>
        /// Cria um curso de Docker pré-configurado
        /// </summary>
        public CourseBuilder AsDockerCourse()
        {
            _name = "Docker Deep Dive";
            _description = "Master container technology with Docker";
            _price = 99.99;
            return this;
        }

        /// <summary>
        /// Cria um curso de Kubernetes pré-configurado
        /// </summary>
        public CourseBuilder AsKubernetesCourse()
        {
            _name = "Kubernetes Mastery";
            _description = "Complete guide to container orchestration";
            _price = 199.99;
            return this;
        }

        /// <summary>
        /// Configura o curso com preço inválido (para testes de validação)
        /// </summary>
        public CourseBuilder WithInvalidPrice()
        {
            _price = -10.00;
            return this;
        }

        /// <summary>
        /// Configura o curso com nome vazio (para testes de validação)
        /// </summary>
        public CourseBuilder WithEmptyName()
        {
            _name = string.Empty;
            return this;
        }

        /// <summary>
        /// Configura o curso com descrição vazia (para testes de validação)
        /// </summary>
        public CourseBuilder WithEmptyDescription()
        {
            _description = string.Empty;
            return this;
        }

        /// <summary>
        /// Constrói o objeto Course com as configurações definidas
        /// </summary>
        public Course Build()
        {
            var course = new Course
            {
                Id = _id,
                Name = _name,
                Description = _description,
                Price = _price
            };

            // Nota: AddLesson tem TODO no código original
            // Quando implementado, descomentar:
            // foreach (var lesson in _lessons)
            // {
            //     course.AddLesson(lesson);
            // }

            return course;
        }

        /// <summary>
        /// Cria um builder com valores padrão
        /// </summary>
        public static CourseBuilder Default() => new();

        /// <summary>
        /// Cria múltiplos cursos de uma vez
        /// </summary>
        public static List<Course> BuildMany(int count)
        {
            var courses = new List<Course>();
            for (int i = 1; i <= count; i++)
            {
                courses.Add(new CourseBuilder()
                    .WithName($"Course {i}")
                    .WithDescription($"Description for course {i}")
                    .WithPrice(50.00 * i)
                    .Build());
            }
            return courses;
        }
    }
}
