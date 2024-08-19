using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileWatcherApp.Data;
using System.Linq;
using System.Threading.Tasks;
using FileWatcherApp.Models;
using FileWatcherApp.Models.Dashboard;

namespace FileWatcherApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly FileWatcherContext _context;
        private const int PageSize = 10;
        public DashboardController(FileWatcherContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index1()
        {
            var boxes = await _context.Boxes
                .Include(b => b.Jobs)
                .ThenInclude(j => j.JobStatuses)
                .ToListAsync();

            var viewModel = boxes.Select(box => new DashboardViewModel
            {
                BoxId = box.BoxId,
                BoxName = box.BoxName,
                Jobs = box.Jobs.Select(job => new DashboardDataViewModel
                {
                    JobName = job.JobName,
                    FilePath = job.FilePath,
                    ExpectedArrivalTime = job.ExpectedArrivalTime,
                    StatusClass = job.JobStatuses?.Any(js => !js.IsAvailable) == true ? "bg-danger" : "bg-success",
                    StatusText = job.JobStatuses?.Any(js => !js.IsAvailable) == true ? "Not Available" : "Available"
                }).ToList(),
                OverallStatusClass = box.Jobs?.Any(j => j.JobStatuses?.Any(js => !js.IsAvailable) == true) == true ? "bg-warning" : "bg-success",
                OverallStatusText = box.Jobs?.Any(j => j.JobStatuses?.Any(js => !js.IsAvailable) == true) == true ? "Some files not available" : "All files available"
            }).ToList();

            return View(viewModel);
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string searchString = "")
        {
            // Apply search filter
            var boxesQuery = _context.Boxes
                .Include(b => b.Jobs)
                .ThenInclude(j => j.JobStatuses)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                boxesQuery = boxesQuery.Where(b => b.BoxName.Contains(searchString) ||
                                                    b.Jobs.Any(j => j.JobName.Contains(searchString)));
            }

            // Get the total count of items
            var totalCount = await boxesQuery.CountAsync();

            // Get the paginated result
            var boxes = await boxesQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Transform to ViewModel
            var viewModel = boxes.Select(box => new DashboardViewModel
            {
                BoxId = box.BoxId,
                BoxName = box.BoxName,
                ScheduleTime = box.ScheduleTime,
                Jobs = box.Jobs.Select(job =>
                {
                    // Get the most recent job status
                    var mostRecentStatus = job.JobStatuses
                                             ?.OrderByDescending(js => js.StatusDate)
                                             .FirstOrDefault();

                    return new DashboardDataViewModel
                    {
                        JobName = job.JobName,
                        FilePath = job.FilePath,
                        ExpectedArrivalTime = job.ExpectedArrivalTime,
                        StatusClass = mostRecentStatus != null && !mostRecentStatus.IsAvailable ? "bg-danger" : "bg-success",
                        StatusText = mostRecentStatus != null && !mostRecentStatus.IsAvailable ? "Not Available" : "Available"
                    };
                }).ToList(),

                // Determine overall status based on the most recent status of each job
                OverallStatusClass = box.Jobs?.Any(j => j.JobStatuses
                                                       ?.OrderByDescending(js => js.StatusDate)
                                                       .FirstOrDefault()
                                                       ?.IsAvailable == false) == true ? "bg-danger" : "bg-success",
                OverallStatusText = box.Jobs?.Any(j => j.JobStatuses
                                                     ?.OrderByDescending(js => js.StatusDate)
                                                     .FirstOrDefault()
                                                     ?.IsAvailable == false) == true ? "Some files not available" : "All files available"
            }).ToList();


            // Prepare the paginated view model
            var paginatedViewModel = new DashboardListViewModel
            {
                Boxes = viewModel,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                SearchString = searchString
            };

            return View(paginatedViewModel);
        }

    }
}
