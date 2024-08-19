using System.Collections.Generic;

namespace FileWatcherApp.Models.Box
{
    public class BoxListViewModel
    {
        public IEnumerable<BoxIndexViewModel> Boxes { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchString { get; set; }
    }

}
