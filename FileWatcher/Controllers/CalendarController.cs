using FileWatcherApp.Data;
using FileWatcherApp.Data.Entities;
using FileWatcherApp.Models.Calendar;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FileWatcherApp.Controllers
{
    public class CalendarController : Controller
    {
        private readonly FileWatcherContext _context;

        public CalendarController(FileWatcherContext context)
        {
            _context = context;
        }
        public IActionResult _Index(string searchString, int page = 1)
        {
            
            var query = _context.Calendars
                                .Include(c => c.CalendarDays)
                                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(c => c.Name.Contains(searchString));
            }

            var totalCount = query.Count();

            var calendars = query.Select(c => new CalendarViewModel
                                 {
                                     CalendarId = c.Id,
                                     Name = c.Name,
                                     ScheduleType = c.ScheduleType.ToString(),
                                     Days = c.ScheduleType == ScheduleType.Custom
                                         ? string.Join(", ", c.CalendarDays.Select(d => d.DayOfWeek.ToString()))
                                         : c.ScheduleType == ScheduleType.Weekly ? "Weekly" :
                                           c.ScheduleType == ScheduleType.Weekdays ? "Weekdays (Mon-Fri)" :
                                           "Daily",
                                     Timezone = c.Timezone,
                                     Description = c.Description
                                 })
                                 .ToList();

            var viewModel = new CalendarListViewModel
            {
                Calendars = calendars                
            };

            return View(viewModel);
        }
        public IActionResult Index()
        {
            return View();
        }
        public PartialViewResult Grid()
        {
            var query = _context.Calendars.AsQueryable();

            var calendars = query.Select(c => new CalendarViewModel
            {
                CalendarId = c.Id,
                Name = c.Name,
                ScheduleType = c.ScheduleType.ToString(),
                Days = FormatDays(c),
                Timezone = c.Timezone,
                Description = c.Description
            }).ToList();

            return PartialView("_Grid", calendars);
        }

        // Helper method to format days based on schedule type
        private static string FormatDays(Calendar calendar)
        {
            if (calendar.CalendarDays == null || !calendar.CalendarDays.Any())
            {
                return calendar.ScheduleType switch
                {
                    ScheduleType.Weekly => "Weekly",
                    ScheduleType.Weekdays => "Weekdays (Mon-Fri)",
                    _ => "Daily"
                };
            }

            return calendar.ScheduleType switch
            {
                ScheduleType.Custom => string.Join(", ", calendar.CalendarDays.Select(d => d.DayOfWeek.ToString())),
                ScheduleType.Weekly => "Weekly",
                ScheduleType.Weekdays => "Weekdays (Mon-Fri)",
                _ => "Daily"
            };
        }
        public async Task<IActionResult> Details(int id)
        {
            var calendar = await _context.Calendars
                .Include(c => c.CalendarDays)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (calendar == null)
            {
                return NotFound();
            }

            var viewModel = new CalendarDetailsViewModel
            {
                CalendarId = calendar.Id,
                Name = calendar.Name,
                ScheduleType = calendar.ScheduleType.ToString(),
                Days = FormatDays(calendar),
                Timezone = calendar.Timezone,
                Description = calendar.Description
            };

            return View(viewModel);
        }
        public IActionResult Create()
        {
            var viewModel = new CalendarViewModel
            {
                // Initialize any default values here if needed
                ScheduleTypes = Enum.GetValues(typeof(ScheduleType)).Cast<ScheduleType>()
                                     .Select(st => new SelectListItem
                                     {
                                         Value = st.ToString(),
                                         Text = st.ToString()
                                     }).ToList()
            };

            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CalendarViewModel model)
        {
            if (ModelState.ErrorCount < 4)
            {
                var calendar = new Calendar
                {
                    Name = model.Name,
                    ScheduleType = Enum.Parse<ScheduleType>(model.ScheduleType), // Convert string to enum
                    Timezone = model.Timezone,
                    Description = model.Description
                };

                // Handle specific days for custom schedules
                if (calendar.ScheduleType == ScheduleType.Custom && !string.IsNullOrEmpty(model.Days))
                {
                    // Assuming Days is a comma-separated list of day names
                    var days = model.Days.Split(',').Select(d => new CalendarDay
                    {
                        DayOfWeek = Enum.Parse<DayOfWeek>(d.Trim(), true),
                        Calendar = calendar
                    }).ToList();

                    calendar.CalendarDays = days;
                }

                _context.Calendars.Add(calendar);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // Repopulate ScheduleTypes for the view if the model is invalid
            model.ScheduleTypes = Enum.GetValues(typeof(ScheduleType)).Cast<ScheduleType>()
                                       .Select(st => new SelectListItem
                                       {
                                           Value = st.ToString(),
                                           Text = st.ToString()
                                       }).ToList();

            return View(model);
        }
        public IActionResult Edit(int id)
        {
            var calendar = _context.Calendars
                                    .Where(c => c.Id == id)
                                    .Select(c => new CalendarViewModel
                                    {
                                        CalendarId = c.Id,
                                        Name = c.Name,
                                        ScheduleType = c.ScheduleType.ToString(),
                                        Timezone = c.Timezone,
                                        Description = c.Description,
                                        Days= string.Join(",", c.CalendarDays.Select(cd => cd.DayOfWeek.ToString()))
                                    })
                                    .FirstOrDefault();

            if (calendar == null)
            {
                return NotFound();
            }

            return View(calendar);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CalendarViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var calendar = _context.Calendars.Find(model.CalendarId);

                if (calendar == null)
                {
                    return NotFound();
                }

                calendar.Name = model.Name;
                calendar.ScheduleType = Enum.Parse<ScheduleType>(model.ScheduleType);
                calendar.Timezone = model.Timezone;
                calendar.Description = model.Description;
                if (calendar.ScheduleType == ScheduleType.Custom && !string.IsNullOrEmpty(model.Days))
                {
                    // Assuming Days is a comma-separated list of day names
                    var days = model.Days.Split(',').Select(d => new CalendarDay
                    {
                        DayOfWeek = Enum.Parse<DayOfWeek>(d.Trim(), true),
                        Calendar = calendar
                    }).ToList();

                    calendar.CalendarDays = days;
                }
                _context.Calendars.Update(calendar);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            // Re-populate the view model if the model state is invalid
            model.ScheduleType = model.ScheduleType; // Maintain the selected value
            return View(model);
        }
        // GET: Calendar/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var calendar = await _context.Calendars
                .Where(c => c.Id == id)
                .Select(c => new CalendarViewModel
                {
                    CalendarId = c.Id,
                    Name = c.Name,
                    ScheduleType = c.ScheduleType.ToString(),
                    Timezone = c.Timezone,
                    Description = c.Description,
                    Days = string.Join(",", c.CalendarDays.Select(cd => cd.DayOfWeek.ToString()))
                })
                .FirstOrDefaultAsync();

            if (calendar == null)
            {
                return NotFound();
            }

            return View(calendar);
        }
        // POST: Calendar/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var calendar = await _context.Calendars.FindAsync(id);

            if (calendar != null)
            {
                _context.Calendars.Remove(calendar);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}