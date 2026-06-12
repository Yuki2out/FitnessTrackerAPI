using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<WorkoutTemplate>> GetUserTemplatesAsync(string userId)
        {
            return await _context.WorkoutTemplates
                .Where(t => t.UserId == userId)
                .Select(t => new WorkoutTemplate
                {
                    Id = t.Id,
                    Name = t.Name,
                    UserId = t.UserId,
                    ExerciseCount = t.ExerciseCount
                })
                .ToListAsync();
        }

    }
}