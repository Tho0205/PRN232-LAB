namespace PRN232.LAB1.API.Models.Requests
{
    public class StudentCreateRequest
    {
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
    }
}
