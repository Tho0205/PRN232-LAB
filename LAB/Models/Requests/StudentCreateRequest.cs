using System.ComponentModel.DataAnnotations;
using PRN232.LAB.API.Models.Validation;

namespace PRN232.LAB.API.Models.Requests
{
    public class StudentCreateRequest
    {
        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [AllowedUniversityEmail]
        public string Email { get; set; } = null!;

        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}
