using System.Security.Claims;
using FitnessTracker.Core.DTOs;
using FitnessTracker.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Fallback routing to api/workoutsessions
    [Authorize]
    public class WorkoutSessionsController : ControllerBase
    {
        private readonly IWorkoutSessionService _workoutSessionService;

        public WorkoutSessionsController(IWorkoutSessionService workoutSessionService)
        {
            _workoutSessionService = workoutSessionService;
        }

        // POST api/workoutsessions/start
        [HttpPost("start")]
        public async Task<IActionResult> Start([FromBody] StartWorkoutModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var session = await _workoutSessionService.StartAsync(userId, model);
            return Ok(session);
        }

        // GET api/workoutsessions/setup-active
        [HttpGet("setup-active")]
        public async Task<IActionResult> SetupActiveFromLibrary([FromQuery] int? templateId, [FromQuery] int? cloneFromId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // This anonymous structure matches exactly what your frontend JavaScript expects to unpack!
            var clientPayload = new
            {
                workoutName = "Custom Routine Log",
                exercises = new List<object>()
            };

            // Case A: User clicked a past history log to clone target items
            if (cloneFromId.HasValue)
            {
                var oldSession = await _workoutSessionService.GetByIdAsync(cloneFromId.Value, userId, false);
                if (oldSession != null)
                {
                    clientPayload = new
                    {
                        workoutName = $"{oldSession.Name ?? "Workout"} Layout",
                        exercises = oldSession.Sets
                            .GroupBy(s => s.ExerciseId)
                            .Select(g => new
                            {
                                exerciseId = g.Key,
                                name = g.First().ExerciseName,
                                sets = g.Select(s => new { targetWeight = s.WeightUsed, targetReps = s.RepsCompleted }).ToList()
                            })
                            .Cast<object>()
                            .ToList()
                    };
                }
            }
            // Case B: User clicked a template block configuration row shortcut
            else if (templateId.HasValue)
            {
                // For now, give a clean title placeholder until you expand template relations later
                clientPayload = new
                {
                    workoutName = "Template Routine",
                    exercises = new List<object>() 
                };
            }

            return Ok(clientPayload);
        }

        // GET api/workoutsessions/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var isAdmin = User.IsInRole("Administrator");
            var session = await _workoutSessionService.GetByIdAsync(id, userId, isAdmin);
            
            if (session == null) return NotFound(new { message = "Session not found." });
            return Ok(session);
        }

        // GET api/workoutsessions/my (Used by library.html history section!)
        [HttpGet("my")]
        public async Task<IActionResult> GetAllMySessions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var sessions = await _workoutSessionService.GetAllMySessionsAsync(userId);
            return Ok(sessions);
        }

        // POST api/workoutsessions/5/sets
        [HttpPost("{id:int}/sets")]
        public async Task<IActionResult> AddSet(int id, [FromBody] AddSetInputModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var createdSet = await _workoutSessionService.AddSetAsync(id, userId, model);
            if (createdSet == null) return BadRequest(new { message = "Could not add set to this session." });

            return Ok(createdSet);
        }

        // DELETE api/workoutsessions/5/sets/12
        [HttpDelete("{id:int}/sets/{setId:int}")]
        public async Task<IActionResult> RemoveSet(int id, int setId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _workoutSessionService.RemoveSetAsync(id, setId, userId);
            if (!success) return BadRequest(new { message = "Could not remove set." });

            return NoContent();
        }

        // POST api/workoutsessions/5/finish
        [HttpPost("{id:int}/finish")]
        public async Task<IActionResult> Finish(int id, [FromQuery] bool save = true)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var finishedSession = await _workoutSessionService.FinishAsync(id, userId, save);
            if (finishedSession == null && save) return BadRequest(new { message = "Could not complete session." });

            return Ok(finishedSession);
        }
    }
}