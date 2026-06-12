using FitnessTracker.Core.DTOs;

public interface IWorkoutRoutineService
{
    Task<IEnumerable<RoutineDto>> GetAllAsync();
    Task<RoutineDto?> GetByIdAsync(int id);
    Task<IEnumerable<RoutineDto>> GetByUserAsync(string userId);
    Task<RoutineDto> CreateAsync(string userId, RoutineInputModel model);
    Task<RoutineDto?> UpdateAsync(int id, string userId, RoutineInputModel model);
    Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
    
    // Add this line so your controller can build active sets from routine templates!
    Task<ActiveWorkoutDto?> SetupActiveSessionFromTemplateAsync(int templateId, string userId);
}