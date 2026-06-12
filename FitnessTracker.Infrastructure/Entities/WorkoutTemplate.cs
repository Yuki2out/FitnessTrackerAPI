using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Infrastructure.Entities
{
    public class WorkoutTemplate
    {
        [Key]
        public int Id {get;set;}
        [Required]
        public string Name {get;set;} = string.Empty;

        [Required]
        public string UserId { get;set;} = string.Empty;

        public int ExerciseCount {get;set;}

    }

}
