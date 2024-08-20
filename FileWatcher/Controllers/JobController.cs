using FileWatcherApp.Data;
using FileWatcherApp.Data.Entities;
using FileWatcherApp.Models.Job;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FileWatcherApp.Controllers
{
    public class JobController : Controller
    {
        private readonly FileWatcherContext _context;
        private const int PageSize = 10;

        public JobController(FileWatcherContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int page = 1, string searchString = "")
        {
            // Query to include related entities
            var jobsQuery = _context.Jobs
                                    .Include(j => j.Box)       // Ensure Box is loaded
                                    .Include(j => j.Calendar)  // Ensure Calendar is loaded
                                    .AsQueryable();

            // Apply search filter if specified
            if (!string.IsNullOrEmpty(searchString))
            {
                jobsQuery = jobsQuery.Where(j => j.JobName.Contains(searchString));
            }

            // Get total count for pagination
            var count = await jobsQuery.CountAsync();

            // Get paginated results
            var items = await jobsQuery
                                .Skip((page - 1) * PageSize)  // Skip based on current page
                                .Take(PageSize)               // Take PageSize items
                                .ToListAsync();

            // Prepare view model
            var viewModel = new JobListViewModel
            {
                Jobs = items,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(count / (double)PageSize),
                SearchString = searchString
            };

            // Return the view with the view model
            return View(viewModel);
        }
        public IActionResult Create()
        {
            var viewModel = new JobViewModel
            {
                // Populate BoxList with available boxes from the database
                BoxList = _context.Boxes.Select(b => new SelectListItem
                {
                    Value = b.BoxId.ToString(),
                    Text = b.BoxName
                }).ToList(),

                // Populate CalendarList with available calendars from the database
                CalendarList = _context.Calendars.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList()
            };

            return View(viewModel);
        }
        [HttpPost]
        public IActionResult Create(JobViewModel model)
        {
            var errors = GetModelStateErrors();
            if (errors.Count() <= 5)
            {
                var job = new Job
                {
                    JobName = model.JobName,
                    FilePath = model.FilePath,
                    ExpectedArrivalTime = model.ExpectedArrivalTime,
                    CheckIntervalMinutes = model.CheckIntervalMinutes,
                    SourceTeamContact = model.SourceTeamContact,
                    BoxId = model.BoxId,
                    CalendarId = model.CalendarId, // New property
                    //TimeZone = model.TimeZone,     // New property
                    IgnoreBoxSchedule = model.IgnoreBoxSchedule // New property
                };

                _context.Jobs.Add(job);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            model.BoxList = _context.Boxes
                                    .Select(b => new SelectListItem
                                    {
                                        Value = b.BoxId.ToString(),
                                        Text = b.BoxName
                                    })
                                    .ToList();

            model.CalendarList = _context.Calendars
                                         .Select(c => new SelectListItem
                                         {
                                             Value = c.Id.ToString(),
                                             Text = c.Name
                                         })
                                         .ToList();

            return View(model);
        }
        public IActionResult Edit(int id)
        {
            var job = _context.Jobs
                              .Where(j => j.JobId == id)
                              .Select(j => new JobViewModel
                              {
                                  JobId = j.JobId,
                                  JobName = j.JobName,
                                  FilePath = j.FilePath,
                                  ExpectedArrivalTime = j.ExpectedArrivalTime,
                                  CheckIntervalMinutes = j.CheckIntervalMinutes,
                                  SourceTeamContact = j.SourceTeamContact,
                                  BoxId = j.BoxId,
                                  CalendarId = j.CalendarId, // New property
                                  //TimeZone = j.TimeZone,     // New property
                                  IgnoreBoxSchedule = j.IgnoreBoxSchedule, // New property
                                  BoxList = _context.Boxes
                                                    .Select(b => new SelectListItem
                                                    {
                                                        Value = b.BoxId.ToString(),
                                                        Text = b.BoxName
                                                    })
                                                    .ToList(),
                                  CalendarList = _context.Calendars
                                                         .Select(c => new SelectListItem
                                                         {
                                                             Value = c.Id.ToString(),
                                                             Text = c.Name
                                                         })
                                                         .ToList() // Populate CalendarList
                              })
                              .FirstOrDefault();

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }
        [HttpPost]
        public IActionResult Edit(JobViewModel model)
        {
            // Validate model state with a threshold of less than 3 errors
            if (ModelState.ErrorCount < 5)
            {
                var job = _context.Jobs.Find(model.JobId);

                if (job == null)
                {
                    return NotFound();
                }

                // Update job properties with the model values
                job.JobName = model.JobName;
                job.FilePath = model.FilePath;
                job.ExpectedArrivalTime = model.ExpectedArrivalTime;
                job.CheckIntervalMinutes = model.CheckIntervalMinutes;
                job.SourceTeamContact = model.SourceTeamContact;
                job.BoxId = model.BoxId;
                job.CalendarId = model.CalendarId;
                //job.TimeZone = model.TimeZone;
                job.IgnoreBoxSchedule = model.IgnoreBoxSchedule; 

                // Save changes to the database
                _context.Jobs.Update(job);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // Repopulate BoxList and CalendarList if the model state is invalid
            model.BoxList = _context.Boxes
                                    .Select(b => new SelectListItem
                                    {
                                        Value = b.BoxId.ToString(),
                                        Text = b.BoxName
                                    })
                                    .ToList();

            model.CalendarList = _context.Calendars
                                         .Select(c => new SelectListItem
                                         {
                                             Value = c.Id.ToString(),
                                             Text = c.Name
                                         })
                                         .ToList();

            return View(model);
        }
        public IActionResult Delete(int id)
        {
            var job = _context.Jobs
                              .Where(j => j.JobId == id)
                              .Select(j => new JobViewModel
                              {
                                  JobId = j.JobId,
                                  JobName = j.JobName,
                                  FilePath = j.FilePath,
                                  ExpectedArrivalTime = j.ExpectedArrivalTime,
                                  CheckIntervalMinutes = j.CheckIntervalMinutes,
                                  BoxId = j.BoxId
                              })
                              .FirstOrDefault();

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var job = _context.Jobs.Find(id);

            if (job != null)
            {
                _context.Jobs.Remove(job);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        private IEnumerable<string> GetModelStateErrors()
        {
            return ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();
        }
    }
}