using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PRN232.LAB.Repositories.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [MaxLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Username { get; set; } = null!;

        [MaxLength(255)]
        public string PasswordHash { get; set; } = null!;

        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string Role { get; set; } = null!;

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
