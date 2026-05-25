namespace PRN232.LAB1.API.Models.Requests
{
    public class EnrollmentCreateRequest
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollDate { get; set; }
        public string Status { get; set; } = null!;
    }
}
