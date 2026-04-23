using HackathonApp.Data;
using HackathonApp.Data.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HackathonApp.ConsoleApp
{
    internal class Program
    {
        private static async Task Main()
        {
            var options = new DbContextOptionsBuilder<HackathonDbContext>()
                .UseInMemoryDatabase("HackathonDB")
                .Options;

            using var context = new HackathonDbContext(options);
            var importService = new ImportService(context);
            var queryService = new QueryService(context);

            importService.DataImported += (inserted, updated, skipped, duration) =>
            {
                Console.WriteLine(
                    $"\nImport complete: {inserted} inserted, {updated} updated, {skipped} skipped in {duration.TotalSeconds:F2}s\n");
            };

            bool running = true;

            while (running)
            {
                Console.WriteLine("===== Hackathon Results Management System =====");
                Console.WriteLine("1) Import XML -> Database");
                Console.WriteLine("2) Run SIMPLE LINQ queries (Q1–Q5) + export to JSON");
                Console.WriteLine("3) Run MEDIUM LINQ queries (Q6–Q10) + export to JSON");
                Console.WriteLine("4) Run COMPLEX LINQ queries (Q11–Q15) + export to JSON");
                Console.WriteLine("0) Exit");
                Console.Write("\nChoose an option: ");

                var choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        await RunImport(importService);
                        break;

                    case "2":
                        await RunSimpleQueries(queryService);
                        break;

                    case "3":
                        await RunMediumQueries(queryService);
                        break;

                    case "4":
                        await RunComplexQueries(queryService);
                        break;

                    case "0":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid option.\n");
                        break;
                }
            }
        }

        // ---------- helper: one place for JSON export ----------
        private static async Task ExportJsonAsync(string fileName, object data)
        {
            Directory.CreateDirectory("Output");
            var options = new JsonSerializerOptions { WriteIndented = true };

            string fullPath = Path.Combine("Output", fileName);
            await File.WriteAllTextAsync(fullPath, JsonSerializer.Serialize(data, options));
        }

        // ---------- IMPORT ----------
        private static async Task RunImport(ImportService importService)
        {
            var xmlPath = Path.Combine(AppContext.BaseDirectory, "Data", "HackathonResults.xml");
            Console.WriteLine($"Using XML file: {xmlPath}");
            await importService.ImportFromXmlAsync(xmlPath);
            Console.WriteLine();
        }

        // ---------- SIMPLE QUERIES (Q1–Q5) ----------
        private static async Task RunSimpleQueries(QueryService qs)
        {
            Console.WriteLine(">>> SIMPLE QUERIES (1–5)\n");

            var q1 = await qs.GetByTeamNameAsync("NeuralNova");
            Console.WriteLine("Q1 – All projects by team 'NeuralNova'");
            ConsoleTable.Print(q1);
            await ExportJsonAsync("q01_neuralnova.json", q1);

            var q2 = await qs.GetByDateAsync(new DateTime(2025, 10, 12));
            Console.WriteLine("Q2 – Projects submitted on 2025-10-12");
            ConsoleTable.Print(q2);
            await ExportJsonAsync("q02_events_2025_10_12.json", q2);

            var q3 = await qs.GetByCategoryAsync("AI-ML");
            Console.WriteLine("Q3 – AI-ML category");
            ConsoleTable.Print(q3);
            await ExportJsonAsync("q03_ai_ml_projects.json", q3);

            var q4 = await qs.GetHighScoreAsync(90);
            Console.WriteLine("Q4 – Projects with Score > 90");
            ConsoleTable.Print(q4);
            await ExportJsonAsync("q04_score_above_90.json", q4);

            var q5 = await qs.GetTop5Async();
            Console.WriteLine("Q5 – Top 5 highest-scoring projects overall");
            ConsoleTable.Print(q5);
            await ExportJsonAsync("q05_top5_overall.json", q5);

            Console.WriteLine("Simple query results exported to JSON (Q1–Q5).\n");
        }

        // ---------- MEDIUM QUERIES (Q6–Q10) ----------
        private static async Task RunMediumQueries(QueryService qs)
        {
            Console.WriteLine(">>> MEDIUM QUERIES (6–10)\n");

            var q6 = await qs.GetByYearAsync(2024);
            Console.WriteLine("Q6 – Projects submitted in 2024");
            ConsoleTable.Print(q6);
            await ExportJsonAsync("q06_year_2024_projects.json", q6);

            var q7 = await qs.GetHealthTech88Async();
            Console.WriteLine("Q7 – HealthTech projects with Score > 88");
            ConsoleTable.Print(q7);
            await ExportJsonAsync("q07_healthtech_over_88.json", q7);

            var q8 = await qs.GetSortedAsync();
            Console.WriteLine("Q8 – Projects sorted by EventDate asc, Score desc");
            ConsoleTable.Print(q8);
            await ExportJsonAsync("q08_sorted_by_date_score.json", q8);

            var q9 = await qs.GetCountPerCategoryAsync();
            Console.WriteLine("Q9 – Count of projects per category");
            ConsoleTable.Print(q9);
            await ExportJsonAsync("q09_count_per_category.json", q9);

            var q10 = await qs.GetTop3ByTeamAsync("ByteForge");
            Console.WriteLine("Q10 – Top 3 projects by 'ByteForge'");
            ConsoleTable.Print(q10);
            await ExportJsonAsync("q10_top3_byteforge.json", q10);

            Console.WriteLine("Medium query results exported to JSON (Q6–Q10).\n");
        }

        // ---------- COMPLEX QUERIES (Q11–Q15) ----------
        private static async Task RunComplexQueries(QueryService qs)
        {
            Console.WriteLine(">>> COMPLEX QUERIES (11–15)\n");

            var q11 = await qs.GetAveragePerCategoryAsync();
            Console.WriteLine("Q11 – Average score per category");
            ConsoleTable.Print(q11);
            await ExportJsonAsync("q11_category_average_scores.json", q11);

            var q12 = await qs.GetSmartCityEnergyAboveAvgAsync();
            Console.WriteLine("Q12 – SmartCity/Energy projects with score ≥ category average");
            ConsoleTable.Print(q12);
            await ExportJsonAsync("q12_smartcity_energy_above_avg.json", q12);

            var q13 = await qs.GetAIProjectsHighScoreAsync();
            Console.WriteLine("Q13 – Projects whose name contains 'AI' and Score > 92");
            ConsoleTable.Print(q13);
            await ExportJsonAsync("q13_ai_projects_over_92.json", q13);

            var q14 = await qs.GetTop5EachCategoryAsync();
            Console.WriteLine("Q14 – Top 5 by score within each category");
            ConsoleTable.Print(q14);
            await ExportJsonAsync("q14_top5_each_category.json", q14);

            var q15 = await qs.GetLargeTeamsAboveGlobalAvgAsync();
            Console.WriteLine("Q15 – Projects where Members ≥ 5 AND Score > global average");
            ConsoleTable.Print(q15);
            await ExportJsonAsync("q15_large_teams_above_avg.json", q15);

            Console.WriteLine("Complex query results exported to JSON (Q11–Q15).\n");
        }
    }
}
