namespace FileWatcherApp.Models.Notification
{
    public class NotificationDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Recipients { get; set; }
        public bool NotifySourceTeam { get; set; }
        public bool NotifySupportTeam { get; set; }
        public bool NotifyL1L2Team { get; set; }
        public bool Enabled { get; set; }
        public string EmailTemplateName { get; set; }
        public Dictionary<string, string> DynamicContentMapping { get; set; }
    }
}
