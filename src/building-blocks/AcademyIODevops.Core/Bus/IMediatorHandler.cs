using AcademyIODevops.Core.Messages;

namespace AcademyIODevops.Core.Bus
{
    public interface IMediatorHandler
    {
        Task PublicEvent<T>(T ev) where T : Event;
    }
}
