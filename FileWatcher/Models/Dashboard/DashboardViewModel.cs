using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FileWatcherApp.Models.Dashboard
{
    public class DashboardViewModel
    {
        public int BoxId { get; set; }
        public string BoxName { get; set; }
        public DateTime ScheduleTime { get; set; }
        public string OverallStatusClass { get; set; }
        public string OverallStatusText { get; set; }
        public List<DashboardDataViewModel> Jobs { get; set; }
    }
}
