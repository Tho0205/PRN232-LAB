using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB.API.Models.Requests
{
    public class SemesterCreateRequest
    {
        [Required]
        [StringLength(100)]
        public string SemesterName { get; set; } = null!;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }
}
