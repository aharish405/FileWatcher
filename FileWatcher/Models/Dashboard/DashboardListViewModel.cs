namespace FileWatcherApp.Models.Dashboard
{
    public class DashboardListViewModel
    {
        public IEnumerable<DashboardViewModel> Boxes { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchString { get; set; }
    }
}
