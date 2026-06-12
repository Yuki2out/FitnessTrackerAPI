using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

    public class UserFavoriteExercise
    {
        [Key]
        public int Id {get; set;}

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int ExerciseId { get; set;}

        [ForeignKey(nameof(ExerciseId))]
        public Exercise? Exercise {get; set;}
        
    }
}