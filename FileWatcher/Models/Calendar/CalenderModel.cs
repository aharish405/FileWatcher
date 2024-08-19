using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FileWatcherApp.Models.Calendar
{
    public class CalendarViewModel
    {
        public int CalendarId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Schedule Type is required")]
        public string ScheduleType { get; set; } // Display the ScheduleType as a string

        public string Days { get; set; } // Display specific days for custom schedules

        public string Timezone { get; set; } // New field for timezone

        public string Description { get; set; } // New field for description

        // List of Schedule Types for dropdown
        public IEnumerable<SelectListItem> ScheduleTypes { get; set; }
    }

    public class CalendarListViewModel
    {
        public IEnumerable<CalendarViewModel> Calendars { get; set; }
        public string SearchString { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }

}