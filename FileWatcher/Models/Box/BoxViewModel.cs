using FileWatcherApp.Data.Entities;
using FileWatcherApp.Models.Job;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FileWatcherApp.Models.Box
{
    public class BoxViewModel
    {
        public int BoxId { get; set; }

        [Required(ErrorMessage = "Box Name is required")]
        public string BoxName { get; set; }

        [Required(ErrorMessage = "Schedule Time is required")]
        public DateTime ScheduleTime { get; set; }

        public int? CalendarId { get; set; }
        public string CalendarName { get; set; }
        public IEnumerable<CalendarDay> CalendarDays { get; set; }
        public string Timezone { get; set; }

        public string OverallStatusClass { get; set; }
        public string OverallStatusText { get; set; }
        public List<JobViewModel> Jobs { get; set; }
        public IEnumerable<SelectListItem> CalendarList { get; set; }
    }
}
