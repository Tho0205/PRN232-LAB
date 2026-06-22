using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB.API.Models.Requests
{
    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; } = null!;
    }
}
