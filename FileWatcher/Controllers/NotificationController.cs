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
using FileWatcherApp.Helpers;
using Newtonsoft.Json;

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


        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateNotificationViewModel
            {
                // Initially, we don't have any dynamic content until a template is selected
                DynamicContentFields = new List<string>(),
                EntityProperties = GetEntityProperties(),
                EmailTemplates= _context.EmailTemplates
                .Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.TemplateName

                })

            };
            
            return View(model);
        }

        // Method to load dynamic content fields when a template is selected
        [HttpPost]
        public IActionResult LoadDynamicFields(int emailTemplateId)
        {
            var emailTemplate = _context.EmailTemplates.Find(emailTemplateId);
            if (emailTemplate == null)
            {
                return NotFound();
            }

            // Deserialize the dynamic fields from the template
            var dynamicFields = emailTemplate.DynamicContent
            .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();

            // Prepare the view model
            var model = new CreateNotificationViewModel
            {
                EmailTemplateId = emailTemplate.Id,
                DynamicContentFields = dynamicFields,
                EntityProperties = GetEntityProperties() // Get Job and Box fields dynamically
            };

            return PartialView("_DynamicContentFieldsPartial", model); // Render partial view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateNotificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Serialize dynamic mappings into JSON to store them in the database
                var dynamicContentMappingJson = JsonConvert.SerializeObject(model.DynamicContentMapping);

                var notification = new Notification
                {
                    Title = model.Title,
                    Recipients = model.AdditionalRecipients,
                    EmailTemplateId = model.EmailTemplateId,
                    DynamicContentFields = dynamicContentMappingJson
                };

                _context.Notifications.Add(notification);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            // Repopulate the entity properties and email templates in case of validation failure
            model.EntityProperties = GetEntityProperties();
            ViewBag.EmailTemplates = new SelectList(_context.EmailTemplates.ToList(), "Id", "Name");

            return View(model);
        }

        // Method to get properties from Job and Box entities using reflection
        private Dictionary<string, List<string>> GetEntityProperties()
        {
            var jobProperties = typeof(Job)
                .GetProperties()
                .Select(p => p.Name)
                .ToList();

            var boxProperties = typeof(Box)
                .GetProperties()
                .Select(p => p.Name)
                .ToList();

            return new Dictionary<string, List<string>>
        {
            { "Job", jobProperties },
            { "Box", boxProperties }
        };

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var notification = _context.Notifications.Find(id);
            if (notification == null)
            {
                return NotFound();
            }

            var dynamicContentMapping = JsonConvert.DeserializeObject<Dictionary<string, string>>(notification.DynamicContentFields);
            var emailTemplate = _context.EmailTemplates.Find(notification.EmailTemplateId);

            var model = new CreateNotificationViewModel
            {
                Id = notification.Id,
                Title = notification.Title,
                AdditionalRecipients = notification.Recipients,
                EmailTemplateId = notification.EmailTemplateId,
                DynamicContentFields = emailTemplate.DynamicContent.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                DynamicContentMapping = dynamicContentMapping,
                EntityProperties = GetEntityProperties(),
                NotifySourceTeam = notification.NotifySourceTeam,
                NotifySupportTeam = notification.NotifySupportTeam,
                NotifyL1L2Team = notification.NotifyL1L2Team,
                Enabled = notification.Enabled,
                EmailTemplates = _context.EmailTemplates.Select(e => new SelectListItem
                {
                    Value = e.Id.ToString(),
                    Text = e.TemplateName
                })
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CreateNotificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                var notification = _context.Notifications.Find(model.Id);
                if (notification == null)
                {
                    return NotFound();
                }

                // Serialize dynamic mappings into JSON to store in the database
                var dynamicContentMappingJson = JsonConvert.SerializeObject(model.DynamicContentMapping);

                notification.Title = model.Title;
                notification.Recipients = model.AdditionalRecipients;
                notification.EmailTemplateId = model.EmailTemplateId;
                notification.DynamicContentFields = dynamicContentMappingJson;
                notification.NotifySourceTeam = model.NotifySourceTeam;
                notification.NotifySupportTeam = model.NotifySupportTeam;
                notification.NotifyL1L2Team = model.NotifyL1L2Team;
                notification.Enabled = model.Enabled;

                _context.Notifications.Update(notification);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            // Repopulate entity properties and email templates in case of validation failure
            model.EntityProperties = GetEntityProperties();
            model.EmailTemplates = _context.EmailTemplates.Select(e => new SelectListItem
            {
                Value = e.Id.ToString(),
                Text = e.TemplateName
            });

            return View(model);
        }

        // Action to show the details of a notification
        [HttpGet]
        public IActionResult Details(int id)
        {
            var notification = _context.Notifications
                .Include(n => n.EmailTemplate) // Include related EmailTemplate for display
                .SingleOrDefault(n => n.Id == id);

            if (notification == null)
            {
                return NotFound();
            }

            // Deserialize the dynamic content fields
            var dynamicContentMapping = JsonConvert.DeserializeObject<Dictionary<string, string>>(notification.DynamicContentFields);

            var model = new NotificationDetailViewModel
            {
                Id = notification.Id,
                Title = notification.Title,
                Recipients = notification.Recipients,
                NotifySourceTeam = notification.NotifySourceTeam,
                NotifySupportTeam = notification.NotifySupportTeam,
                NotifyL1L2Team = notification.NotifyL1L2Team,
                Enabled = notification.Enabled,
                EmailTemplateName = notification.EmailTemplate.TemplateName,
                DynamicContentMapping = dynamicContentMapping
            };

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
