namespace FileWatcherApp.Models.EmailTemplate
{
    public class EmailTemplateViewModel
    {
        public int Id { get; set; }
        public string TemplateName { get; set; }
        public string Subject { get; set; }
        public string EmailBody { get; set; } // This will store the HTML content
    }
}
