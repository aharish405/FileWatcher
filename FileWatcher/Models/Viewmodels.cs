using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FileWatcherApp.ViewModels
{
    public class BoxViewModel
    {
        public int BoxId { get; set; }

        [Required(ErrorMessage = "Box Name is required")]
        public string BoxName { get; set; }

        [Required(ErrorMessage = "Schedule Time is required")]
        public DateTime ScheduleTime { get; set; }

        public string OverallStatusClass { get; set; }
        public string OverallStatusText { get; set; }
        public List<JobViewModel> Jobs { get; set; }
    }

    //public class JobViewModel
    //{
    //    public string JobName { get; set; }
    //    public string FilePath { get; set; }
    //    public DateTime ExpectedArrivalTime { get; set; }
    //    public string StatusClass { get; set; }
    //    public string StatusText { get; set; }
    //}
    public class JobViewModel
    {
        public int JobId { get; set; }

        [Required(ErrorMessage = "Job Name is required")]
        public string JobName { get; set; }

        [Required(ErrorMessage = "File Path is required")]
        public string FilePath { get; set; }

        [Required(ErrorMessage = "Expected Arrival Time is required")]
        public DateTime ExpectedArrivalTime { get; set; }

        [Required(ErrorMessage = "Check Interval Minutes is required")]
        public int CheckIntervalMinutes { get; set; }

        [Required(ErrorMessage = "Box is required")]
        public int BoxId { get; set; }

        public IEnumerable<SelectListItem> BoxList { get; set; }
    }
}
