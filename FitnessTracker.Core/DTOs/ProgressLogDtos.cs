using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Core.DTOs
{
    public class ProgressLogInputModel
    {
        [Required(ErrorMessage = "Exercise ID is required.")]
        public int ExerciseId { get; set; }

        [Range(0, 1000.0, ErrorMessage = "Weight must be between 0 and 1000 kg.")]
        public double WeightUsed { get; set; }

        [Range(1, 200, ErrorMessage = "Reps must be between 1 and 200.")]
        public int RepsCompleted { get; set; }
    }

    public class ProgressLogDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public int ExerciseId { get; set; }
        public string ExerciseName { get; set; } = string.Empty;
        public double WeightUsed { get; set; }
        public int RepsCompleted { get; set; }
        public DateTime LogDate { get; set; }
    }


    // 1. Used for rendering items inside the "Past Workouts" historical list column
    public class PastWorkoutDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime EndTime { get; set; }
    }

    // 2. Used for rendering items inside the "Favorite Exercises" quick list column
    public class FavoriteExerciseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // 3. The macro engine payload that initializes the active workout session view state
    public class ActiveWorkoutDto
    {
        public string WorkoutName { get; set; } = string.Empty;
        public List<ExerciseActiveInputDto> Exercises { get; set; } = new();
    }

    // 4. Group blocks containing logs split clean across exercise targets
    public class ExerciseActiveInputDto
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<SetInputDto> Sets { get; set; } = new();
    }

    // 5. The base weight-rep tracking placeholders
    public class SetInputDto
    {
        public decimal TargetWeight { get; set; }
        public int TargetReps { get; set; }
    }

}