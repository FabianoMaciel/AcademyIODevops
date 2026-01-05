using AcademyIODevops.Students.API.Data;
using Microsoft.EntityFrameworkCore;

namespace AcademyIODevops.Students.API.Tests.Fixtures
{
    /// <summary>
    /// Fixture para testes de repositório que precisam de um contexto de banco de dados.
    /// Usa InMemory database para testes rápidos e isolados.
    /// </summary>
    public class RepositoryTestFixture
    {
        /// <summary>
        /// Cria um novo StudentsContext com InMemory database.
        /// Cada contexto usa um banco de dados único (isolamento entre testes).
        /// </summary>
        public StudentsContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<StudentsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new StudentsContext(options);
        }

        /// <summary>
        /// Cria um contexto com nome de database específico.
        /// Útil quando múltiplos contextos precisam compartilhar dados.
        /// </summary>
        public StudentsContext CreateContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<StudentsContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;

            return new StudentsContext(options);
        }

        /// <summary>
        /// Faz seed do database com entidades e salva as mudanças.
        /// </summary>
        public void SeedDatabase(StudentsContext context, params object[] entities)
        {
            if (entities == null || entities.Length == 0)
                return;

            context.AddRange(entities);
            context.SaveChanges();
        }

        /// <summary>
        /// Limpa todas as entidades do contexto.
        /// </summary>
        public void ClearDatabase(StudentsContext context)
        {
            context.StudentUsers.RemoveRange(context.StudentUsers);
            context.Registrations.RemoveRange(context.Registrations);
            context.Certifications.RemoveRange(context.Certifications);
            context.SaveChanges();
        }
    }
}
