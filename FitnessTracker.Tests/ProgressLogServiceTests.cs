using FitnessTracker.Infrastructure;
using FitnessTracker.Core.DTOs;
using FitnessTracker.Core.Services;
using FitnessTracker.Infrastructure.Entities;
using FitnessTracker.Infrastructure.Enums;
using FitnessTracker.Infrastructure.Entities;
namespace FitnessTracker.Tests
{
    [TestFixture]
    public class ProgressLogServiceTests
    {
        private ApplicationDbContext _context = null!;
        private ProgressLogService _service = null!;
        private const string User1Id = "user-1";
        private const string User2Id = "user-2";

        [SetUp]
        public void SetUp()
        {
            _context = TestDbContextFactory.Create("ProgressLogDb_" + Guid.NewGuid());
            _service = new ProgressLogService(_context);
        }

        [TearDown]
        public void TearDown() => _context.Dispose();

        [Test]
        public async Task GetAllAsync_ReturnsAllLogs()
        {
            _context.ProgressLogs.AddRange(
                new ProgressLog { UserId = User1Id, ExerciseId = 1, WeightUsed = 80, RepsCompleted = 10, LogDate = DateTime.UtcNow },
                new ProgressLog { UserId = User2Id, ExerciseId = 1, WeightUsed = 60, RepsCompleted = 12, LogDate = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();
            var result = (await _service.GetAllAsync()).ToList();
            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task GetAllAsync_ReturnsEmpty_WhenNoLogs()
        {
            var result = await _service.GetAllAsync();
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetByUserAsync_ReturnsOnlyUserLogs()
        {
            _context.ProgressLogs.AddRange(
                new ProgressLog { UserId = User1Id, ExerciseId = 1, WeightUsed = 80, RepsCompleted = 10, LogDate = DateTime.UtcNow },
                new ProgressLog { UserId = User1Id, ExerciseId = 1, WeightUsed = 85, RepsCompleted = 8, LogDate = DateTime.UtcNow },
                new ProgressLog { UserId = User2Id, ExerciseId = 1, WeightUsed = 60, RepsCompleted = 12, LogDate = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();
            var result = (await _service.GetByUserAsync(User1Id)).ToList();
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result.All(l => l.UserId == User1Id), Is.True);
        }

        [Test]
        public async Task GetByUserAsync_ReturnsEmpty_WhenUserHasNoLogs()
        {
            var result = await _service.GetByUserAsync("nonexistent-user");
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetByUserAsync_ReturnsLogsOrderedByDateDescending()
        {
            _context.ProgressLogs.AddRange(
                new ProgressLog { UserId = User1Id, ExerciseId = 1, WeightUsed = 70, RepsCompleted = 10, LogDate = DateTime.UtcNow.AddDays(-5) },
                new ProgressLog { UserId = User1Id, ExerciseId = 1, WeightUsed = 80, RepsCompleted = 10, LogDate = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();
            var result = (await _service.GetByUserAsync(User1Id)).ToList();
            Assert.That(result[0].LogDate, Is.GreaterThan(result[1].LogDate));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsLog_WhenExists()
        {
            _context.ProgressLogs.Add(new ProgressLog
            {
                Id = 50,
                UserId = User1Id,
                ExerciseId = 1,
                WeightUsed = 100,
                RepsCompleted = 5,
                LogDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            var result = await _service.GetByIdAsync(50);
            Assert.That(result, Is.Not.Null);
            Assert.That(result!.WeightUsed, Is.EqualTo(100));
        }

        [Test]
        public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
        {
            var result = await _service.GetByIdAsync(9999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateAsync_AddsLogToDatabase()
        {
            var result = await _service.CreateAsync(User1Id, new ProgressLogInputModel
            {
                ExerciseId = 1,
                WeightUsed = 75,
                RepsCompleted = 10
            });
            Assert.That(result, Is.Not.Null);
            Assert.That(_context.ProgressLogs.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task CreateAsync_SetsCorrectUserId()
        {
            var result = await _service.CreateAsync(User1Id, new ProgressLogInputModel
            {
                ExerciseId = 1,
                WeightUsed = 60,
                RepsCompleted = 12
            });
            Assert.That(result.UserId, Is.EqualTo(User1Id));
        }

        [Test]
        public async Task CreateAsync_MapsWeightAndRepsCorrectly()
        {
            var result = await _service.CreateAsync(User1Id, new ProgressLogInputModel
            {
                ExerciseId = 1,
                WeightUsed = 120.5,
                RepsCompleted = 3
            });
            Assert.That(result.WeightUsed, Is.EqualTo(120.5));
            Assert.That(result.RepsCompleted, Is.EqualTo(3));
        }

        [Test]
        public async Task DeleteAsync_RemovesLog_WhenOwnerDeletes()
        {
            _context.ProgressLogs.Add(new ProgressLog
            {
                Id = 200,
                UserId = User1Id,
                ExerciseId = 1,
                WeightUsed = 80,
                RepsCompleted = 8,
                LogDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            var result = await _service.DeleteAsync(200, User1Id, isAdmin: false);
            Assert.That(result, Is.True);
            Assert.That(_context.ProgressLogs.Find(200), Is.Null);
        }

        [Test]
        public async Task DeleteAsync_RemovesLog_WhenAdminDeletes()
        {
            _context.ProgressLogs.Add(new ProgressLog
            {
                Id = 201,
                UserId = User1Id,
                ExerciseId = 1,
                WeightUsed = 80,
                RepsCompleted = 8,
                LogDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            var result = await _service.DeleteAsync(201, User2Id, isAdmin: true);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteAsync_ReturnsFalse_WhenOtherUserTriesToDelete()
        {
            _context.ProgressLogs.Add(new ProgressLog
            {
                Id = 202,
                UserId = User1Id,
                ExerciseId = 1,
                WeightUsed = 80,
                RepsCompleted = 8,
                LogDate = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();
            var result = await _service.DeleteAsync(202, User2Id, isAdmin: false);
            Assert.That(result, Is.False);
            Assert.That(_context.ProgressLogs.Find(202), Is.Not.Null);
        }

        [Test]
        public async Task DeleteAsync_ReturnsFalse_WhenLogNotFound()
        {
            var result = await _service.DeleteAsync(9999, User1Id, isAdmin: false);
            Assert.That(result, Is.False);
        }
    }
}