using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FileWatcherApp.Data;
using FileWatcherApp.Models.ExcludeCalendar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileWatcherApp.Data.Entities;

public class ExcludeCalendarController : Controller
{
    private readonly FileWatcherContext _context;
    private const int PageSize = 10; 

    public ExcludeCalendarController(FileWatcherContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(int page = 1, string searchString = "")
    {
        var excludeCalendarsQuery = _context.ExcludeCalendars.AsQueryable();

        if (!string.IsNullOrEmpty(searchString))
        {
            excludeCalendarsQuery = excludeCalendarsQuery.Where(ec => ec.Name.Contains(searchString));
        }

        var count = await excludeCalendarsQuery.CountAsync();
        var excludeCalendars = await excludeCalendarsQuery
            .Skip((page - 1) * PageSize)
            .Take(PageSize)
            .ToListAsync();

        var viewModel = new ExcludeCalendarListViewModel
        {
            ExcludeCalendars = excludeCalendars.Select(ec => new ExcludeCalendarViewModel
            {
                ExcludeCalendarId = ec.ExcludeCalendarId,
                Name = ec.Name,
                ExcludedDates = string.Join(", ", ec.ExcludedDates.Select(d => d.ToString("yyyy-MM-dd"))),
                BoxCount = ec.Box?.Count ?? 0
            }).ToList(),
            CurrentPage = page,
            TotalPages = (int)Math.Ceiling(count / (double)PageSize),
            SearchString = searchString
        };

        return View(viewModel);
    }

    public IActionResult Create()
    {
        return View(new ExcludeCalendarCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExcludeCalendarCreateViewModel model)
    {
        if (ModelState.IsValid)
        {
            var excludeCalendar = new ExcludeCalendar
            {
                Name = model.Name,
                ExcludedDates = model.SelectedDates
        .Split(new[] { ", " }, StringSplitOptions.None) // Split the string into an array of date strings
        .Select(d => DateTime.Parse(d)) // Parse each date string into a DateTime object
        .ToList() // Convert the result to a List<DateTime>
            };


            _context.Add(excludeCalendar);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var excludeCalendar = await _context.ExcludeCalendars.FindAsync(id);
        if (excludeCalendar == null)
        {
            return NotFound();
        }

        var model = new ExcludeCalendarEditViewModel
        {
            ExcludeCalendarId = excludeCalendar.ExcludeCalendarId,
            Name = excludeCalendar.Name,
            SelectedDates = excludeCalendar.ExcludedDates.Select(d => d.ToString("yyyy-MM-dd")).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ExcludeCalendarEditViewModel model)
    {
        if (id != model.ExcludeCalendarId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var excludeCalendar = await _context.ExcludeCalendars.FindAsync(id);
                excludeCalendar.Name = model.Name;
                excludeCalendar.ExcludedDates = model.SelectedDates.Select(d => DateTime.Parse(d)).ToList();

                _context.Update(excludeCalendar);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ExcludeCalendars.Any(ec => ec.ExcludeCalendarId == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var excludeCalendar = await _context.ExcludeCalendars.FindAsync(id);
        if (excludeCalendar == null)
        {
            return NotFound();
        }

        var model = new ExcludeCalendarViewModel
        {
            ExcludeCalendarId = excludeCalendar.ExcludeCalendarId,
            Name = excludeCalendar.Name,
            ExcludedDates = string.Join(", ", excludeCalendar.ExcludedDates.Select(d => d.ToString("yyyy-MM-dd")))
        };

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var excludeCalendar = await _context.ExcludeCalendars.FindAsync(id);
        _context.ExcludeCalendars.Remove(excludeCalendar);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
