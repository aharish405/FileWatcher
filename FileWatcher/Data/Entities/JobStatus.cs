namespace FileWatcherApp.Data.Entities
{
    public class JobStatus
    {
        public int JobStatusId { get; set; }
        public int JobId { get; set; }
        public string? FilepathChecked { get; set; }
        public DateTime StatusDate { get; set; }
        public bool IsAvailable { get; set; }
        public string? StatusMessage { get; set; }

        public virtual Job Job { get; set; }
    }
}
