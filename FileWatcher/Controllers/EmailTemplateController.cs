using FileWatcherApp.Data;
using FileWatcherApp.Data.Entities;
using FileWatcherApp.Models.EmailTemplate;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileWatcherApp.Controllers
{
    public class EmailTemplateController : Controller
    {
        private readonly FileWatcherContext _context;

        public EmailTemplateController(FileWatcherContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public PartialViewResult Grid()
        {
            var emailTemplates = _context.EmailTemplates.AsNoTracking()
                .Select(et => new CreateEmailTemplateViewModel
                {
                    Id = et.Id,
                    TemplateName = et.TemplateName,
                    Subject = et.Subject,
                    EmailBody = et.EmailBody,
                    DynamicContent=et.DynamicContent??""
                }).ToList();

            return PartialView("_Grid", emailTemplates);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEmailTemplateViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Extract dynamic content placeholders from the email body
                var dynamicContent = ExtractDynamicContent(model.EmailBody);

                var emailTemplate = new EmailTemplate
                {
                    TemplateName = model.TemplateName,
                    Subject = model.Subject,
                    EmailBody = model.EmailBody,
                    DynamicContent = dynamicContent,
                    Enabled = true // Default value
                };

                _context.EmailTemplates.Add(emailTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // Method to extract dynamic content from email body
        private string ExtractDynamicContent(string emailBody)
        {
            var matches = Regex.Matches(emailBody, @"\{(.*?)\}");
            var dynamicContent = string.Join(",", matches.Cast<Match>().Select(m => m.Groups[1].Value));
            return dynamicContent;
        }


        public async Task<IActionResult> Edit(int id)
        {
            var emailTemplate = await _context.EmailTemplates
            .FirstOrDefaultAsync(et => et.Id == id);

            var notification= await _context.Notifications.FirstOrDefaultAsync(x=>x.EmailTemplateId==id);
            if (emailTemplate == null)
            {
                return NotFound();
            }
            ViewBag.IsLinked = false;
            // Check if the email template is linked to any notifications
            if (notification!=null)
            {
                // Pass a flag or message to the view indicating the template cannot be edited
                ViewBag.IsLinked = true;
                ViewBag.Message = "This template is linked to one or more notifications. Modifications are not allowed. Please delete the linked notifications first.";
            }

            var model = new CreateEmailTemplateViewModel
            {
                Id = emailTemplate.Id,
                TemplateName = emailTemplate.TemplateName,
                Subject = emailTemplate.Subject,
                EmailBody = emailTemplate.EmailBody
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CreateEmailTemplateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var emailTemplate = await _context.EmailTemplates.FindAsync(id);
                if (emailTemplate == null)
                {
                    return NotFound();
                }

                emailTemplate.TemplateName = model.TemplateName;
                emailTemplate.Subject = model.Subject;
                emailTemplate.EmailBody = model.EmailBody;
                emailTemplate.DynamicContent = ExtractDynamicContent(model.EmailBody);

                _context.EmailTemplates.Update(emailTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var emailTemplate = await _context.EmailTemplates.FindAsync(id);
            if (emailTemplate == null)
            {
                return NotFound();
            }

            var model = new CreateEmailTemplateViewModel
            {
                Id = emailTemplate.Id,
                TemplateName = emailTemplate.TemplateName,
                Subject = emailTemplate.Subject,
                EmailBody = emailTemplate.EmailBody
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emailTemplate = await _context.EmailTemplates.FindAsync(id);
            if (emailTemplate != null)
            {
                _context.EmailTemplates.Remove(emailTemplate);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
