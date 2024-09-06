using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FileWatcherApp.Models.Job
{
    public class DeleteViewModel
    {
        public int JobId { get; set; }
        public string JobName { get; set; }
        public string FilePath { get; set; }
        public DateTime? ExpectedArrivalTime { get; set; }
        public int? CheckIntervalMinutes { get; set; }
        public string BoxName { get; set; }
        public string CalendarName { get; set; }
        public bool IsActive { get; set; }
    }

}
