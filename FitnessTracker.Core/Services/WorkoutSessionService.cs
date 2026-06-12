using FitnessTracker.Core.DTOs;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Infrastructure;
using FitnessTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Core.Services
{
    public class WorkoutSessionService : IWorkoutSessionService
    {
        private readonly ApplicationDbContext _context;

        public WorkoutSessionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WorkoutSessionDto> StartAsync(string userId, StartWorkoutModel model)
        {
            // If the user already has an active (unfinished) session, return that
            // instead of creating a duplicate.
            var existingActive = await _context.WorkoutSessions
                .Include(s => s.Sets).ThenInclude(set => set.Exercise)
                .Where(s => s.UserId == userId && !s.IsCompleted)
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefaultAsync();

            if (existingActive != null)
            {
                return MapToDto(existingActive);
            }

            var session = new WorkoutSession
            {
                UserId = userId,
                Name = model.Name,
                StartTime = DateTime.UtcNow,
                IsCompleted = false
            };

            _context.WorkoutSessions.Add(session);
            await _context.SaveChangesAsync();

            return MapToDto(session);
        }

        public async Task<WorkoutSessionDto?> GetActiveAsync(string userId)
        {
            var session = await _context.WorkoutSessions
                .Include(s => s.Sets).ThenInclude(set => set.Exercise)
                .Where(s => s.UserId == userId && !s.IsCompleted)
                .OrderByDescending(s => s.StartTime)
                .FirstOrDefaultAsync();

            return session == null ? null : MapToDto(session);
        }

        public async Task<WorkoutSessionDto?> GetByIdAsync(int id, string userId, bool isAdmin)
        {
            var session = await _context.WorkoutSessions
                .Include(s => s.Sets).ThenInclude(set => set.Exercise)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (session == null) return null;
            if (session.UserId != userId && !isAdmin) return null;

            return MapToDto(session);
        }

        public async Task<IEnumerable<WorkoutSessionDto>> GetHistoryAsync(string userId)
        {
            // Leverages the clean mapping projection engine below
            return await GetAllMySessionsAsync(userId);
        }

        public async Task<WorkoutSetDto?> AddSetAsync(int sessionId, string userId, AddSetInputModel model)
        {
            var session = await _context.WorkoutSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId && !s.IsCompleted);

            if (session == null) return null;

            var exercise = await _context.Exercises.FindAsync(model.ExerciseId);
            if (exercise == null) return null;

            var nextOrder = await _context.WorkoutSets
                .Where(s => s.WorkoutSessionId == sessionId)
                .CountAsync() + 1;

            var set = new WorkoutSet
            {
                WorkoutSessionId = sessionId,
                ExerciseId = model.ExerciseId,
                WeightUsed = model.WeightUsed,
                RepsCompleted = model.RepsCompleted,
                SetOrder = nextOrder
            };

            _context.WorkoutSets.Add(set);
            await _context.SaveChangesAsync();

            return new WorkoutSetDto
            {
                Id = set.Id,
                ExerciseId = exercise.Id,
                ExerciseName = exercise.Name,
                WeightUsed = set.WeightUsed,
                RepsCompleted = set.RepsCompleted,
                SetOrder = set.SetOrder
            };
        }

        public async Task<bool> RemoveSetAsync(int sessionId, int setId, string userId)
        {
            var session = await _context.WorkoutSessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId && !s.IsCompleted);
            if (session == null) return false;

            var set = await _context.WorkoutSets
                .FirstOrDefaultAsync(s => s.Id == setId && s.WorkoutSessionId == sessionId);
            if (set == null) return false;

            _context.WorkoutSets.Remove(set);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<WorkoutSessionDto?> FinishAsync(int sessionId, string userId, bool save)
        {
            var session = await _context.WorkoutSessions
                .Include(s => s.Sets).ThenInclude(set => set.Exercise)
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId && !s.IsCompleted);

            if (session == null) return null;

            if (!save)
            {
                _context.WorkoutSessions.Remove(session);
                await _context.SaveChangesAsync();
                return null;
            }

            session.EndTime = DateTime.UtcNow;
            session.IsCompleted = true;
            await _context.SaveChangesAsync();

            return MapToDto(session);
        }

        public async Task<IEnumerable<WorkoutSessionDto>> GetAllMySessionsAsync(string userId)
        {
            return await _context.WorkoutSessions
                .Where(s => s.UserId == userId && s.IsCompleted)
                .OrderByDescending(s => s.EndTime)
                .Select(s => new WorkoutSessionDto
                {
                    Id = s.Id,
                    UserId = s.UserId,
                    Name = s.Name,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    IsCompleted = s.IsCompleted,
                    Sets = s.Sets.OrderBy(set => set.SetOrder).Select(set => new WorkoutSetDto
                    {
                        Id = set.Id,
                        ExerciseId = set.ExerciseId,
                        ExerciseName = set.Exercise != null ? set.Exercise.Name : "Unknown Exercise",
                        WeightUsed = set.WeightUsed,
                        RepsCompleted = set.RepsCompleted,
                        SetOrder = set.SetOrder
                    }).ToList()
                })
                .ToListAsync();
        }

        // ─── HELPER METHOD: OBJECT TO DTO MAPPER ───
        private static WorkoutSessionDto MapToDto(WorkoutSession session)
        {
            return new WorkoutSessionDto
            {
                Id = session.Id,
                UserId = session.UserId,
                Name = session.Name,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                IsCompleted = session.IsCompleted,
                Sets = session.Sets == null 
                    ? new List<WorkoutSetDto>()
                    : session.Sets.OrderBy(set => set.SetOrder).Select(set => new WorkoutSetDto
                      {
                          Id = set.Id,
                          ExerciseId = set.ExerciseId,
                          ExerciseName = set.Exercise != null ? set.Exercise.Name : "Unknown Exercise",
                          WeightUsed = set.WeightUsed,
                          RepsCompleted = set.RepsCompleted,
                          SetOrder = set.SetOrder
                      }).ToList()
            };
        }
    }
}