using System.ComponentModel.DataAnnotations;

namespace FileWatcherApp.Data.Entities
{
    public class Box
    {
        [Key]
        public int BoxId { get; set; }
        [Required]
        public string BoxName { get; set; }
        public int? CalendarId { get; set; }
        public Calendar Calendar { get; set; }
        public DateTime ScheduleTime { get; set; }
        public ICollection<Job> Jobs { get; set; }

        public int? ExcludeCalendarId { get; set; }
        public ExcludeCalendar ExcludeCalendar { get; set; }
    }
}