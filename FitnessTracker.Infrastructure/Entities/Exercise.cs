using System.ComponentModel.DataAnnotations;
using FitnessTracker.Infrastructure.Enums; // Add this using statement

namespace FitnessTracker.Infrastructure.Entities
{
    public class Exercise
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public MuscleGroup MuscleGroup { get; set; } // Changed from string
    }
}