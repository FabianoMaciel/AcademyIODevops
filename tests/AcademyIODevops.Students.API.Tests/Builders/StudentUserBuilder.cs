using AcademyIODevops.Students.API.Models;

namespace AcademyIODevops.Students.API.Tests.Builders
{
    /// <summary>
    /// Builder pattern para facilitar a criação de objetos StudentUser em testes.
    /// Fornece valores padrão razoáveis e permite customização fluente.
    /// </summary>
    public class StudentUserBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _userName = "student.test";
        private string _firstName = "John";
        private string _lastName = "Doe";
        private string _email = "john.doe@academyio.com";
        private DateTime _dateOfBirth = new DateTime(1995, 5, 15);
        private bool _isAdmin = false;

        /// <summary>
        /// Define um ID específico para o estudante
        /// </summary>
        public StudentUserBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        /// <summary>
        /// Define o username do estudante
        /// </summary>
        public StudentUserBuilder WithUserName(string userName)
        {
            _userName = userName;
            return this;
        }

        /// <summary>
        /// Define o primeiro nome do estudante
        /// </summary>
        public StudentUserBuilder WithFirstName(string firstName)
        {
            _firstName = firstName;
            return this;
        }

        /// <summary>
        /// Define o sobrenome do estudante
        /// </summary>
        public StudentUserBuilder WithLastName(string lastName)
        {
            _lastName = lastName;
            return this;
        }

        /// <summary>
        /// Define o email do estudante
        /// </summary>
        public StudentUserBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        /// <summary>
        /// Define a data de nascimento do estudante
        /// </summary>
        public StudentUserBuilder WithDateOfBirth(DateTime dateOfBirth)
        {
            _dateOfBirth = dateOfBirth;
            return this;
        }

        /// <summary>
        /// Define se o estudante é administrador
        /// </summary>
        public StudentUserBuilder WithIsAdmin(bool isAdmin)
        {
            _isAdmin = isAdmin;
            return this;
        }

        /// <summary>
        /// Cria um estudante administrador
        /// </summary>
        public StudentUserBuilder AsAdmin()
        {
            _isAdmin = true;
            _userName = "admin.user";
            _firstName = "Admin";
            _lastName = "User";
            _email = "admin@academyio.com";
            return this;
        }

        /// <summary>
        /// Cria um estudante regular pré-configurado
        /// </summary>
        public StudentUserBuilder AsRegularStudent()
        {
            _isAdmin = false;
            _userName = "student.regular";
            _firstName = "Regular";
            _lastName = "Student";
            _email = "regular.student@academyio.com";
            return this;
        }

        /// <summary>
        /// Cria um estudante com dados específicos
        /// </summary>
        public StudentUserBuilder WithFullName(string firstName, string lastName)
        {
            _firstName = firstName;
            _lastName = lastName;
            return this;
        }

        /// <summary>
        /// Configura o estudante com username vazio (para testes de validação)
        /// </summary>
        public StudentUserBuilder WithEmptyUserName()
        {
            _userName = string.Empty;
            return this;
        }

        /// <summary>
        /// Configura o estudante com nome vazio (para testes de validação)
        /// </summary>
        public StudentUserBuilder WithEmptyFirstName()
        {
            _firstName = string.Empty;
            return this;
        }

        /// <summary>
        /// Configura o estudante com sobrenome vazio (para testes de validação)
        /// </summary>
        public StudentUserBuilder WithEmptyLastName()
        {
            _lastName = string.Empty;
            return this;
        }

        /// <summary>
        /// Configura o estudante com email vazio (para testes de validação)
        /// </summary>
        public StudentUserBuilder WithEmptyEmail()
        {
            _email = string.Empty;
            return this;
        }

        /// <summary>
        /// Configura o estudante como menor de idade
        /// </summary>
        public StudentUserBuilder AsMinor()
        {
            _dateOfBirth = DateTime.Now.AddYears(-15);
            return this;
        }

        /// <summary>
        /// Configura o estudante como adulto
        /// </summary>
        public StudentUserBuilder AsAdult()
        {
            _dateOfBirth = DateTime.Now.AddYears(-25);
            return this;
        }

        /// <summary>
        /// Constrói o objeto StudentUser com as configurações definidas
        /// </summary>
        public StudentUser Build()
        {
            return new StudentUser(
                _id,
                _userName,
                _firstName,
                _lastName,
                _email,
                _dateOfBirth,
                _isAdmin
            );
        }

        /// <summary>
        /// Cria um builder com valores padrão
        /// </summary>
        public static StudentUserBuilder Default() => new();

        /// <summary>
        /// Cria múltiplos estudantes de uma vez
        /// </summary>
        public static List<StudentUser> BuildMany(int count)
        {
            var students = new List<StudentUser>();
            for (int i = 1; i <= count; i++)
            {
                students.Add(new StudentUserBuilder()
                    .WithUserName($"student{i}")
                    .WithFirstName($"Student{i}")
                    .WithLastName($"Test{i}")
                    .WithEmail($"student{i}@academyio.com")
                    .WithDateOfBirth(new DateTime(1990 + i, i, i))
                    .Build());
            }
            return students;
        }

        /// <summary>
        /// Cria uma lista de estudantes com diferentes perfis
        /// </summary>
        public static List<StudentUser> BuildDiverseStudents()
        {
            return new List<StudentUser>
            {
                new StudentUserBuilder()
                    .WithUserName("john.doe")
                    .WithFirstName("John")
                    .WithLastName("Doe")
                    .WithEmail("john.doe@academyio.com")
                    .AsAdult()
                    .Build(),

                new StudentUserBuilder()
                    .WithUserName("jane.smith")
                    .WithFirstName("Jane")
                    .WithLastName("Smith")
                    .WithEmail("jane.smith@academyio.com")
                    .AsAdult()
                    .Build(),

                new StudentUserBuilder()
                    .WithUserName("admin.user")
                    .AsAdmin()
                    .Build(),

                new StudentUserBuilder()
                    .WithUserName("young.student")
                    .WithFirstName("Young")
                    .WithLastName("Student")
                    .WithEmail("young@academyio.com")
                    .AsMinor()
                    .Build()
            };
        }
    }
}
