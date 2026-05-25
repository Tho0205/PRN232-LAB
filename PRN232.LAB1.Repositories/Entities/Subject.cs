using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232.LAB1.Repositories.Entities
{
    [Table("Subject")]
    public class Subject
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubjectId { get; set; }

        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string SubjectCode { get; set; } = null!;

        [MaxLength(100)]
        public string SubjectName { get; set; } = null!;

        public int Credit { get; set; }
    }
}
