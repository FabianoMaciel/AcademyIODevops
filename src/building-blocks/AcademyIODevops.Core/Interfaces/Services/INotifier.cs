using AcademyIODevops.Core.Notifications;

namespace AcademyIODevops.Core.Interfaces.Services
{
    public interface INotifier
    {
        bool HasNotification();
        List<Notification> GetNotifications();
        void Handle(Notification notification);
    }
}
