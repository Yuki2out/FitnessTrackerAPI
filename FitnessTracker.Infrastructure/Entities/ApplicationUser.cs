using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace FitnessTracker.Infrastructure.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        // EF Core will track this cleanly without inline default evaluation mismatches
        public DateTime RegistrationDate { get; set; }

        public ICollection<WorkoutRoutine> Routines { get; set; } = new List<WorkoutRoutine>();
    }
}