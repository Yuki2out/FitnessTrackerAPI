using FitnessTracker.Infrastructure.Enums;

namespace FitnessTracker.Core.DTOs
{
    public class ExerciseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public MuscleGroup MuscleGroup { get; set; } // Changed from string
    }
}