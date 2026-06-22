namespace PRN232.LAB.Services.Models.Business
{
    public class StudentModel
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        
        public ICollection<EnrollmentModel>? Enrollments { get; set; }
    }
}
