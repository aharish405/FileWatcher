using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace FileWatcherApp.Models.ImportCsv
{
    public class ImportCsvViewModel
    {
        [Required]
        public IFormFile CsvFile { get; set; }

        [Required]
        public string ImportType { get; set; } // "Job" or "Box"
    }
}
