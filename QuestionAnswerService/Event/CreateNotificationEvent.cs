using EventBus.Base;

namespace QuestionAnswerService.Event
{
    public class CreateNotificationEvent : IntegrationEvent
    {
        public CreateNotificationEvent(string notificationType, string notificationText, int notificationUserId)
        {
            NotificationType = notificationType;
            NotificationText = notificationText;
            NotificationUserId = notificationUserId;
                
        }
        public string NotificationType { get; set; }
        public string NotificationText { get; set; }
        public int NotificationUserId { get; set; }
    }
}
