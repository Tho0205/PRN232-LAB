namespace PRN232.LAB.API.Models.Responses
{
    public class CourseResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }

        public SemesterResponse? Semester { get; set; }
        public ICollection<EnrollmentResponse>? Enrollments { get; set; }
    }
}
