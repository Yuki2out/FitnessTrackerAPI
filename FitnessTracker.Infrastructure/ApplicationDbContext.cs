using System.Text.Json;
using System.Text.Json.Serialization; 
using FitnessTracker.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FitnessTracker.Infrastructure.Entities;
namespace FitnessTracker.Infrastructure
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<WorkoutSession> WorkoutSessions { get; set; }
        public DbSet<WorkoutSet> WorkoutSets { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<WorkoutRoutine> WorkoutRoutines { get; set; }
        public DbSet<ProgressLog> ProgressLogs { get; set; }
        public DbSet<UserFavoriteExercise> UserFavoriteExercises { get; set; }
        public DbSet<WorkoutTemplate> WorkoutTemplates { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed System Identity Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "admin-role-id-111", Name = "Administrator", NormalizedName = "ADMINISTRATOR" },
                new IdentityRole { Id = "user-role-id-222", Name = "User", NormalizedName = "USER" }
            );

            // A deleted Exercise shouldn't cascade-delete a user's logged sets
            builder.Entity<WorkoutSet>()
                .HasOne(s => s.Exercise)
                .WithMany()
                .HasForeignKey(s => s.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

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