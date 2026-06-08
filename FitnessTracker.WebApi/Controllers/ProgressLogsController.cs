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
    public class ProgressLogsController : ControllerBase
    {
        private readonly IProgressLogService _logService;

        public ProgressLogsController(IProgressLogService logService)
        {
            _logService = logService;
        }

        // GET api/progresslogs  — Admin sees all; users see their own
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var isAdmin = User.IsInRole("Administrator");
            if (isAdmin)
            {
                var all = await _logService.GetAllAsync();
                return Ok(all);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var mine = await _logService.GetByUserAsync(userId);
            return Ok(mine);
        }

        // GET api/progresslogs/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var log = await _logService.GetByIdAsync(id);
            if (log == null)
                return NotFound(new { message = $"Log with id {id} not found." });

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = User.IsInRole("Administrator");

            if (log.UserId != userId && !isAdmin)
                return Forbid();

            return Ok(log);
        }

        // POST api/progresslogs
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProgressLogInputModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var created = await _logService.CreateAsync(userId, model);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // DELETE api/progresslogs/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var isAdmin = User.IsInRole("Administrator");

            var deleted = await _logService.DeleteAsync(id, userId, isAdmin);
            if (!deleted)
                return NotFound(new { message = $"Log with id {id} not found or access denied." });

            return NoContent();
        }

        // GET api/progresslogs/my  — Convenience endpoint for current user
        [HttpGet("my")]
        public async Task<IActionResult> GetMine()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var logs = await _logService.GetByUserAsync(userId);
            return Ok(logs);
        }
    }
}