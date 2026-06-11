using FitnessTracker.Infrastructure;
using FitnessTracker.Core.DTOs;
using FitnessTracker.Core.Services;
using FitnessTracker.Infrastructure.Entities;
using FitnessTracker.Infrastructure.Enums;

namespace FitnessTracker.Tests
{
    [TestFixture]
    public class WorkoutRoutineServiceTests
    {
        private ApplicationDbContext _context = null!;
        private WorkoutRoutineService _service = null!;
        private const string User1Id = "user-1";
        private const string User2Id = "user-2";

        [SetUp]
        public void SetUp()
        {
            _context = TestDbContextFactory.Create("RoutineDb_" + Guid.NewGuid());
            _service = new WorkoutRoutineService(_context);
        }

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public async Task GetAllAsync_ReturnsAllRoutines()
        {
            _context.WorkoutRoutines.AddRange(
                new WorkoutRoutine { Name = "Push Day", Description = "Chest and shoulder focus", CreatorId = User1Id },
                new WorkoutRoutine { Name = "Pull Day", Description = "Back and bicep focus", CreatorId = User2Id }
            );
            await _context.SaveChangesAsync();
            var result = (await _service.GetAllAsync()).ToList();
            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetAllAsync_ReturnsEmpty_WhenNoRoutines()
        {
            var result = await _service.GetAllAsync();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetByIdAsync_ReturnsRoutine_WhenExists()
        {
            _context.WorkoutRoutines.Add(new WorkoutRoutine
            {
                Id = 10,
                Name = "Leg Day",
                Description = "Full lower body workout",
                CreatorId = User1Id
            });
            await _context.SaveChangesAsync();
            var result = await _service.GetByIdAsync(10);
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Leg Day"));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            var result = await _service.GetByIdAsync(9999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetByUserAsync_ReturnsOnlyUsersRoutines()
        {
            _context.WorkoutRoutines.AddRange(
                new WorkoutRoutine { Name = "My Routine A", Description = "First routine for user one", CreatorId = User1Id },
                new WorkoutRoutine { Name = "My Routine B", Description = "Second routine for user one", CreatorId = User1Id },
                new WorkoutRoutine { Name = "Other Routine", Description = "Routine belonging to user two", CreatorId = User2Id }
            );
            await _context.SaveChangesAsync();
            var result = (await _service.GetByUserAsync(User1Id)).ToList();
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.All(r => r.CreatorId == User1Id), Is.True);
        }

        [Test]
        public async Task GetByUserAsync_ReturnsEmpty_ForUserWithNoRoutines()
        {
            var result = await _service.GetByUserAsync("no-routines-user");
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task CreateAsync_AddsRoutineToDatabase()
        {
            var result = await _service.CreateAsync(User1Id, new RoutineInputModel
            {
                Name = "Full Body",
                Description = "Complete full body workout session",
                ExerciseIds = new List<int> { 1, 2 }
            });
            Assert.That(result, Is.Not.Null);
            Assert.That(_context.WorkoutRoutines.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task CreateAsync_SetsCreatorId()
        {
            var result = await _service.CreateAsync(User1Id, new RoutineInputModel
            {
                Name = "My Split",
                Description = "Personal workout split routine",
                ExerciseIds = new List<int>()
            });
            Assert.That(result.CreatorId, Is.EqualTo(User1Id));
        }

        [Test]
        public async Task CreateAsync_AttachesExercises()
        {
            var result = await _service.CreateAsync(User1Id, new RoutineInputModel
            {
                Name = "Upper Body",
                Description = "Upper body strength training day",
                ExerciseIds = new List<int> { 1, 2 }
            });
            Assert.That(result.Exercises, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task CreateAsync_WorksWithNoExercises()
        {
            var result = await _service.CreateAsync(User1Id, new RoutineInputModel
            {
                Name = "Empty Routine",
                Description = "A routine with no exercises yet",
                ExerciseIds = new List<int>()
            });
            Assert.That(result.Exercises, Is.Empty);
        }

        [Test]
        public async Task UpdateAsync_UpdatesRoutine_WhenOwnerUpdates()
        {
            _context.WorkoutRoutines.Add(new WorkoutRoutine
            {
                Id = 20,
                Name = "Old Routine",
                Description = "Old description for this routine",
                CreatorId = User1Id
            });
            await _context.SaveChangesAsync();
            var result = await _service.UpdateAsync(20, User1Id, new RoutineInputModel
            {
                Name = "New Routine",
                Description = "Updated description for the routine",
                ExerciseIds = new List<int> { 1 }
            });
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("New Routine"));
        }

        [Test]
        public async Task UpdateAsync_ReturnsNull_WhenOtherUserUpdates()
        {
            _context.WorkoutRoutines.Add(new WorkoutRoutine
            {
                Id = 21,
                Name = "Someones Routine",
                Description = "Routine created by user one",
                CreatorId = User1Id
            });
            await _context.SaveChangesAsync();
            var result = await _service.UpdateAsync(21, User2Id, new RoutineInputModel
            {
                Name = "Hacked",
                Description = "Attempted update by wrong user",
                ExerciseIds = new List<int>()
            });
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateAsync_ReturnsNull_WhenNotFound()
        {
            var result = await _service.UpdateAsync(9999, User1Id, new RoutineInputModel
            {
                Name = "Ghost",
                Description = "Updating a non-existent routine",
                ExerciseIds = new List<int>()
            });
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_RemovesRoutine_WhenOwnerDeletes()
        {
            _context.WorkoutRoutines.Add(new WorkoutRoutine
            {
                Id = 30,
                Name = "To Delete",
                Description = "This routine will be deleted now",
                CreatorId = User1Id
            });
            await _context.SaveChangesAsync();
            var result = await _service.DeleteAsync(30, User1Id, isAdmin: false);
            Assert.That(result, Is.True);
            Assert.That(_context.WorkoutRoutines.Find(30), Is.Null);
        }

        [Test]
        public async Task DeleteAsync_RemovesRoutine_WhenAdminDeletes()
        {
            _context.WorkoutRoutines.Add(new WorkoutRoutine
            {
                Id = 31,
                Name = "Admin Delete",
                Description = "Admin will delete this routine",
                CreatorId = User1Id
            });
            await _context.SaveChangesAsync();
            var result = await _service.DeleteAsync(31, User2Id, isAdmin: true);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteAsync_ReturnsFalse_WhenOtherUserDeletes()
        {
            _context.WorkoutRoutines.Add(new WorkoutRoutine
            {
                Id = 32,
                Name = "Protected",
                Description = "User two cannot delete this routine",
                CreatorId = User1Id
            });
            await _context.SaveChangesAsync();
            var result = await _service.DeleteAsync(32, User2Id, isAdmin: false);
            Assert.That(result, Is.False);
            Assert.That(_context.WorkoutRoutines.Find(32), Is.Not.Null);
        }

        [Test]
        public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
        {
            var result = await _service.DeleteAsync(9999, User1Id, isAdmin: false);
            Assert.That(result, Is.False);
        }
    }
}