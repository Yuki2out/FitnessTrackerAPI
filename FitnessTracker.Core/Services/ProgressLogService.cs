using FitnessTracker.Core.DTOs;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Infrastructure;
using FitnessTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Core.Services
{
    public class ProgressLogService : IProgressLogService
    {
        private readonly ApplicationDbContext _context;

        public ProgressLogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProgressLogDto>> GetAllAsync()
        {
            return await _context.ProgressLogs
                .Include(p => p.User)
                .Include(p => p.Exercise)
                .Select(p => MapToDto(p))
                .ToListAsync();
        }

        public async Task<IEnumerable<ProgressLogDto>> GetByUserAsync(string userId)
        {
            return await _context.ProgressLogs
                .Where(p => p.UserId == userId)
                .Include(p => p.User)
                .Include(p => p.Exercise)
                .OrderByDescending(p => p.LogDate)
                .Select(p => MapToDto(p))
                .ToListAsync();
        }

        public async Task<ProgressLogDto?> GetByIdAsync(int id)
        {
            var log = await _context.ProgressLogs
                .Include(p => p.User)
                .Include(p => p.Exercise)
                .FirstOrDefaultAsync(p => p.Id == id);

            return log == null ? null : MapToDto(log);
        }

        public async Task<ProgressLogDto> CreateAsync(string userId, ProgressLogInputModel model)
        {
            var log = new ProgressLog
            {
                UserId = userId,
                ExerciseId = model.ExerciseId,
                WeightUsed = model.WeightUsed,
                RepsCompleted = model.RepsCompleted,
                LogDate = DateTime.UtcNow
            };

            _context.ProgressLogs.Add(log);
            await _context.SaveChangesAsync();

            await _context.Entry(log).Reference(p => p.User).LoadAsync();
            await _context.Entry(log).Reference(p => p.Exercise).LoadAsync();
            return MapToDto(log);
        }

        public async Task<bool> DeleteAsync(int id, string userId, bool isAdmin)
        {
            var log = await _context.ProgressLogs.FindAsync(id);
            if (log == null) return false;

            if (log.UserId != userId && !isAdmin) return false;

            _context.ProgressLogs.Remove(log);
            await _context.SaveChangesAsync();
            return true;
        }

        private static ProgressLogDto MapToDto(ProgressLog p) => new()
        {
            Id = p.Id,
            UserId = p.UserId,
            UserFullName = p.User != null ? $"{p.User.FirstName} {p.User.LastName}" : string.Empty,
            ExerciseId = p.ExerciseId,
            ExerciseName = p.Exercise?.Name ?? string.Empty,
            WeightUsed = p.WeightUsed,
            RepsCompleted = p.RepsCompleted,
            LogDate = p.LogDate
        };
    }
}