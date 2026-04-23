using System;
using System.ComponentModel.DataAnnotations;

namespace HackathonApp.Data.Entities
{
    // Represents a hackathon project entry in the database
    public class Project
    {
        [Key]
        public int Id { get; set; } // Primary key

        [Required, MaxLength(100)]
        public string TeamName { get; set; } = string.Empty; // Name of the team

        [Required, MaxLength(120)]
        public string ProjectName { get; set; } = string.Empty; // Name of the project

        [Required, MaxLength(50)]
        public string Category { get; set; } = string.Empty; // Project category (e.g., AI, HealthTech)

        [Required]
        public DateTime EventDate { get; set; } // Date of the hackathon event

        [Range(0.0, 100.0)]
        public decimal Score { get; set; } // Project score (0-100)

        [Range(1, 15)]
        public int Members { get; set; } // Number of team members

        [Required, MaxLength(100)]
        public string Captain { get; set; } = string.Empty; // Team captain's name
    }
}
