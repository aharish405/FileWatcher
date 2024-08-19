using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FileWatcherApp.Models.Dashboard
{
    public class DashboardDataViewModel
    {
        public int JobId { get; set; }
        public string JobName { get; set; }
        public string FilePath { get; set; }
        public DateTime ExpectedArrivalTime { get; set; }
        public int CheckIntervalMinutes { get; set; }
        public int BoxId { get; set; }
        public string BoxName { get; set; }
        public string StatusClass { get; set; }
        public string StatusText { get; set; }
        public IEnumerable<SelectListItem> BoxList { get; set; }
    }
}
