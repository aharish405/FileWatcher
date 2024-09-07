using System.ComponentModel.DataAnnotations;

namespace FileWatcherApp.Data.Entities
{
    public class EmailTemplate
    {
        [Key]
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string Subject { get; set; }
        public string EmailBody { get; set; }
        public string? DynamicContent { get; set; }
        public bool Enabled { get; set; }

    }
}
