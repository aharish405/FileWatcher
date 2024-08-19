

namespace FileWatcherApp.Models.Job
{
    public class JobListViewModel
    {
        public IEnumerable<Data.Entities.Job> Jobs { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchString { get; set; }
    }

}