using PRN232.LAB.Services.Models.Business;

namespace PRN232.LAB.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthSessionModel?> LoginAsync(string username, string password);
        Task<AuthSessionModel?> RefreshTokenAsync(string refreshToken);
    }
}
