namespace PRN232.LAB1.API.Models.Requests
{
    public class SemesterCreateRequest
    {
        public string SemesterName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
