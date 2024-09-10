using FileWatcherApp.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FileWatcherApp.Helpers
{
    public class EmailWith_DynamicContent
    {
        private readonly FileWatcherContext _context;

        public EmailWith_DynamicContent(FileWatcherContext context)
        {
            _context = context;
        }
        public async Task SendNotification(int notificationId)
        {
            var notification = await _context.Notifications
                .Include(n => n.EmailTemplate)
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification != null)
            {
                // Deserialize dynamic content mapping
                var dynamicMappings = JsonConvert.DeserializeObject<Dictionary<string, string>>(notification.DynamicContentFields);

                var jobs = _context.Jobs.Include(j => j.Box).ToList();

                foreach (var job in jobs)
                {
                    var emailContent = notification.EmailTemplate.EmailBody;

                    foreach (var mapping in dynamicMappings)
                    {
                        var entityName = mapping.Value.Split('.')[0];
                        var propertyName = mapping.Value.Split('.')[1];

                        object value = null;

                        if (entityName == "Job")
                        {
                            value = GetPropertyValue(job, propertyName);
                        }
                        else if (entityName == "Box")
                        {
                            value = GetPropertyValue(job.Box, propertyName);
                        }

                        emailContent = emailContent.Replace($"{{{mapping.Key}}}", value?.ToString() ?? string.Empty);
                    }

                    //SendEmail(notification.Recipients, emailContent);
                }
            }
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);
        }

    }
}
