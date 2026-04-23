using HackathonApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HackathonApp.Data.Services
{
    // DTO for Q9
    public class CategoryCount
    {
        public string Category { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    // DTO for Q11
    public class CategoryAverage
    {
        public string Category { get; set; } = string.Empty;
        public decimal AvgScore { get; set; }
    }

    // DTO for Q14 (one row per project)
    public class CategoryTopProject
    {
        public string Category { get; set; } = string.Empty;
        public int Id { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string ProjectName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public decimal Score { get; set; }
        public int Members { get; set; }
        public string Captain { get; set; } = string.Empty;
    }

    public class QueryService
    {
        private readonly HackathonDbContext _context;

        public QueryService(HackathonDbContext context)
        {
            _context = context;
        }

        // SIMPLE QUERIES -----------------------------

        public async Task<List<Project>> GetByTeamNameAsync(string team) =>
            await _context.Projects.Where(p => p.TeamName == team).ToListAsync();

        public async Task<List<Project>> GetByDateAsync(DateTime date) =>
            await _context.Projects.Where(p => p.EventDate == date).ToListAsync();

        public async Task<List<Project>> GetByCategoryAsync(string category) =>
            await _context.Projects.Where(p => p.Category == category).ToListAsync();

        public async Task<List<Project>> GetHighScoreAsync(decimal score) =>
            await _context.Projects.Where(p => p.Score > score).ToListAsync();

        public async Task<List<Project>> GetTop5Async() =>
            await _context.Projects.OrderByDescending(p => p.Score).Take(5).ToListAsync();


        // MEDIUM QUERIES -----------------------------

        public async Task<List<Project>> GetByYearAsync(int year) =>
            await _context.Projects.Where(p => p.EventDate.Year == year).ToListAsync();

        public async Task<List<Project>> GetHealthTech88Async() =>
            await _context.Projects
                .Where(p => p.Category == "HealthTech" && p.Score > 88)
                .ToListAsync();

        public async Task<List<Project>> GetSortedAsync() =>
            await _context.Projects
                .OrderBy(p => p.EventDate)
                .ThenByDescending(p => p.Score)
                .ToListAsync();

        public async Task<List<CategoryCount>> GetCountPerCategoryAsync() =>
            await _context.Projects
                .GroupBy(p => p.Category)
                .Select(g => new CategoryCount
                {
                    Category = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

        public async Task<List<Project>> GetTop3ByTeamAsync(string team) =>
            await _context.Projects
                .Where(p => p.TeamName == team)
                .OrderByDescending(p => p.Score)
                .Take(3)
                .ToListAsync();


        // COMPLEX QUERIES -----------------------------

        public async Task<List<CategoryAverage>> GetAveragePerCategoryAsync() =>
            await _context.Projects
                .GroupBy(p => p.Category)
                .Select(g => new CategoryAverage
                {
                    Category = g.Key,
                    AvgScore = g.Average(x => x.Score)
                })
                .ToListAsync();

        public async Task<List<Project>> GetSmartCityEnergyAboveAvgAsync()
        {
            var averages = await _context.Projects
                .GroupBy(p => p.Category)
                .Select(g => new { g.Key, Avg = g.Average(x => x.Score) })
                .ToListAsync();

            var avgDict = averages.ToDictionary(x => x.Key, x => x.Avg);
            var valid = new[] { "SmartCity", "Energy" };

            return await _context.Projects
                .Where(p => valid.Contains(p.Category) && p.Score >= avgDict[p.Category])
                .ToListAsync();
        }

        public async Task<List<Project>> GetAIProjectsHighScoreAsync() =>
            await _context.Projects
                .Where(p => p.ProjectName.Contains("AI") && p.Score > 92)
                .ToListAsync();

        // ★ FIXED (EF-safe) Q14
        public async Task<List<CategoryTopProject>> GetTop5EachCategoryAsync()
        {
            var all = await _context.Projects.ToListAsync();

            return all
                .GroupBy(p => p.Category)
                .SelectMany(g => g
                    .OrderByDescending(x => x.Score)
                    .Take(5)
                    .Select(p => new CategoryTopProject
                    {
                        Category = g.Key,
                        Id = p.Id,
                        TeamName = p.TeamName,
                        ProjectName = p.ProjectName,
                        EventDate = p.EventDate,
                        Score = p.Score,
                        Members = p.Members,
                        Captain = p.Captain
                    }))
                .ToList();
        }

        public async Task<List<Project>> GetLargeTeamsAboveGlobalAvgAsync()
        {
            var avg = await _context.Projects.AverageAsync(p => p.Score);

            return await _context.Projects
                .Where(p => p.Members >= 5 && p.Score > avg)
                .ToListAsync();
        }
    }
}
