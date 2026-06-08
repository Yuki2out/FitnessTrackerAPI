using FitnessTracker.Core.DTOs;
using FitnessTracker.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExercisesController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;

        public ExercisesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        // GET api/exercises
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exercises = await _exerciseService.GetAllAsync();
            return Ok(exercises);
        }

        // GET api/exercises/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var exercise = await _exerciseService.GetByIdAsync(id);
            if (exercise == null)
                return NotFound(new { message = $"Exercise with id {id} not found." });

            return Ok(exercise);
        }

        // POST api/exercises  — Admin only
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Create([FromBody] ExerciseInputModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _exerciseService.CreateAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT api/exercises/5  — Admin only
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Update(int id, [FromBody] ExerciseInputModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _exerciseService.UpdateAsync(id, model);
            if (updated == null)
                return NotFound(new { message = $"Exercise with id {id} not found." });

            return Ok(updated);
        }

        // DELETE api/exercises/5  — Admin only
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _exerciseService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Exercise with id {id} not found." });

            return NoContent();
        }
    }
}