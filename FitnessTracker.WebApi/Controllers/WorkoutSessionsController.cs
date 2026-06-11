using System.Security.Claims;
using FitnessTracker.Core.DTOs;
using FitnessTracker.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Maps to: api/WorkoutSessions
    [Authorize]
    public class WorkoutSessionsController : ControllerBase
    {
        private readonly IWorkoutSessionService _sessionService;

        public WorkoutSessionsController(IWorkoutSessionService sessionService)
        {
            _sessionService = sessionService;
        }

        // 1. MATCHES: Task<IEnumerable<WorkoutSessionDto>> GetHistoryAsync(string userId)
        // GET api/workoutsessions/my
        [HttpGet("my")]
        public async Task<IActionResult> GetAllMySessions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var sessions = await _sessionService.GetHistoryAsync(userId);
            return Ok(sessions);
        }

        // 2. MATCHES: Task<WorkoutSessionDto?> GetByIdAsync(int id, string userId, bool isAdmin)
        // GET api/workoutsessions/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = User.IsInRole("Administrator");

            var session = await _sessionService.GetByIdAsync(id, userId, isAdmin);
            if (session == null)
                return NotFound(new { message = $"Workout session with id {id} not found." });

            return Ok(session);
        }

        // 3. MATCHES: Task<WorkoutSessionDto> StartAsync(string userId, StartWorkoutModel model)
        // POST api/workoutsessions/start
        [HttpPost("start")]
        public async Task<IActionResult> StartSession([FromBody] StartWorkoutModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var activeSession = await _sessionService.StartAsync(userId, model);
            return Ok(activeSession);
        }

        // 4. MATCHES: Task<WorkoutSessionDto?> GetActiveAsync(string userId)
        // GET api/workoutsessions/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveSession()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var active = await _sessionService.GetActiveAsync(userId);
            if (active == null)
                return NoContent(); // 204 status: UI knows there isn't a live tracking session running

            return Ok(active);
        }

        // 5. MATCHES: Task<WorkoutSetDto?> AddSetAsync(int sessionId, string userId, AddSetInputModel model)
        // POST api/workoutsessions/5/sets
        [HttpPost("{sessionId:int}/sets")]
        public async Task<IActionResult> AddSet(int sessionId, [FromBody] AddSetInputModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var newSet = await _sessionService.AddSetAsync(sessionId, userId, model);
            
            if (newSet == null)
                return NotFound(new { message = "Could not log set. Session not found or access denied." });

            return Ok(newSet);
        }

        // 6. MATCHES: Task<bool> RemoveSetAsync(int sessionId, int setId, string userId)
        // DELETE api/workoutsessions/5/sets/12
        [HttpDelete("{sessionId:int}/sets/{setId:int}")]
        public async Task<IActionResult> RemoveSet(int sessionId, int setId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var deleted = await _sessionService.RemoveSetAsync(sessionId, setId, userId);
            
            if (!deleted)
                return BadRequest(new { message = "Could not remove set. Session/Set not found or unauthorized." });

            return NoContent();
        }

        // 7. MATCHES: Task<WorkoutSessionDto?> FinishAsync(int sessionId, string userId, bool save)
        // POST api/workoutsessions/5/finish?save=true
        [HttpPost("{sessionId:int}/finish")]
        public async Task<IActionResult> FinishWorkout(int sessionId, [FromQuery] bool save)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var finishedSession = await _sessionService.FinishAsync(sessionId, userId, save);

            if (finishedSession == null && save)
                return NotFound(new { message = "Could not find or save session." });

            return Ok(finishedSession);
        }
    }
}