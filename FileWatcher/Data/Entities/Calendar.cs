namespace FileWatcherApp.Data.Entities
{
    public class Calendar
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ScheduleType ScheduleType { get; set; }
        public string Timezone { get; set; } // New field
        public string Description { get; set; } // New field
        public ICollection<CalendarDay> CalendarDays { get; set; } // For "some days of the week" schedules
    } 
}




