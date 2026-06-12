namespace FitnessTracker.Infrastructure.Entities
{
    public class WorkoutSession
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public string? Name { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public bool IsCompleted { get; set; }

        public ICollection<WorkoutSet> Sets { get; set; } = new List<WorkoutSet>();
    }
}