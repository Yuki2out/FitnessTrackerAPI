using FitnessTracker.Infrastructure;
using FitnessTracker.Infrastructure.Entities;
using FitnessTracker.Infrastructure.Enums;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Tests
{
    public static class TestDbContextFactory
    {
        public static ApplicationDbContext Create(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureCreated();

            if (!context.Exercises.Any())
            {
                context.Exercises.AddRange(
                    new Exercise { Id = 1, Name = "Bench Press", Description = "Classic chest pressing exercise", MuscleGroup = MuscleGroup.Chest },
                    new Exercise { Id = 2, Name = "Squat", Description = "Barbell back squat exercise", MuscleGroup = MuscleGroup.Legs }
                );
                context.SaveChanges();
            }

            // Seed test users so .Include(p => p.User) / .Include(r => r.Creator)
            // don't drop rows in the InMemory provider (required-nav behaves like inner join)
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new ApplicationUser { Id = "user-1", UserName = "user1@test.com", Email = "user1@test.com", FirstName = "Alice", LastName = "Anderson" },
                    new ApplicationUser { Id = "user-2", UserName = "user2@test.com", Email = "user2@test.com", FirstName = "Bob", LastName = "Brown" }
                );
                context.SaveChanges();
            }

            return context;
        }
    }
}