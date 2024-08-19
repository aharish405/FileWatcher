﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FileWatcherApp.Models.Job
{
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
        public string BoxName { get; set; }

        public IEnumerable<SelectListItem> BoxList { get; set; }

        // New Properties
        public int? CalendarId { get; set; }
        public string TimeZone { get; set; }
        public bool IgnoreBoxSchedule { get; set; }
        public IEnumerable<SelectListItem> CalendarList { get; set; } // To populate calendar dropdown
    }
}
