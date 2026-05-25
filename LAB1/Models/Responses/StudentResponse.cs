namespace PRN232.LAB1.API.Models.Responses
{
    public class StudentResponse
    {
        public int StudentId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        
        public ICollection<EnrollmentResponse>? Enrollments { get; set; }
    }
}
