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