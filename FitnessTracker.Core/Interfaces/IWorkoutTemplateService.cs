using FitnessTracker.Infrastructure.Entities;

namespace FitnessTracker.Core.Interfaces
{
    public interface IWorkoutTemplateService
    {
        Task<IEnumerable<WorkoutTemplate>> GetUserTemplatesAsync(string userId);
    }
}