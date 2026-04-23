using HackathonApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Xml.Linq;

namespace HackathonApp.Data.Services
{
    // Delegate for reporting import results
    public delegate void DataImportedHandler(int inserted, int updated, int skipped, TimeSpan duration);
    // Service for importing hackathon projects from XML into the database
    public class ImportService
    {
        private readonly HackathonDbContext _context;
        // Event triggered after import completes
        public event DataImportedHandler? DataImported;

        public ImportService(HackathonDbContext context)
        {
            _context = context;
        }
        // Import projects from XML file asynchronously
        public async Task ImportFromXmlAsync(string filePath)
        {
            var stopwatch = Stopwatch.StartNew();
            int inserted = 0, updated = 0, skipped = 0;

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return;
            }

            XDocument doc = XDocument.Load(filePath);
            var projects = doc.Root?.Elements("Project");

            if (projects == null)
            {
                Console.WriteLine("No project data found in XML file.");
                return;
            }

            foreach (var p in projects)
            {
                try
                {
                    var project = new Project
                    {
                        Id = int.Parse(p.Element("Id")!.Value),
                        TeamName = p.Element("TeamName")!.Value,
                        ProjectName = p.Element("ProjectName")!.Value,
                        Category = p.Element("Category")!.Value,
                        EventDate = DateTime.Parse(p.Element("EventDate")!.Value),
                        Score = decimal.Parse(p.Element("Score")!.Value),
                        Members = int.Parse(p.Element("Members")!.Value),
                        Captain = p.Element("Captain")!.Value
                    };

                    // Validation
                    if (project.Id <= 0 || string.IsNullOrWhiteSpace(project.TeamName) ||
                        string.IsNullOrWhiteSpace(project.ProjectName) ||
                        string.IsNullOrWhiteSpace(project.Category) ||
                        string.IsNullOrWhiteSpace(project.Captain) ||
                        project.Members < 1 || project.Members > 15 ||
                        project.Score < 0 || project.Score > 100 ||
                        project.EventDate > DateTime.Now)
                    {
                        skipped++;
                        continue;
                    }

                    var existing = await _context.Projects.FindAsync(project.Id);
                    if (existing == null)
                    {
                        await _context.Projects.AddAsync(project);
                        inserted++;
                    }
                    else
                    {
                        _context.Entry(existing).CurrentValues.SetValues(project);
                        updated++;
                    }
                }
                catch
                {
                    skipped++;  // Skip invalid records
                }
            }

            await _context.SaveChangesAsync();
            stopwatch.Stop();
            // Trigger event after import finishes
            DataImported?.Invoke(inserted, updated, skipped, stopwatch.Elapsed);
        }
    }
}
