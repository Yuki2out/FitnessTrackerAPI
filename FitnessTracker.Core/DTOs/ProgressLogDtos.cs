using System.ComponentModel.DataAnnotations;
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


    
    public class PastWorkoutDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime EndTime { get; set; }
    }

    
    public class FavoriteExerciseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    
    public class ActiveWorkoutDto
    {
        public string WorkoutName { get; set; } = string.Empty;
        public List<ExerciseActiveInputDto> Exercises { get; set; } = new();
    }

   
    public class ExerciseActiveInputDto
    {
        public int ExerciseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<SetInputDto> Sets { get; set; } = new();
    }

    
    public class SetInputDto
    {
        public decimal TargetWeight { get; set; }
        public int TargetReps { get; set; }
    }


    

    public class CreateTemplateFromHistoryDto
    {
        [Required]
        public int WorkoutSessionId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
    }
    

}