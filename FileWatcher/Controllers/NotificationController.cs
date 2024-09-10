using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FileWatcherApp.Data;
using FileWatcherApp.Models;
using FileWatcherApp.Models.Notification;
using System.Linq;
using System.Threading.Tasks;
using FileWatcherApp.Data.Entities;
using FileWatcherApp.Models.Job;

namespace FileWatcherApp.Controllers
{
    public class NotificationController : Controller
    {
        private readonly FileWatcherContext _context;

        public NotificationController(FileWatcherContext context)
        {
            _context = context;
        }

        // GET: Notification
        public IActionResult Index()
        {
            return View();
        }

        public async Task<PartialViewResult> Grid()
        {
            var notifications = await _context.Notifications
                .Include(n => n.EmailTemplate)
                .Select(n => new NotificationDetailViewModel
                {
                    Id = n.Id,
                    Title = n.Title,
                    Recipients = n.Recipients,
                    NotifySourceTeam = n.NotifySourceTeam,
                    NotifySupportTeam = n.NotifySupportTeam,
                    NotifyL1L2Team = n.NotifyL1L2Team,
                    Enabled = n.Enabled,
                    EmailTemplateName = n.EmailTemplate.TemplateName
                })
                .ToListAsync();

            return PartialView("_Grid", notifications);
        }


        // GET: Notification/Create
        public IActionResult Create()
        {
            var viewModel = new CreateNotificationViewModel
            {
                EmailTemplates = new SelectList(_context.EmailTemplates, "Id", "TemplateName")
            };
            return View(viewModel);
        }

        // POST: Notification/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNotificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var notification = new Notification
                {
                    Title = model.Title,
                    Recipients = model.Recipients,
                    NotifySourceTeam = model.NotifySourceTeam,
                    NotifySupportTeam = model.NotifySupportTeam,
                    NotifyL1L2Team = model.NotifyL1L2Team,
                    Enabled = model.Enabled,
                    EmailTemplateId = model.EmailTemplateId
                };

                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.EmailTemplates = new SelectList(_context.EmailTemplates, "Id", "TemplateName", model.EmailTemplateId);
            return View(model);
        }

      
        // GET: Notification/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            var viewModel = new EditNotificationViewModel
            {
                Id = notification.Id,
                Title = notification.Title,
                Recipients = notification.Recipients,
                NotifySourceTeam = notification.NotifySourceTeam,
                NotifySupportTeam = notification.NotifySupportTeam,
                NotifyL1L2Team = notification.NotifyL1L2Team,
                Enabled = notification.Enabled,
                EmailTemplateId = notification.EmailTemplateId,
                EmailTemplates = new SelectList(_context.EmailTemplates, "Id", "TemplateName", notification.EmailTemplateId)
            };

            return View(viewModel);
        }

        // POST: Notification/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditNotificationViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var notification = await _context.Notifications.FindAsync(id);
                if (notification == null)
                {
                    return NotFound();
                }

                notification.Title = model.Title;
                notification.Recipients = model.Recipients;
                notification.NotifySourceTeam = model.NotifySourceTeam;
                notification.NotifySupportTeam = model.NotifySupportTeam;
                notification.NotifyL1L2Team = model.NotifyL1L2Team;
                notification.Enabled = model.Enabled;
                notification.EmailTemplateId = model.EmailTemplateId;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            model.EmailTemplates = new SelectList(_context.EmailTemplates, "Id", "TemplateName", model.EmailTemplateId);
            return View(model);
        }

        // GET: Notification/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var notification = await _context.Notifications
                .Include(n => n.EmailTemplate)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (notification == null)
            {
                return NotFound();
            }

            var viewModel = new DeleteNotificationViewModel
            {
                Id = notification.Id,
                Title = notification.Title,
                Recipients = notification.Recipients,
                NotifySourceTeam = notification.NotifySourceTeam,
                NotifySupportTeam = notification.NotifySupportTeam,
                NotifyL1L2Team = notification.NotifyL1L2Team,
                Enabled = notification.Enabled,
                EmailTemplateName = notification.EmailTemplate.TemplateName
            };

            return View(viewModel);
        }

        // POST: Notification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
