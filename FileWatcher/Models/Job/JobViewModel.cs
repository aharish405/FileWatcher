using FileWatcherApp.Data.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FileWatcherApp.Models.Job
{
    public class JobDetailsViewModel
    {
        public int JobId { get; set; }
        public string JobName { get; set; }
        public string FilePath { get; set; }
        public DateTime ExpectedArrivalTime { get; set; }
        public int CheckIntervalMinutes { get; set; }
        public string BoxName { get; set; }
        public string CalendarName { get; set; }
        public string Timezone { get; set; }
        public bool IsActive { get; set; }
        public bool NotifySourceTeamAutomatically { get; set; }
        public ICollection<JobStatus> JobStatuses { get; set; } // Assuming this is a collection of related status records
    }
}
