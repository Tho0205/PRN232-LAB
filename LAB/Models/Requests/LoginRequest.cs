using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB.API.Models.Requests
{
    public class LoginRequest
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Password { get; set; } = null!;
    }
}
