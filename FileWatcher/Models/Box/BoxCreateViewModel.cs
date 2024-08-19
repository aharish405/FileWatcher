using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FileWatcherApp.Models.Box
{
    public class BoxCreateViewModel
    {
        [Required(ErrorMessage = "Box Name is required")]
        public string BoxName { get; set; }

        [Required(ErrorMessage = "Schedule Time is required")]
        public DateTime ScheduleTime { get; set; }

        public int? CalendarId { get; set; }
        public IEnumerable<SelectListItem> CalendarList { get; set; }
    }

}
