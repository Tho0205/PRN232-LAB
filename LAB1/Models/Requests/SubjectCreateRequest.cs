namespace PRN232.LAB1.API.Models.Requests
{
    public class SubjectCreateRequest
    {
        public string SubjectCode { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public int Credit { get; set; }
    }
}
