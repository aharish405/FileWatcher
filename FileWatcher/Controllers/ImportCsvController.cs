using Microsoft.AspNetCore.Mvc;
using FileWatcherApp.Models;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileWatcherApp.Data;
using System.Text;
using FileWatcherApp.Data.Entities;
using FileWatcherApp.Models.ImportCsv;

namespace FileWatcherApp.Controllers
{
    public class ImportCsvController : Controller
    {
        private readonly FileWatcherContext _context;

        public ImportCsvController(FileWatcherContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(new ImportCsvViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Import(ImportCsvViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var reader = new StreamReader(model.CsvFile.OpenReadStream()))
                {
                    var csvData = new List<string[]>();
                    string line;
                    bool isFirstRow = true;

                    while ((line = reader.ReadLine()) != null)
                    {
                        var fields = line.Split(',');

                        if (isFirstRow)
                        {
                            fields = fields.Concat(new[] { "Status", "Error" }).ToArray(); // Add Status and Error columns
                            csvData.Add(fields);
                            isFirstRow = false;
                            continue;
                        }

                        csvData.Add(fields);
                    }

                    if (model.ImportType == "Box")
                    {
                        var list = await ProcessBoxImport(csvData);
                        return ReturnCsvFile(list, "BoxImportResults.csv");
                    }
                    else if (model.ImportType == "Job")
                    {
                        var list = await ProcessJobImport(csvData);
                        return ReturnCsvFile(list, "JobImportResults.csv");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid import type.");
                        return View("Index", model);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid import type.");
                return View("Index", model);
            }
        }

        private async Task<List<string[]>> ProcessBoxImport(List<string[]> csvData)
        {
            var results = new List<string[]>();

            for (int i = 1; i < csvData.Count; i++)
            {
                var row = csvData[i];
                try
                {
                    var box = new Box
                    {
                        BoxName = row[0],
                        CalendarId = string.IsNullOrEmpty(row[1]) ? (int?)null : int.Parse(row[1]),
                        ScheduleTime = DateTime.ParseExact(row[2], "HH:mm:ss", CultureInfo.InvariantCulture),
                        IsActive = bool.Parse(row[3]),
                        NotifySourceTeamAutomatically = bool.Parse(row[4]),
                        ExcludeCalendarId = string.IsNullOrEmpty(row[5]) ? (int?)null : int.Parse(row[5])
                    };

                    _context.Boxes.Add(box);
                    await _context.SaveChangesAsync();
                    // If import is successful, add a success status
                    results.Add(row.Concat(new[] { "Imported", "" }).ToArray());
                }
                catch (System.Exception ex)
                {
                    // If import fails, add an error message
                    results.Add(row.Concat(new[] { "Failed", ex.Message }).ToArray());
                }
            }
            return results;
        }

        private async Task<List<string[]>> ProcessJobImport(List<string[]> csvData)
        {
            var results = new List<string[]>();

            for (int i = 1; i < csvData.Count; i++)
            {
                var row = csvData[i];
                try
                {
                    var job = new Job
                    {
                        JobName = row[0],
                        FilePath = row[1],
                        ExpectedArrivalTime = DateTime.ParseExact(row[2], "HH:mm:ss", CultureInfo.InvariantCulture),
                        CheckIntervalMinutes = int.Parse(row[3]),
                        BoxId = int.Parse(row[4]),
                        IsActive = bool.Parse(row[5]),
                        NotifySourceTeamAutomatically = bool.Parse(row[6]),
                        SourceTeamContact = row[7],
                        AxwayID = row[8],
                        CalendarId = string.IsNullOrEmpty(row[9]) ? (int?)null : int.Parse(row[9]),
                        IgnoreBoxSchedule = bool.Parse(row[10])
                    };

                    _context.Jobs.Add(job);
                    await _context.SaveChangesAsync();
                    // If import is successful, add a success status
                    results.Add(row.Concat(new[] { "Imported", "" }).ToArray());
                }
                catch (System.Exception ex)
                {
                    // If import fails, add an error message
                    results.Add(row.Concat(new[] { "Failed", ex.Message }).ToArray());
                }
            }

            return results;
        }

        private IActionResult ReturnCsvFile(List<string[]> results, string fileName)
        {
            var sb = new StringBuilder();

            foreach (var row in results)
            {
                sb.AppendLine(string.Join(",", row));
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", fileName);
        }

        public IActionResult DownloadTemplate(string templateType)
        {
            string csvContent;
            string fileName;

            if (templateType == "Box")
            {
                csvContent = GenerateBoxTemplate();
                fileName = "BoxTemplate.csv";
            }
            else if (templateType == "Job")
            {
                csvContent = GenerateJobTemplate();
                fileName = "JobTemplate.csv";
            }
            else
            {
                return BadRequest("Invalid template type.");
            }

            var bytes = Encoding.UTF8.GetBytes(csvContent);
            return File(bytes, "text/csv", fileName);
        }

        private string GenerateBoxTemplate()
        {
            var sb = new StringBuilder();
            sb.AppendLine("BoxName,CalendarId,ScheduleTime,IsActive,NotifySourceTeamAutomatically,ExcludeCalendarId");
            sb.AppendLine("Example Box,,14:00:00,true,true,"); // Add an example row
            return sb.ToString();
        }

        private string GenerateJobTemplate()
        {
            var sb = new StringBuilder();
            sb.AppendLine("JobName,FilePath,ExpectedArrivalTime,CheckIntervalMinutes,BoxId,IsActive,NotifySourceTeamAutomatically,SourceTeamContact,AxwayID,CalendarId,IgnoreBoxSchedule");
            sb.AppendLine("Example Job,/path/to/file.txt,15:30:00,30,1,true,true,contact@example.com,AX123,,false"); // Add an example row
            return sb.ToString();
        }
    }
}
