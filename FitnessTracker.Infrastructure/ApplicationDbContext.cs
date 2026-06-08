using System.Text.Json;
using System.Text.Json.Serialization; // 1. ADD THIS IMPORT
using FitnessTracker.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FitnessTracker.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutRoutine> WorkoutRoutines { get; set; }
        public DbSet<ProgressLog> ProgressLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed System Identity Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "admin-role-id-111", Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
                new IdentityRole { Id = "user-role-id-222", Name = "User", NormalizedName = "USER" }
            );

            // Tell EF Core to store the enum values as readable strings in the DB column
            builder.Entity<Exercise>()
                .Property(e => e.MuscleGroup)
                .HasConversion<string>();

            // FIX: Set static SQL database defaults for date values
            builder.Entity<ApplicationUser>()
                .Property(u => u.RegistrationDate)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Entity<ProgressLog>()
                .Property(p => p.LogDate)
                .HasDefaultValueSql("GETUTCDATE()");

            // Read and Seed Exercises from JSON file safely
            string jsonPath = Path.Combine(AppContext.BaseDirectory, "Data", "exercises.json");
            if (!File.Exists(jsonPath))
            {
                jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "FitnessTracker.Infrastructure", "Data", "exercises.json");
            }

            if (File.Exists(jsonPath))
            {
                string jsonString = File.ReadAllText(jsonPath);
                
                var serializerOptions = new JsonSerializerOptions
                {
                    Converters = { new JsonStringEnumConverter() }
                };

                var exercises = JsonSerializer.Deserialize<List<Exercise>>(jsonString, serializerOptions);
                if (exercises != null)
                {
                    builder.Entity<Exercise>().HasData(exercises);
                }
            }
        }
    }
}