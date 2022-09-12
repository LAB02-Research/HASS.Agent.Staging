using System.Text.Json.Serialization;

namespace HASS.Agent.Models.HomeAssistant
{
    public class NotificationAction
    {
        public string Action { get; set; }
        public string Title { get; set; }
    }

    public class NotificationExtraData
    {
        public int Duration { get; set; } = 0;
        public string Image { get; set; }

        public List<NotificationAction> Actions { get; set; } = new();
    }
    
    public class Notification
    {
        public Notification()
        {
            //
        }

        public string Message { get; set; }
        public string Title { get; set; }

        public NotificationExtraData Data { get; set; } = new();
    }
}
