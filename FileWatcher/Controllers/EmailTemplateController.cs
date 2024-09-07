using FileWatcherApp.Models.EmailTemplate;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace FileWatcherApp.Controllers
{


    public class EmailTemplateController : Controller
    {
        // Replace this with your data access logic
        private static List<EmailTemplateViewModel> _emailTemplates = new List<EmailTemplateViewModel>();

        public IActionResult Index()
        {
            return View(_emailTemplates);
        }

        public IActionResult Create()
        {
            return View(new EmailTemplateViewModel());
        }

        [HttpPost]
        public IActionResult Create(EmailTemplateViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Id = _emailTemplates.Count + 1; // Simplified ID generation
                _emailTemplates.Add(model);
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var template = _emailTemplates.FirstOrDefault(t => t.Id == id);
            if (template == null)
            {
                return NotFound();
            }

            return View(template);
        }

        [HttpPost]
        public IActionResult Edit(EmailTemplateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var template = _emailTemplates.FirstOrDefault(t => t.Id == model.Id);
                if (template != null)
                {
                    template.TemplateName = model.TemplateName;
                    template.Subject = model.Subject;
                    template.EmailBody = model.EmailBody;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public IActionResult Delete(int id)
        {
            var template = _emailTemplates.FirstOrDefault(t => t.Id == id);
            if (template != null)
            {
                _emailTemplates.Remove(template);
            }

            return RedirectToAction(nameof(Index));
        }
    }

}
