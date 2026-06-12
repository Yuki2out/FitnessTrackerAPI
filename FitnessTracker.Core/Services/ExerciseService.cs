using FitnessTracker.Core.DTOs;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Infrastructure;
using FitnessTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Core.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly ApplicationDbContext _context;

        public ExerciseService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExerciseDto>> GetAllAsync()
        {
            return await _context.Exercises
                .Select(e => MapToDto(e))
                .ToListAsync();
        }

        public async Task<ExerciseDto?> GetByIdAsync(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            return exercise == null ? null : MapToDto(exercise);
        }

        public async Task<ExerciseDto> CreateAsync(ExerciseInputModel model)
        {
            var exercise = new Exercise
            {
                Name = model.Name,
                Description = model.Description,
                MuscleGroup = model.MuscleGroup
            };

            _context.Exercises.Add(exercise);
            await _context.SaveChangesAsync();
            return MapToDto(exercise);
        }

        public async Task<IEnumerable<ExerciseDto>> GetFavoritesByUserAsync(string userId)
        {
            return await _context.UserFavoriteExercises
                .Where(f => f.UserId == userId)
                .Select(f => f.Exercise)
                .Select(ex => new ExerciseDto
                {
                    Id = ex!.Id,
                    Name = ex.Name,
                    Description = ex.Description,
                    MuscleGroup = ex.MuscleGroup
                })
                .ToListAsync();
        }

        public async Task<bool> FavoriteExerciseAsync(string userId, int exerciseId)
        {
            var exists = await _context.UserFavoriteExercises.AnyAsync(f => f.UserId == userId && f.ExerciseId == exerciseId);
            if (exists) return true;
            _context.UserFavoriteExercises.Add(new UserFavoriteExercise
            {
                UserId = userId,
                ExerciseId = exerciseId
            });
            
            return await _context.SaveChangesAsync() > 0  ;      
        }

        public async Task<bool> UnfavoriteExerciseAsync(string userId, int exerciseId)
        {
            var fav = await _context.UserFavoriteExercises.FirstOrDefaultAsync(f => f.UserId ==  userId && f.ExerciseId == exerciseId);
            _context.UserFavoriteExercises.Remove(fav);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> IsFavoritedAsync(string userId,int exerciseId)
        {
            
            return await _context.UserFavoriteExercises.AnyAsync(f => f.UserId == userId && f.ExerciseId == exerciseId);
        }


        public async Task<ExerciseDto?> UpdateAsync(int id, ExerciseInputModel model)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null) return null;

            exercise.Name = model.Name;
            exercise.Description = model.Description;
            exercise.MuscleGroup = model.MuscleGroup;

            await _context.SaveChangesAsync();
            return MapToDto(exercise);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var exercise = await _context.Exercises.FindAsync(id);
            if (exercise == null) return false;

            _context.Exercises.Remove(exercise);
            await _context.SaveChangesAsync();
            return true;
        }

        private static ExerciseDto MapToDto(Exercise e) => new()
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            MuscleGroup = e.MuscleGroup
        };
    }
}