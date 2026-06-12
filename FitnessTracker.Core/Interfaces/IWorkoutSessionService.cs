using FitnessTracker.Core.DTOs;

namespace FitnessTracker.Core.Interfaces
{
    public interface IWorkoutSessionService
    {
        Task<WorkoutSessionDto> StartAsync(string userId, StartWorkoutModel model);
        Task<WorkoutSessionDto?> GetActiveAsync(string userId);
        Task<WorkoutSessionDto?> GetByIdAsync(int id, string userId, bool isAdmin);
        Task<IEnumerable<WorkoutSessionDto>> GetHistoryAsync(string userId);
        Task<WorkoutSetDto?> AddSetAsync(int sessionId, string userId, AddSetInputModel model);
        Task<bool> RemoveSetAsync(int sessionId, int setId, string userId);
        
        /// <summary>
        /// Ends the active session. If <paramref name="save"/> is false, the session
        /// (and its sets) are discarded entirely and null is returned.
        /// </summary>
        /// 
        Task<IEnumerable<WorkoutSessionDto>> GetAllMySessionsAsync(string userId);
        Task<WorkoutSessionDto?> FinishAsync(int sessionId, string userId, bool save);
    }
}