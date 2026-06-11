using FitnessTracker.Core.DTOs;
using FitnessTracker.Core.Services;
using FitnessTracker.Infrastructure;
using FitnessTracker.Infrastructure.Entities;
using FitnessTracker.Infrastructure.Enums;

namespace FitnessTracker.Tests
{
    [TestFixture]
    public class ExerciseServiceTests
    {
        private ApplicationDbContext _context = null!;
        private ExerciseService _service = null!;

        [SetUp]
        public void SetUp()
        {
            _context = TestDbContextFactory.Create("ExerciseDb_" + Guid.NewGuid());
            _service = new ExerciseService(_context);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        // - GetAll

        [Test]
        public async Task GetTaskAsync_ReturnsAllExercises()
        {
            // TestDbContextFactory seeds 2 exercises (Id 1, 2) into every fresh context.
            // Adding 2 more should bring the total to 4.
            _context.Exercises.AddRange(
                new Exercise { Id = 100, Name = "Squat", Description = "Leg exercise for quads", MuscleGroup = MuscleGroup.Legs },
                new Exercise { Id = 101, Name = "Bench Press", Description = "Chest pressing movement", MuscleGroup = MuscleGroup.Chest }
            );
            await _context.SaveChangesAsync();

            var result = (await _service.GetAllAsync()).ToList();

            Assert.That(result, Has.Count.EqualTo(4));
            Assert.That(result.Any(e => e.Id == 100), Is.True);
            Assert.That(result.Any(e => e.Id == 101), Is.True);
        }

        [Test]
        public async Task GetTaskAsync_ReturnsOnlySeededExercises_WhenNoneAdded()
        {
            // The factory always seeds 2 exercises (Id 1 = Bench Press, Id 2 = Squat),
            // so a "fresh" context is never truly empty.
            var result = (await _service.GetAllAsync()).ToList();

            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.Any(e => e.Id == 1), Is.True);
            Assert.That(result.Any(e => e.Id == 2), Is.True);
        }

        [Test]
        public async Task GetTaskAsync_MapsFieldsCorrectly()
        {
            _context.Exercises.Add(new Exercise
            {
                Id = 200,
                Name = "Pull-Up",
                Description = "Upper body pulling movement",
                MuscleGroup = MuscleGroup.Back
            });
            await _context.SaveChangesAsync();

            var result = (await _service.GetAllAsync()).First(e => e.Id == 200);

            Assert.That(result.Name, Is.EqualTo("Pull-Up"));
            Assert.That(result.Description, Is.EqualTo("Upper body pulling movement"));
            Assert.That(result.MuscleGroup, Is.EqualTo(MuscleGroup.Back));
        }

        // -- GetById

        [Test]
        public async Task GetByIdAsync_ReturnsExercise_WhenExists()
        {
            _context.Exercises.Add(new Exercise
            {
                Id = 300,
                Name = "Deadlift",
                Description = "Full body pulling movement",
                MuscleGroup = MuscleGroup.Back
            });
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(300);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Deadlift"));
        }

        [Test]
        public async Task GetByIdAsync_ReutrnsNull_WhenNotFound()
        {
            var result = await _service.GetByIdAsync(9999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsCorrectId()
        {
            _context.Exercises.AddRange(
                new Exercise { Id = 401, Name = "Lunge", Description = "Single leg exercise", MuscleGroup = MuscleGroup.Legs },
                new Exercise { Id = 402, Name = "Curl", Description = "Bicep isolation exercise", MuscleGroup = MuscleGroup.Arms }
            );
            await _context.SaveChangesAsync();

            var result = await _service.GetByIdAsync(402);

            Assert.That(result!.Id, Is.EqualTo(402));
            Assert.That(result.Name, Is.EqualTo("Curl"));
        }
    }
}