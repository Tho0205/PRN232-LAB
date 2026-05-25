namespace PRN232.LAB1.API.Models.Requests
{
    public class CourseCreateRequest
    {
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
    }
}
