using System.ComponentModel.DataAnnotations;

namespace FileWatcherApp.Data.Entities
{
    public class Job
    {
        public int JobId { get; set; }
        public string JobName { get; set; }
        public string FilePath { get; set; }
        public DateTime ExpectedArrivalTime { get; set; }
        public int CheckIntervalMinutes { get; set; }
        public int BoxId { get; set; } // Foreign Key
        public virtual Box Box { get; set; }
        public virtual ICollection<JobStatus> JobStatuses { get; set; }        
        public int? CalendarId { get; set; }
        public bool IgnoreBoxSchedule { get; set; } // Flag to ignore box schedule
        public Calendar Calendar { get; set; }
    }
}
