namespace FileWatcherApp.Models.ExcludeCalendar
{
    public class ExcludeCalendarViewModel
    {
        public int ExcludeCalendarId { get; set; }
        public string Name { get; set; }
        public string ExcludedDates { get; set; }
        public int BoxCount { get; set; }
    }

    public class ExcludeCalendarListViewModel
    {
        public List<ExcludeCalendarViewModel> ExcludeCalendars { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchString { get; set; }
    }

    public class ExcludeCalendarCreateViewModel
    {
        public string Name { get; set; }
        //public List<string> SelectedDates { get; set; }
        public string SelectedDates { get; set; }
    }

    public class ExcludeCalendarEditViewModel
    {
        public int ExcludeCalendarId { get; set; }
        public string Name { get; set; }
        public List<string> SelectedDates { get; set; }
    }

}
