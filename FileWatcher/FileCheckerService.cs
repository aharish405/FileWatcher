using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FileWatcherApp.Data;
using FileWatcherApp.Models;
using Microsoft.EntityFrameworkCore;
using FileWatcherApp.Data.Entities;

public class FileCheckerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<FileCheckerService> _logger;

    public FileCheckerService(IServiceProvider serviceProvider, ILogger<FileCheckerService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<FileWatcherContext>();
                    var jobs = await context.Jobs
                        .Include(j => j.Box)
                        .Include(j => j.Calendar)
                        .ToListAsync(stoppingToken);

                    var currentDate = DateTime.Now;
                    var currentTime = DateTime.Now.TimeOfDay;

                    foreach (var job in jobs)
                    {
                        TimeSpan startTime, endTime;

                        if (job.IgnoreBoxSchedule)
                        {
                            // Use the job-specific time
                            startTime = job.ExpectedArrivalTime.TimeOfDay.Subtract(TimeSpan.FromMinutes(job.CheckIntervalMinutes));
                            endTime = job.ExpectedArrivalTime.TimeOfDay;
                        }
                        else
                        {
                            // Use the box's schedule
                            var box = job.Box;
                            var boxScheduleTime = box.ScheduleTime.TimeOfDay;
                            var boxCalendar = box.Calendar;

                            if (boxCalendar != null)
                            {
                                // Determine if the current date is in the calendar days
                                var todayDayOfWeek = currentDate.DayOfWeek;
                                var calendarDays = boxCalendar.CalendarDays.Select(cd => cd.DayOfWeek).ToList();

                                if (!calendarDays.Contains(todayDayOfWeek))
                                {
                                    // Skip checking if today is not in the calendar days
                                    continue;
                                }
                            }

                            startTime = box.ScheduleTime.TimeOfDay.Subtract(TimeSpan.FromMinutes(job.CheckIntervalMinutes));
                            endTime = box.ScheduleTime.TimeOfDay;
                        }

                        // Check if current time is within the check window
                        if (currentTime >= startTime && currentTime <= endTime)
                        {
                            string filePath = job.FilePath;
                            filePath = ResolveFilePath(filePath, currentDate); // Use current date for file resolution

                            bool isAvailable = File.Exists(filePath);
                            string statusMessage = isAvailable ? "File is available" : "File is not available";

                            var jobStatus = new JobStatus
                            {
                                JobId = job.JobId,
                                StatusDate = DateTime.Now, // Store the current date and time
                                IsAvailable = isAvailable,
                                StatusMessage = statusMessage
                            };

                            context.JobStatuses.Add(jobStatus);
                        }
                    }

                    await context.SaveChangesAsync(stoppingToken);
                }

                _logger.LogInformation("File statuses checked at: {time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while checking file statuses.");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private string ResolveFilePath(string filePath, DateTime currentDate)
    {
        // Replace {EffectiveDate} with today's date in MMDDYYYY format
        string effectiveDate = currentDate.ToString("MMddyyyy");
        filePath = filePath.Replace("{EffectiveDate}", effectiveDate);

        // Handle wildcard characters (*)
        if (filePath.Contains("*"))
        {
            var directoryPath = Path.GetDirectoryName(filePath);
            var filePattern = Path.GetFileName(filePath);

            if (Directory.Exists(directoryPath))
            {
                var matchingFiles = Directory.GetFiles(directoryPath, filePattern);
                if (matchingFiles.Length > 0)
                {
                    filePath = matchingFiles.OrderByDescending(f => f).FirstOrDefault(); // Get the latest matching file
                }
            }
        }

        return filePath;
    }
}
