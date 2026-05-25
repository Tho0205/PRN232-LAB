namespace PRN232.LAB1.Services.Models.Business
{
    public class CourseModel
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }

        public SemesterModel? Semester { get; set; }
        public ICollection<EnrollmentModel>? Enrollments { get; set; }
    }
}
