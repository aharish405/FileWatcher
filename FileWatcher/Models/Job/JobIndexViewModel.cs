namespace FileWatcherApp.Models.Job
{
    public class JobIndexViewModel
    {
        public int JobId { get; set; }
        public string JobName { get; set; }
        public string FilePath { get; set; }
        public string BoxName { get; set; }
        public string CalendarName { get; set; }
        public DateTime ExpectedArrivalTime { get; set; }
        public int CheckIntervalMinutes { get; set; }
        public bool IsActive { get; set; }
        public bool NotifySourceTeamAutomatically { get; set; }
        public string Timezone { get; internal set; }
    }

}
