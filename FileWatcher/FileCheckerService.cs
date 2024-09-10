using FileWatcherApp.Data;
using FileWatcherApp.Data.Entities;
using FileWatcherApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

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
                        .Include(j => j.Box)  // Include related Box entity for each job
                        .Include(j => j.Calendar)  // Include related Calendar entity for each job
                        .ToListAsync(stoppingToken);

                    var currentDate = DateTime.Now.Date;  // Get the current date
                    var currentTime = DateTime.Now.TimeOfDay;  // Get the current time

                    foreach (var job in jobs)
                    {
                        try
                        {
                            var box = job.Box;
                            var boxCalendar = box?.Calendar;

                            // Check for excluded dates first
                            if (box?.ExcludeCalendarId.HasValue == true)
                            {
                                // Retrieve the exclude calendar if applicable
                                var excludeCalendar = await context.ExcludeCalendars
                                    .Where(ec => ec.ExcludeCalendarId == box.ExcludeCalendarId)
                                    .FirstOrDefaultAsync(stoppingToken);

                                if (excludeCalendar != null)
                                {
                                    // Check if the current date is in the excluded dates list
                                    var excludedDates = excludeCalendar.ExcludedDates;
                                    if (excludedDates.Contains(currentDate))
                                    {
                                        // Skip processing for today if it's an excluded date
                                        continue;
                                    }
                                }
                            }

                            TimeSpan startTime, endTime;

                            if (job.IgnoreBoxSchedule)
                            {
                                // Use the job-specific time if IgnoreBoxSchedule is enabled
                                startTime = job.ExpectedArrivalTime.TimeOfDay.Subtract(TimeSpan.FromMinutes(job.CheckIntervalMinutes));
                                endTime = job.ExpectedArrivalTime.TimeOfDay;
                            }
                            else
                            {
                                // Use the box's schedule time
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

                                // Calculate start and end time based on the box's schedule
                                startTime = box.ScheduleTime.TimeOfDay.Subtract(TimeSpan.FromMinutes(job.CheckIntervalMinutes));
                                endTime = box.ScheduleTime.TimeOfDay;
                            }

                            // Check if the current time is within the check window
                            //if (currentTime >= startTime && currentTime <= endTime)
                            if (true)
                            {
                                // Resolve the file path with dynamic date placeholders
                                string filePath = job.FilePath;
                                filePath = ResolveFilePath(filePath, currentDate);

                                // Check if the file exists
                                bool isAvailable = File.Exists(filePath);
                                string statusMessage = isAvailable ? "File is available" : "File is not available";

                                // Create a new job status entry
                                var jobStatus = new JobStatus
                                {
                                    JobId = job.JobId,
                                    StatusDate = DateTime.Now,  // Store the current date and time
                                    IsAvailable = isAvailable,
                                    StatusMessage = statusMessage
                                };

                                // Add the job status to the context
                                context.JobStatuses.Add(jobStatus);
                            }
                        }
                        catch (Exception x)
                        {
                            string message = $":{x.Message} for job: {job.JobName}";
                            _logger.LogError(message);
                            continue;
                            //throw;
                        }
                    }

                    // Save all job statuses to the database
                    await context.SaveChangesAsync(stoppingToken);
                }

                // Log the successful completion of the file status check
                _logger.LogInformation("File statuses checked at: {time}", DateTimeOffset.Now);
            }
            catch (Exception ex)
            {
                // Log any errors that occur during the process
                _logger.LogError(ex, "An error occurred while checking file statuses.");
            }

            // Delay the next execution to prevent continuous checking
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private string ResolveFilePath(string filePath, DateTime currentDate)
    {
        // Regular expression to match {Today-n} and {Today+n} placeholders
        var regex = new Regex(@"{Today([+-]\d+)?}", RegexOptions.Compiled);

        // Replace {Today} and {Today-n} and {Today+n} placeholders with the appropriate date
        filePath = regex.Replace(filePath, match =>
        {
            var daysOffset = 0;

            if (match.Groups[1].Success)
            {
                // Parse the offset value if present
                int.TryParse(match.Groups[1].Value, out daysOffset);
            }

            // Calculate the date based on the offset and format it as MMddyyyy
            var resultDate = currentDate.AddDays(daysOffset).ToString("MMddyyyy");
            return resultDate;
        });

        // Handle wildcard characters (*)
        if (filePath.Contains("*"))
        {
            // Extract directory path and file pattern
            var directoryPath = Path.GetDirectoryName(filePath);
            var filePattern = Path.GetFileName(filePath);

            if (Directory.Exists(directoryPath))
            {
                // Get matching files in the directory
                var matchingFiles = Directory.GetFiles(directoryPath, filePattern);
                if (matchingFiles.Length > 0)
                {
                    // Get the latest matching file
                    filePath = matchingFiles.OrderByDescending(f => f).FirstOrDefault();
                }
            }
        }

        return filePath;
    }
}
