using System.Security.Claims;
using FitnessTracker.Core.DTOs;
using FitnessTracker.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkoutRoutinesController : ControllerBase
    {
        private readonly IWorkoutRoutineService _routineService;

        public WorkoutRoutinesController(IWorkoutRoutineService routineService)
        {
            _routineService = routineService;
        }

        // GET api/workoutroutines  — Admin sees all; users see their own
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var isAdmin = User.IsInRole("Administrator");
            if (isAdmin)
            {
                var all = await _routineService.GetAllAsync();
                return Ok(all);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var mine = await _routineService.GetByUserAsync(userId);
            return Ok(mine);
        }

        // GET api/workoutroutines/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var routine = await _routineService.GetByIdAsync(id);
            if (routine == null)
                return NotFound(new { message = $"Routine with id {id} not found." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = User.IsInRole("Administrator");

            if (routine.CreatorId != userId && !isAdmin)
                return Forbid();

            return Ok(routine);
        }

        // POST api/workoutroutines
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoutineInputModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var created = await _routineService.CreateAsync(userId, model);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/workoutroutines/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] RoutineInputModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var updated = await _routineService.UpdateAsync(id, userId, model);
            if (updated == null)
                return NotFound(new { message = $"Routine with id {id} not found or you don't own it." });

            return Ok(updated);
        }

        // DELETE api/workoutroutines/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = User.IsInRole("Administrator");

            var deleted = await _routineService.DeleteAsync(id, userId, isAdmin);
            if (!deleted)
                return NotFound(new { message = $"Routine with id {id} not found or access denied." });

            return NoContent();
        }
    }
}