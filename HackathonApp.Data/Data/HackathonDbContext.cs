using HackathonApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace HackathonApp.Data
{
    // Database context for Hackathon application
    // Handles connection to the database and entity configuration
    public class HackathonDbContext : DbContext
    {
        public DbSet<Project> Projects { get; set; } // Projects table

        public HackathonDbContext(DbContextOptions<HackathonDbContext> options)
            : base(options) { }
        // Configure entity properties and database column types
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Store Score as decimal with 5 digits, 2 decimal places
            modelBuilder.Entity<Project>().Property(p => p.Score).HasColumnType("decimal(5,2)");
        }
    }
}
