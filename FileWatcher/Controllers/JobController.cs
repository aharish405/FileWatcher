using FileWatcherApp.Data;
using FileWatcherApp.Data.Entities;
using FileWatcherApp.Models.Box;
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
        public async Task<IActionResult> _Index(int page = 1, string searchString = "")
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
                Jobs = items
                
            };

            // Return the view with the view model
            return View(viewModel);
        }
        public IActionResult Index()
        {
            return View();
        }

        public PartialViewResult Grid()
        {
            var jobsQuery = _context.Jobs
                .Include(j => j.Box) 
                .Include(j => j.Calendar)
                .AsQueryable();

            var jobs = jobsQuery.Select(j => new JobIndexViewModel
            {
                JobId = j.JobId,
                JobName = j.JobName,
                FilePath = j.FilePath,
                ExpectedArrivalTime = j.ExpectedArrivalTime,
                CheckIntervalMinutes = j.CheckIntervalMinutes,
                BoxName = j.Box != null ? j.Box.BoxName : "None",
                CalendarName = j.Calendar != null ? j.Calendar.Name : "None",
                Timezone = j.Calendar != null ? j.Calendar.Timezone : "None",
                IsActive = j.IsActive,
                NotifySourceTeamAutomatically = j.NotifySourceTeamAutomatically
            }).ToList();

            return PartialView("_Grid", jobs);
        }
        public async Task<IActionResult> Details(int id)
        {
            // Fetch the job including related entities
            var job = await _context.Jobs
                .Include(j => j.Box)       // Include Box details
                .Include(j => j.Calendar)  // Include Calendar details
                .FirstOrDefaultAsync(j => j.JobId == id);

            if (job == null)
            {
                return NotFound();
            }

            // Prepare the view model
            var viewModel = new JobDetailsViewModel
            {
                JobId = job.JobId,
                JobName = job.JobName,
                FilePath = job.FilePath,
                ExpectedArrivalTime = job.ExpectedArrivalTime,
                CheckIntervalMinutes = job.CheckIntervalMinutes,
                BoxName = job.Box != null ? job.Box.BoxName : "None",
                CalendarName = job.Calendar != null ? job.Calendar.Name : "None",
                Timezone = job.Calendar != null ? job.Calendar.Timezone : "None",
                IsActive = job.IsActive,
                NotifySourceTeamAutomatically = job.NotifySourceTeamAutomatically,
                JobStatuses = job.JobStatuses // Assuming JobStatuses is a collection of related status records
            };

            return View(viewModel);
        }
        public IActionResult Create()
        {
            var viewModel = new CreateViewModel
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
        public IActionResult Create(CreateViewModel model)
        {
            var errors = GetModelStateErrors();
            if (ModelState.IsValid)
            {
                var job = new Job
                {
                    JobName = model.JobName,
                    FilePath = model.FilePath,
                    ExpectedArrivalTime = model.ExpectedArrivalTime,
                    CheckIntervalMinutes = model.CheckIntervalMinutes,
                    SourceTeamContact = model.SourceTeamContact,
                    BoxId = model.BoxId,
                    CalendarId = model.CalendarId,
                    IgnoreBoxSchedule = model.IgnoreBoxSchedule,
                    IsActive = model.IsActive,
                    NotifySourceTeamAutomatically=model.NotifySourceTeamAutomatically,
                    AxwayID = model.AxwayID
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
                              .Select(j => new EditViewModel
                              {
                                  JobId = j.JobId,
                                  JobName = j.JobName,
                                  FilePath = j.FilePath,
                                  ExpectedArrivalTime = j.ExpectedArrivalTime,
                                  CheckIntervalMinutes = j.CheckIntervalMinutes,
                                  SourceTeamContact = j.SourceTeamContact,
                                  BoxId = j.BoxId,
                                  CalendarId = j.CalendarId,                                  
                                  IgnoreBoxSchedule = j.IgnoreBoxSchedule, 
                                  IsActive = j.IsActive,
                                  NotifySourceTeamAutomatically = j.NotifySourceTeamAutomatically,
                                  AxwayID = j.AxwayID,
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
        public IActionResult Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
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
                job.IgnoreBoxSchedule = model.IgnoreBoxSchedule;
                job.IsActive = model.IsActive;
                job.NotifySourceTeamAutomatically = model.NotifySourceTeamAutomatically;
                job.AxwayID = model.AxwayID;

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
            // Fetch the job entity from the database
            var job = _context.Jobs
                .Include(j => j.Box)
                .Include(j => j.Calendar)
                .Where(j => j.JobId == id)
                .Select(j => new DeleteViewModel
                {
                    JobId = j.JobId,
                    JobName = j.JobName,
                    FilePath = j.FilePath,
                    ExpectedArrivalTime = j.ExpectedArrivalTime,
                    CheckIntervalMinutes = j.CheckIntervalMinutes,
                    BoxName = j.Box != null ? j.Box.BoxName : "None",
                    CalendarName = j.Calendar != null ? j.Calendar.Name : "None",
                    IsActive = j.IsActive
                })
                .FirstOrDefault();

            // Check if the job exists
            if (job == null)
            {
                return NotFound();
            }

            // Return the view with the DeleteViewModel
            return View(job);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var job = await _context.Jobs.FindAsync(id);

            if (job == null)
            {
                return NotFound();
            }

            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private IEnumerable<string> GetModelStateErrors()
        {
            return ModelState.Values.SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage)
                                    .ToList();
        }
    }
}