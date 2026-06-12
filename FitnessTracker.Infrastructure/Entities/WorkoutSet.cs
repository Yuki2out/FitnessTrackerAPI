namespace FitnessTracker.Infrastructure.Entities
{
    public class WorkoutSet
    {
        public int Id { get; set; }

        public int WorkoutSessionId { get; set; }
        public WorkoutSession WorkoutSession { get; set; } = null!;

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; } = null!;

        public double WeightUsed { get; set; }
        public int RepsCompleted { get; set; }

        // Order in which the set was added, so the UI can replay the session in sequence
        public int SetOrder { get; set; }
    }
}