using Microsoft.AspNetCore.Mvc.Rendering;

namespace FileWatcherApp.Models.Notification
{
    public class EditNotificationViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Recipients { get; set; }
        public bool NotifySourceTeam { get; set; }
        public bool NotifySupportTeam { get; set; }
        public bool NotifyL1L2Team { get; set; }
        public bool Enabled { get; set; }
        public int EmailTemplateId { get; set; }
        public IEnumerable<SelectListItem>? EmailTemplates { get; set; }
    }
}