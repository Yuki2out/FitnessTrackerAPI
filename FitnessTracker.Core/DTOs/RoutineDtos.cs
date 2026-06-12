using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Core.DTOs
{
    public class RoutineInputModel
    {
        [Required(ErrorMessage = "Routine name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(300, ErrorMessage = "Description cannot exceed 300 characters.")]
        public string Description { get; set; } = string.Empty;

        public List<int> ExerciseIds { get; set; } = new();
    }

    public class RoutineDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string CreatorId { get; set; } = string.Empty;
        public string CreatorName { get; set; } = string.Empty;
        public List<ExerciseDto> Exercises { get; set; } = new();
    }
}
