using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232.LAB1.Repositories.Entities
{
    [Table("Semester")]
    public class Semester
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SemesterId { get; set; }

        [MaxLength(100)]
        public string SemesterName { get; set; } = null!;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
