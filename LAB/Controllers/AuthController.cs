using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LAB.API.Models.Requests;
using PRN232.LAB.API.Models.Responses;
using PRN232.LAB.Services.Interfaces;

namespace PRN232.LAB.API.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Route("api/v{version:apiVersion}/auth")]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login", Name = "Login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request,
            [FromHeader(Name = "X-Request-Id")] string? requestId)
        {
            var session = await _authService.LoginAsync(request.Username, request.Password);
            if (session == null)
            {
                return Error("Invalid username or password", null, 401);
            }

            if (!string.IsNullOrWhiteSpace(requestId))
            {
                Response.Headers["X-Request-Id"] = requestId;
            }

            return Success(new AuthResponse
            {
                AccessToken = session.AccessToken,
                RefreshToken = session.RefreshToken,
                ExpiresIn = session.ExpiresIn
            }, "Login successful");
        }

        [HttpPost("refresh-token", Name = "RefreshToken")]
        public async Task<IActionResult> RefreshToken(
            [FromBody] RefreshTokenRequest request,
            [FromHeader(Name = "X-Request-Id")] string? requestId)
        {
            var session = await _authService.RefreshTokenAsync(request.RefreshToken);
            if (session == null)
            {
                return Error("Invalid or expired refresh token", null, 401);
            }

            if (!string.IsNullOrWhiteSpace(requestId))
            {
                Response.Headers["X-Request-Id"] = requestId;
            }

            return Success(new AuthResponse
            {
                AccessToken = session.AccessToken,
                RefreshToken = session.RefreshToken,
                ExpiresIn = session.ExpiresIn
            }, "Token refreshed successfully");
        }
    }
}
