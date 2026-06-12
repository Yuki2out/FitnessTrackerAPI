using System.Security.Claims;
using FitnessTracker.Core.DTOs;
using FitnessTracker.Core.Interfaces;
using FitnessTracker.Infrastructure;
using FitnessTracker.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Core.Services
{
    public class WorkoutTemplateService : IWorkoutTemplateService
    {
        private readonly ApplicationDbContext _context;

        public WorkoutTemplateService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteTemplateAsync(string userId, int templateId)
{
            var template = await _context.WorkoutTemplates
                .FirstOrDefaultAsync(t => t.Id == templateId && t.UserId == userId);

            if (template == null) return false;

            _context.WorkoutTemplates.Remove(template);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CreateFromHistoryAsync(string userId, CreateTemplateFromHistoryDto dto)
        {
            // 1. Grab the historical session along with its logged sets
            var session = await _context.WorkoutSessions
                .Include(s => s.Sets)
                .FirstOrDefaultAsync(s => s.Id == dto.WorkoutSessionId && s.UserId == userId);

            if (session == null) return false;

            // 2. Count how many unique exercises were executed in this past workout
            var uniqueExerciseCount = session.Sets
                .Select(s => s.ExerciseId)
                .Distinct()
                .Count();

            // 3. Map directly to your clean flat entity properties
            var template = new WorkoutTemplate
            {
                Name = dto.Name,
                UserId = userId,
                ExerciseCount = uniqueExerciseCount,
                PastWorkoutSessionId = dto.WorkoutSessionId // // Matches your backend integer property!
            };

            _context.WorkoutTemplates.Add(template);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<WorkoutTemplate>> GetUserTemplatesAsync(string userId)
        {
            // Simple flat fetch matching your exact entity layout
            return await _context.WorkoutTemplates
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }
    }
}