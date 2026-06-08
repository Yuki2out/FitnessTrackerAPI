using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessTracker.Infrastructure.Entities
{
    public class ProgressLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser? User { get; set; }

        [Required]
        public int ExerciseId { get; set; }

        [ForeignKey(nameof(ExerciseId))]
        public Exercise? Exercise { get; set; }

        [Range(0.1, 1000.0)]
        public double WeightUsed { get; set; }

        [Range(1, 200)]
        public int RepsCompleted { get; set; }

        public DateTime LogDate { get; set; }
    }
}