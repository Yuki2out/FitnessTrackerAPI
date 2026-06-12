using FitnessTracker.Core.DTOs;
using FitnessTracker.Infrastructure.Entities;

namespace FitnessTracker.Core.Interfaces
{
    public interface IExerciseService
    {
        Task<IEnumerable<ExerciseDto>> GetAllAsync();
        Task<ExerciseDto?> GetByIdAsync(int id);
        Task<ExerciseDto> CreateAsync(ExerciseInputModel model);
        Task<ExerciseDto?> UpdateAsync(int id, ExerciseInputModel model);
        Task<bool> FavoriteExerciseAsync(string userId, int exerciseId);
        Task<bool> UnfavoriteExerciseAsync(string userId, int exerciseId);
        Task<bool> IsFavoritedAsync(string userId,int exerciseId);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ExerciseDto>> GetFavoritesByUserAsync(string userId);
    }

    public interface IWorkoutRoutineService
    {
        Task<IEnumerable<RoutineDto>> GetAllAsync();
        Task<RoutineDto?> GetByIdAsync(int id);
        Task<IEnumerable<RoutineDto>> GetByUserAsync(string userId);
        Task<RoutineDto> CreateAsync(string userId, RoutineInputModel model);
        Task<RoutineDto?> UpdateAsync(int id, string userId, RoutineInputModel model);
        Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
    }


  

    public interface IProgressLogService
    {
        Task<IEnumerable<ProgressLogDto>> GetAllAsync();
        Task<IEnumerable<ProgressLogDto>> GetByUserAsync(string userId);
        Task<ProgressLogDto?> GetByIdAsync(int id);
        Task<ProgressLogDto> CreateAsync(string userId, ProgressLogInputModel model);
        Task<bool> DeleteAsync(int id, string userId, bool isAdmin);
    }

    public interface IAuthService
    {
        Task<AuthResponseDto?> RegisterAsync(RegisterModel model);
        Task<AuthResponseDto?> LoginAsync(LoginModel model);
    }

    public interface IWorkoutLogService
    {
        Task<IEnumerable<PastWorkoutDto>> GetUserHistoryAsync(string userId);
        Task<IEnumerable<FavoriteExerciseDto>> GetUserFavoritesAsync(string userId);
        Task<ActiveWorkoutDto?> SetupActiveSessionFromPastWorkoutAsync(int pastWorkoutId, string userId);
    }
}
