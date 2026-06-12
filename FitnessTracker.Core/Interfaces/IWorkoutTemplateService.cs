using FitnessTracker.Infrastructure.Entities;
using FitnessTracker.Core.DTOs; 
namespace FitnessTracker.Core.Interfaces
{
    public interface IWorkoutTemplateService
    {
        Task<IEnumerable<WorkoutTemplate>> GetUserTemplatesAsync(string userId);
        Task<bool> DeleteTemplateAsync(string userId, int templateId);
        Task<bool> CreateFromHistoryAsync(string userId, CreateTemplateFromHistoryDto dto);
    }
}