using System.ComponentModel.DataAnnotations;

namespace PRN232.LAB.API.Models.Requests
{
    public class CourseCreateRequest
    {
        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = null!;

        [Range(1, int.MaxValue)]
        public int SemesterId { get; set; }
    }
}
