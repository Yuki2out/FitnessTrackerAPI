using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Core.DTOs
{
    public class StartWorkoutModel
    {
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }
    }

    public class AddSetInputModel
    {
        [Required(ErrorMessage = "Exercise ID is required.")]
        public int ExerciseId { get; set; }

        [Range(0.1, 1000.0, ErrorMessage = "Weight must be between 0.1 and 1000 kg.")]
        public double WeightUsed { get; set; }

        [Range(1, 200, ErrorMessage = "Reps must be between 1 and 200.")]
        public int RepsCompleted { get; set; }
    }

    public class WorkoutSetDto
    {
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public double WeightUsed { get; set; }
        public int RepsCompleted { get; set; }
        public int SetOrder { get; set; }
    }

    public class WorkoutSessionDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? Name { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public bool IsCompleted { get; set; }
        public List<WorkoutSetDto> Sets { get; set; } = new();
    }
}