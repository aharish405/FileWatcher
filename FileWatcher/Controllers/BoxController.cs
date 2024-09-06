using FileWatcherApp.Data;
using FileWatcherApp.Data.Entities;
using FileWatcherApp.Models.Box;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace FileWatcherApp.Controllers
{
    public class BoxController : Controller
    {
        private readonly FileWatcherContext _context;
        private const int PageSize = 10;

        public BoxController(FileWatcherContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> _Index(int page = 1, string searchString = "")
        {
            var boxesQuery = _context.Boxes
                .Include(b => b.Calendar)
                .ThenInclude(c => c.CalendarDays)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                boxesQuery = boxesQuery.Where(b => b.BoxName.Contains(searchString));
            }

            var count = await boxesQuery.CountAsync();
            var boxes = await boxesQuery
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            var viewModel = new BoxListViewModel
            {
                Boxes = boxes.Select(b => new BoxIndexViewModel
                {
                    BoxId = b.BoxId,
                    BoxName = b.BoxName,
                    ScheduleTime = b.ScheduleTime,
                    CalendarName = b.Calendar != null ? b.Calendar.Name : "None",
                    CalendarDays = b.Calendar != null ? b.Calendar.CalendarDays : new List<CalendarDay>(),
                    Timezone = b.Calendar != null ? b.Calendar.Timezone : "None",
                    ScheduleType = b.Calendar != null ? b.Calendar.ScheduleType : new ScheduleType(),
                    IsActive = b.IsActive,
                }).ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(count / (double)PageSize),
                SearchString = searchString
            };

            return View(viewModel);
        }

        public IActionResult Index()
        {
            return View();
        }

        public PartialViewResult Grid()
        {
            var boxesQuery = _context.Boxes
                .Include(b => b.Calendar)
                .ThenInclude(c => c.CalendarDays)
                .AsQueryable();

            var boxes = boxesQuery.Select(b => new BoxIndexViewModel
            {
                BoxId = b.BoxId,
                BoxName = b.BoxName,
                ScheduleTime = b.ScheduleTime,
                CalendarName = b.Calendar != null ? b.Calendar.Name : "None",
                Timezone = b.Calendar != null ? b.Calendar.Timezone : "None",
                ScheduleType = b.Calendar != null ? b.Calendar.ScheduleType : new ScheduleType(),
                IsActive = b.IsActive,
            }).ToList();

            return PartialView("_Grid", boxes);
        }

        public IActionResult Details(int id)
        {
            var box = _context.Boxes
                .Include(b => b.Calendar)
                .Include(b => b.ExcludeCalendar)
                .Include(b => b.Jobs)
                .FirstOrDefault(b => b.BoxId == id);

            if (box == null)
            {
                return NotFound();
            }

            var viewModel = new BoxDetailsViewModel
            {
                BoxId = box.BoxId,
                BoxName = box.BoxName,
                ScheduleTime = box.ScheduleTime,
                CalendarName = box.Calendar?.Name ?? "None",
                Timezone = box.Calendar?.Timezone ?? "None",
                IsActive = box.IsActive,
                NotifySourceTeamAutomatically = box.NotifySourceTeamAutomatically,
                ExcludeCalendarName = box.ExcludeCalendar?.Name ?? "None",
                Jobs = box.Jobs?.Select(j => j.JobName).ToList() ?? new List<string>()
            };

            return View(viewModel);
        }
        public IActionResult Create()
        {
            var viewModel = new BoxCreateViewModel
            {
                CalendarList = _context.Calendars
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name
                    })
                    .ToList(),
                ExcludeCalendarList=_context.ExcludeCalendars
                .Select(x=> new SelectListItem
                {
                    Value=x.ExcludeCalendarId.ToString(),
                    Text=x.Name
                }).ToList(),
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(BoxCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var box = new Box
                {
                    BoxName = model.BoxName,
                    ScheduleTime = model.ScheduleTime,
                    CalendarId = model.CalendarId,
                    ExcludeCalendarId=model.ExcludeCalendarId,
                    IsActive=model.IsActive
                };

                _context.Boxes.Add(box);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            model.CalendarList = _context.Calendars
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToList();
            model.ExcludeCalendarList = _context.ExcludeCalendars
               .Select(x => new SelectListItem
               {
                   Value = x.ExcludeCalendarId.ToString(),
                   Text = x.Name
               }).ToList();

            return View(model);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var box = await _context.Boxes
                                    .Include(b => b.Calendar)
                                    .Where(b => b.BoxId == id)
                                    .Select(b => new BoxEditViewModel
                                    {
                                        BoxId = b.BoxId,
                                        BoxName = b.BoxName,
                                        ScheduleTime = b.ScheduleTime,
                                        CalendarId = b.CalendarId,
                                        ExcludeCalendarId=b.ExcludeCalendarId, 
                                        IsActive=b.IsActive,
                                        NotifySourceTeamAutomatically=b.NotifySourceTeamAutomatically
                                    })
                                    .FirstOrDefaultAsync();

            if (box == null)
            {
                return NotFound();
            }

            var calendars = await _context.Calendars.ToListAsync();
            box.CalendarList = calendars
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == box.CalendarId
                });
            box.ExcludeCalendarList = _context.ExcludeCalendars
              .Select(x => new SelectListItem
              {
                  Value = x.ExcludeCalendarId.ToString(),
                  Text = x.Name
              }).ToList();

            return View(box);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BoxEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var box = await _context.Boxes.FindAsync(model.BoxId);

                if (box == null)
                {
                    return NotFound();
                }

                box.BoxName = model.BoxName;
                box.ScheduleTime = model.ScheduleTime;
                box.CalendarId = model.CalendarId;
                box.IsActive = model.IsActive;
                box.ExcludeCalendarId = model.ExcludeCalendarId;
                box.NotifySourceTeamAutomatically = model.NotifySourceTeamAutomatically;

                _context.Boxes.Update(box);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            model.CalendarList = (await _context.Calendars.ToListAsync())
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == model.CalendarId
                });
            model.ExcludeCalendarList = _context.ExcludeCalendars
              .Select(x => new SelectListItem
              {
                  Value = x.ExcludeCalendarId.ToString(),
                  Text = x.Name
              }).ToList();

            return View(model);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var box = await _context.Boxes
                .Include(b => b.Calendar)
                .Where(b => b.BoxId == id)
                .Select(b => new BoxDeleteViewModel
                {
                    BoxId = b.BoxId,
                    BoxName = b.BoxName,
                    ScheduleTime = b.ScheduleTime,
                    CalendarName = b.Calendar.Name
                })
                .FirstOrDefaultAsync();

            if (box == null)
            {
                return NotFound();
            }

            return View(box);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var box = await _context.Boxes.FindAsync(id);

            if (box != null)
            {
                _context.Boxes.Remove(box);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

    }
}