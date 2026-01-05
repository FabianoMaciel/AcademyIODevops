using AcademyIODevops.Students.API.Models;

namespace AcademyIODevops.Students.API.Tests.Builders
{
    /// <summary>
    /// Builder pattern para facilitar a criação de objetos Certification em testes.
    /// Fornece valores padrão razoáveis e permite customização fluente.
    /// </summary>
    public class CertificationBuilder
    {
        private Guid _id = Guid.NewGuid();
        private Guid _courseId = Guid.NewGuid();
        private Guid _studentId = Guid.NewGuid();

        /// <summary>
        /// Define um ID específico para a certificação
        /// </summary>
        public CertificationBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        /// Define o ID do curso
        /// </summary>
        public CertificationBuilder WithCourseId(Guid courseId)
        {
            _courseId = courseId;
            return this;
        }

        /// <summary>
        /// Define o ID do estudante
        /// </summary>
        public CertificationBuilder WithStudentId(Guid studentId)
        {
            _studentId = studentId;
            return this;
        }

        /// <summary>
        /// Configura a certificação com CourseId vazio (para testes de validação)
        /// </summary>
        public CertificationBuilder WithEmptyCourseId()
        {
            _courseId = Guid.Empty;
            return this;
        }

        /// <summary>
        /// Configura a certificação com StudentId vazio (para testes de validação)
        /// </summary>
        public CertificationBuilder WithEmptyStudentId()
        {
            _studentId = Guid.Empty;
            return this;
        }

        /// <summary>
        /// Constrói o objeto Certification com as configurações definidas
        /// </summary>
        public Certification Build()
        {
            var certification = new Certification
            {
                Id = _id,
                CourseId = _courseId,
                StudentId = _studentId
            };

            return certification;
        }

        /// <summary>
        /// Cria um builder com valores padrão
        /// </summary>
        public static CertificationBuilder Default() => new();

        /// <summary>
        /// Cria múltiplas certificações de uma vez para um estudante específico
        /// </summary>
        public static List<Certification> BuildMany(int count, Guid studentId)
        {
            var certifications = new List<Certification>();
            for (int i = 1; i <= count; i++)
            {
                certifications.Add(new CertificationBuilder()
                    .WithStudentId(studentId)
                    .WithCourseId(Guid.NewGuid())
                    .Build());
            }
            return certifications;
        }

        /// <summary>
        /// Cria certificações para múltiplos cursos de um estudante
        /// </summary>
        public static List<Certification> BuildForMultipleCourses(Guid studentId, List<Guid> courseIds)
        {
            var certifications = new List<Certification>();
            foreach (var courseId in courseIds)
            {
                certifications.Add(new CertificationBuilder()
                    .WithStudentId(studentId)
                    .WithCourseId(courseId)
                    .Build());
            }
            return certifications;
        }

        /// <summary>
        /// Cria certificações para múltiplos estudantes de um curso
        /// </summary>
        public static List<Certification> BuildForMultipleStudents(Guid courseId, List<Guid> studentIds)
        {
            var certifications = new List<Certification>();
            foreach (var studentId in studentIds)
            {
                certifications.Add(new CertificationBuilder()
                    .WithStudentId(studentId)
                    .WithCourseId(courseId)
                    .Build());
            }
            return certifications;
        }
    }
}
