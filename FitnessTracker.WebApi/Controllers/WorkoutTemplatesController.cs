using System.Security.Claims;
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

        [HttpGet]
        public async Task<IActionResult> GetMyTemplates()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var templates = await _templateService.GetUserTemplatesAsync(userId);
            return Ok(templates);
        }
    }
}