using System.ComponentModel.DataAnnotations;
using FitnessTracker.Infrastructure.Enums;

namespace FitnessTracker.Core.DTOs
{
    public class ExerciseInputModel
    {
        [Required(ErrorMessage = "Exercise name is mandatory.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is mandatory.")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Muscle group specification is mandatory.")]
        [EnumDataType(typeof(MuscleGroup), ErrorMessage = "Invalid muscle group selected.")] // Enforces exact selection match
        public MuscleGroup MuscleGroup { get; set; } // Changed from string
    }
}