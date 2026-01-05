using System.Diagnostics.CodeAnalysis;

namespace AcademyIODevops.Core.Exceptions
{
    [Serializable]
    [ExcludeFromCodeCoverage]
    public class DatabaseNotFoundException : Exception
    {
        public DatabaseNotFoundException()
        {
        }

        public DatabaseNotFoundException(string message)
            : base(message)
        {
        }

        public DatabaseNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
