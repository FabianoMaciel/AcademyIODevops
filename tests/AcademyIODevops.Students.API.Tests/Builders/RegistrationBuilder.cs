using AcademyIODevops.Core.Enums;
using AcademyIODevops.Students.API.Models;

namespace AcademyIODevops.Students.API.Tests.Builders
{
    /// <summary>
    /// Builder pattern para facilitar a criação de objetos Registration em testes.
    /// Fornece valores padrão razoáveis e permite customização fluente.
    /// </summary>
    public class RegistrationBuilder
    {
        private Guid _id = Guid.NewGuid();
        private Guid _studentId = Guid.NewGuid();
        private Guid _courseId = Guid.NewGuid();
        private DateTime _registrationTime = DateTime.UtcNow;
        private EProgressLesson _status = EProgressLesson.NotStarted;
        private StudentUser? _student = null;

        /// <summary>
        /// Define um ID específico para a matrícula
        /// </summary>
        public RegistrationBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        /// Define o ID do estudante
        /// </summary>
        public RegistrationBuilder WithStudentId(Guid studentId)
        {
            _studentId = studentId;
            return this;
        }

        /// <summary>
        /// Define o ID do curso
        /// </summary>
        public RegistrationBuilder WithCourseId(Guid courseId)
        {
            _courseId = courseId;
            return this;
        }

        /// <summary>
        /// Define a data/hora de matrícula
        /// </summary>
        public RegistrationBuilder WithRegistrationTime(DateTime registrationTime)
        {
            _registrationTime = registrationTime;
            return this;
        }

        /// <summary>
        /// Define o status da matrícula
        /// </summary>
        public RegistrationBuilder WithStatus(EProgressLesson status)
        {
            _status = status;
            return this;
        }

        /// <summary>
        /// Define o estudante associado
        /// </summary>
        public RegistrationBuilder WithStudent(StudentUser student)
        {
            _student = student;
            _studentId = student.Id;
            return this;
        }

        /// <summary>
        /// Configura a matrícula como não iniciada
        /// </summary>
        public RegistrationBuilder AsNotStarted()
        {
            _status = EProgressLesson.NotStarted;
            return this;
        }

        /// <summary>
        /// Configura a matrícula como em progresso
        /// </summary>
        public RegistrationBuilder AsInProgress()
        {
            _status = EProgressLesson.InProgress;
            return this;
        }

        /// <summary>
        /// Configura a matrícula como concluída
        /// </summary>
        public RegistrationBuilder AsCompleted()
        {
            _status = EProgressLesson.Completed;
            return this;
        }

        /// <summary>
        /// Configura a matrícula com data antiga
        /// </summary>
        public RegistrationBuilder AsOldRegistration()
        {
            _registrationTime = DateTime.UtcNow.AddMonths(-6);
            return this;
        }

        /// <summary>
        /// Configura a matrícula com data recente
        /// </summary>
        public RegistrationBuilder AsRecentRegistration()
        {
            _registrationTime = DateTime.UtcNow.AddDays(-1);
            return this;
        }

        /// <summary>
        /// Configura a matrícula com StudentId vazio (para testes de validação)
        /// </summary>
        public RegistrationBuilder WithEmptyStudentId()
        {
            _studentId = Guid.Empty;
            return this;
        }

        /// <summary>
        /// Configura a matrícula com CourseId vazio (para testes de validação)
        /// </summary>
        public RegistrationBuilder WithEmptyCourseId()
        {
            _courseId = Guid.Empty;
            return this;
        }

        /// <summary>
        /// Constrói o objeto Registration com as configurações definidas
        /// </summary>
        public Registration Build()
        {
            var registration = new Registration(_studentId, _courseId, _registrationTime)
            {
                Id = _id,
                Status = _status
            };

            if (_student != null)
            {
                registration.Student = _student;
            }

            return registration;
        }

        /// <summary>
        /// Cria um builder com valores padrão
        /// </summary>
        public static RegistrationBuilder Default() => new();

        /// <summary>
        /// Cria múltiplas matrículas de uma vez para um estudante específico
        /// </summary>
        public static List<Registration> BuildMany(int count, Guid studentId)
        {
            var registrations = new List<Registration>();
            for (int i = 1; i <= count; i++)
            {
                registrations.Add(new RegistrationBuilder()
                    .WithStudentId(studentId)
                    .WithCourseId(Guid.NewGuid())
                    .WithRegistrationTime(DateTime.UtcNow.AddDays(-i))
                    .Build());
            }
            return registrations;
        }

        /// <summary>
        /// Cria matrículas com diferentes status para um estudante
        /// </summary>
        public static List<Registration> BuildWithDifferentStatuses(Guid studentId)
        {
            return new List<Registration>
            {
                new RegistrationBuilder()
                    .WithStudentId(studentId)
                    .WithCourseId(Guid.NewGuid())
                    .AsNotStarted()
                    .Build(),

                new RegistrationBuilder()
                    .WithStudentId(studentId)
                    .WithCourseId(Guid.NewGuid())
                    .AsInProgress()
                    .WithRegistrationTime(DateTime.UtcNow.AddDays(-15))
                    .Build(),

                new RegistrationBuilder()
                    .WithStudentId(studentId)
                    .WithCourseId(Guid.NewGuid())
                    .AsCompleted()
                    .WithRegistrationTime(DateTime.UtcNow.AddMonths(-2))
                    .Build()
            };
        }

        /// <summary>
        /// Cria matrículas para múltiplos cursos de um estudante
        /// </summary>
        public static List<Registration> BuildForMultipleCourses(Guid studentId, List<Guid> courseIds)
        {
            var registrations = new List<Registration>();
            foreach (var courseId in courseIds)
            {
                registrations.Add(new RegistrationBuilder()
                    .WithStudentId(studentId)
                    .WithCourseId(courseId)
                    .Build());
            }
            return registrations;
        }
    }
}
