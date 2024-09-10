namespace FileWatcherApp.Data.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }        
        public string? Recipients { get; set; }        
        public bool NotifySourceTeam { get; set; }
        public bool NotifySupportTeam { get; set; }
        public bool NotifyL1L2Team { get; set; }
        public bool Enabled { get; set; }

        public int EmailTemplateId { get; set; }
        public EmailTemplate EmailTemplate { get; set; }
    }
}
