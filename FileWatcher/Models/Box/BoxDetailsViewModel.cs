using System;
using System.Collections.Generic;

namespace FileWatcherApp.Models.Box
{
    public class BoxDetailsViewModel
    {
        public int BoxId { get; set; }
        public string BoxName { get; set; }
        public DateTime ScheduleTime { get; set; }
        public string CalendarName { get; set; }
        public string Timezone { get; set; }
        public bool IsActive { get; set; }
        public bool NotifySourceTeamAutomatically { get; set; }
        public string ExcludeCalendarName { get; set; }
        public List<string> Jobs { get; set; }
    }
}
