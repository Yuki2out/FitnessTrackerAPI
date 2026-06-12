using System.Security.Claims;
using FitnessTracker.Core.DTOs;
using FitnessTracker.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessTracker.WebApi.Controllers
{
    [ApiController]
    [Route("api/workout-templates")]
    [Authorize]
    public class WorkoutTemplatesController : ControllerBase
    {
        private readonly IWorkoutTemplateService _templateService;

        public WorkoutTemplatesController(IWorkoutTemplateService templateService)
        {
            _templateService = templateService;
        }
    
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _templateService.DeleteTemplateAsync(userId, id);
            if (!success) return NotFound(new { message = "Template not found or unauthorized." });

            return Ok(new { message = "Template deleted successfully." });
        }

        [HttpPost("from-history")]
        public async Task<IActionResult> CreateFromHistory([FromBody] CreateTemplateFromHistoryDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var success = await _templateService.CreateFromHistoryAsync(userId, dto);
            
            if (!success) 
                return NotFound(new { message = "Workout session history entry not found or unauthorized." });

            return Ok(new { message = "Template created successfully from workout history!" });
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTemplates()
        {
            // 1. Grab the current user's unique identity token ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // 2. Query the service layer for this user's templates
            var templates = await _templateService.GetUserTemplatesAsync(userId);

            // 3. Return a clean 200 OK containing the array lists
            return Ok(templates);
        }
    }
}