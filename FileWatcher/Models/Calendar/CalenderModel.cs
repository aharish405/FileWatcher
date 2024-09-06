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
        public string ScheduleType { get; set; } 

        public string Days { get; set; } 

        public string Timezone { get; set; } 

        public string Description { get; set; } 
        public IEnumerable<SelectListItem> ScheduleTypes { get; set; }
    }

    public class CalendarListViewModel
    {
        public IEnumerable<CalendarViewModel> Calendars { get; set; }
        
    }

}