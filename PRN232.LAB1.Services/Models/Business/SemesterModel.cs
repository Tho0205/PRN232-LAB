namespace PRN232.LAB1.Services.Models.Business
{
    public class SemesterModel
    {
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public ICollection<CourseModel>? Courses { get; set; }
    }
}
