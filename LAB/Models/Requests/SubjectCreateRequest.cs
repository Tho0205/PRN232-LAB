using System.ComponentModel.DataAnnotations;
using PRN232.LAB.API.Models.Validation;

namespace PRN232.LAB.API.Models.Requests
{
    public class SubjectCreateRequest
    {
        [Required]
        [StringLength(20)]
        [UniversityCode]
        public string SubjectCode { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string SubjectName { get; set; } = null!;

        [Range(1, 4)]
        public int Credit { get; set; }
    }
}
