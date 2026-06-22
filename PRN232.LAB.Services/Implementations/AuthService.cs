using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PRN232.LAB.Repositories.Entities;
using PRN232.LAB.Repositories.Interfaces;
using PRN232.LAB.Services.Interfaces;
using PRN232.LAB.Services.Models.Business;
using PRN232.LAB.Services.Models.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PRN232.LAB.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly JwtSettings _jwtSettings;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public AuthService(IUnitOfWork unitOfWork, IOptions<JwtSettings> jwtOptions)
        {
            _unitOfWork = unitOfWork;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<AuthSessionModel?> LoginAsync(string username, string password)
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync(u => u.Username == username);
            var user = users.FirstOrDefault();

            if (user == null)
            {
                return null;
            }

            var verification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            if (verification == PasswordVerificationResult.Failed)
            {
                return null;
            }

            return await IssueTokensAsync(user);
        }

        public async Task<AuthSessionModel?> RefreshTokenAsync(string refreshToken)
        {
            var tokens = await _unitOfWork.RefreshTokenRepository.GetAllAsync(
                t => t.Token == refreshToken && !t.IsRevoked,
                includeProperties: "User");

            var tokenEntity = tokens.FirstOrDefault();

            if (tokenEntity == null || tokenEntity.ExpiresAt <= DateTime.UtcNow)
            {
                return null;
            }

            tokenEntity.IsRevoked = true;
            tokenEntity.RevokedAt = DateTime.UtcNow;
            _unitOfWork.RefreshTokenRepository.Update(tokenEntity);
            await _unitOfWork.SaveAsync();

            return await IssueTokensAsync(tokenEntity.User);
        }

        private async Task<AuthSessionModel> IssueTokensAsync(User user)
        {
            var accessToken = CreateAccessToken(user);
            var newRefreshToken = CreateRefreshTokenValue();
            var refreshTokenEntity = new RefreshToken
            {
                UserId = user.UserId,
                Token = newRefreshToken,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
                IsRevoked = false
            };

            await _unitOfWork.RefreshTokenRepository.InsertAsync(refreshTokenEntity);
            await _unitOfWork.SaveAsync();

            return new AuthSessionModel
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = _jwtSettings.AccessTokenMinutes * 60
            };
        }

        private string CreateAccessToken(User user)
        {
            if (string.IsNullOrWhiteSpace(_jwtSettings.Secret))
            {
                throw new InvalidOperationException("JWT secret is not configured.");
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string CreateRefreshTokenValue()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                .Replace("=", string.Empty)
                .Replace("+", string.Empty)
                .Replace("/", string.Empty);
        }
    }
}
