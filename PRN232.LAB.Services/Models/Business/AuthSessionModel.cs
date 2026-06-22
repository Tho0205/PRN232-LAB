namespace PRN232.LAB.Services.Models.Business
{
    public class AuthSessionModel
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public int ExpiresIn { get; set; }
    }
}
