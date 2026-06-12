using FitnessTracker.Core.DTOs;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Infrastructure;
using FitnessTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Core.Services
{
    public class WorkoutRoutineService : IWorkoutRoutineService
    {
        private readonly ApplicationDbContext _context;

        public WorkoutRoutineService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FitnessTracker.Core.DTOs.ActiveWorkoutDto?> SetupActiveSessionFromTemplateAsync(int templateId, string userId)
        {
            // 1. Fetch the routine template including its direct collection of exercises
            var routine = await _context.WorkoutRoutines
                .Include(r => r.Exercises) 
                .FirstOrDefaultAsync(r => r.Id == templateId);

            // 2. Validate ownership and database existence
            if (routine == null || routine.CreatorId != userId)
            {
                return null; 
            }

            // 3. Map the tracked entities cleanly into your DTO target layout structure
            var activeSessionDto = new FitnessTracker.Core.DTOs.ActiveWorkoutDto
            {
                WorkoutName = routine.Name,
                Exercises = routine.Exercises.Select(ex => new FitnessTracker.Core.DTOs.ExerciseActiveInputDto
                {
                    ExerciseId = ex.Id,
                    Name = ex.Name,
                    Sets = new List<FitnessTracker.Core.DTOs.SetInputDto>
                    {
                        new FitnessTracker.Core.DTOs.SetInputDto { TargetWeight = 0, TargetReps = 0 }
                    }
                }).ToList()
            };

            return activeSessionDto;
        }


        public async Task<IEnumerable<RoutineDto>> GetAllAsync()
        {
            return await _context.WorkoutRoutines
                .Include(r => r.Creator)
                .Include(r => r.Exercises)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task<RoutineDto?> GetByIdAsync(int id)
        {
            var routine = await _context.WorkoutRoutines
                .Include(r => r.Creator)
                .Include(r => r.Exercises)
                .FirstOrDefaultAsync(r => r.Id == id);

            return routine == null ? null : MapToDto(routine);
        }

        public async Task<IEnumerable<RoutineDto>> GetByUserAsync(string userId)
        {
            return await _context.WorkoutRoutines
                .Where(r => r.CreatorId == userId)
                .Include(r => r.Creator)
                .Include(r => r.Exercises)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task<RoutineDto> CreateAsync(string userId, RoutineInputModel model)
        {
            var exercises = await _context.Exercises
                .Where(e => model.ExerciseIds.Contains(e.Id))
                .ToListAsync();

            var routine = new WorkoutRoutine
            {
                Name = model.Name,
                Description = model.Description,
                CreatorId = userId,
                Exercises = exercises
            };

            _context.WorkoutRoutines.Add(routine);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            await _context.Entry(routine).Reference(r => r.Creator).LoadAsync();
            return MapToDto(routine);
        }

        public async Task<RoutineDto?> UpdateAsync(int id, string userId, RoutineInputModel model)
        {
            var routine = await _context.WorkoutRoutines
                .Include(r => r.Creator)
                .Include(r => r.Exercises)
                .FirstOrDefaultAsync(r => r.Id == id && r.CreatorId == userId);

            if (routine == null) return null;

            var exercises = await _context.Exercises
                .Where(e => model.ExerciseIds.Contains(e.Id))
                .ToListAsync();

            routine.Name = model.Name;
            routine.Description = model.Description;
            routine.Exercises = exercises;

            await _context.SaveChangesAsync();
            return MapToDto(routine);
        }

        public async Task<bool> DeleteAsync(int id, string userId, bool isAdmin)
        {
            var routine = await _context.WorkoutRoutines.FindAsync(id);
            if (routine == null) return false;

            // Only the creator or an admin can delete
            if (routine.CreatorId != userId && !isAdmin) return false;

            _context.WorkoutRoutines.Remove(routine);
            await _context.SaveChangesAsync();
            return true;
        }

        private static RoutineDto MapToDto(WorkoutRoutine r) => new()
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            CreatorId = r.CreatorId,
            CreatorName = r.Creator != null ? $"{r.Creator.FirstName} {r.Creator.LastName}" : string.Empty,
            Exercises = r.Exercises.Select(e => new ExerciseDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                MuscleGroup = e.MuscleGroup
            }).ToList()
        };
    }
}