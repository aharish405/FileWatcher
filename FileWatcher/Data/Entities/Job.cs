using System.ComponentModel.DataAnnotations;

namespace FileWatcherApp.Data.Entities
{
    public class Job
    {
        private bool ignoreBoxSchedule;

        public int JobId { get; set; }
        public required string JobName { get; set; }
        public required string FilePath { get; set; }
        public bool IsActive { get; set; }
        public bool NotifySourceTeamAutomatically { get; set; }
        public string? SourceTeamContact { get; set; }
        public string? AxwayID { get; set; }
        public DateTime ExpectedArrivalTime { get; set; }
        public int CheckIntervalMinutes { get; set; }
        public int BoxId { get; set; } // Foreign Key
        public virtual  Box Box { get; set; }
        public virtual ICollection<JobStatus>? JobStatuses { get; set; }        
        public int? CalendarId { get; set; }
        public bool IgnoreBoxSchedule { get => ignoreBoxSchedule; set => ignoreBoxSchedule = value; } // Flag to ignore box schedule
        public Calendar? Calendar { get; set; }
    }
}
