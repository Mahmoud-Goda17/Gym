using Gym.Application.Interfaces;
using Gym.Application.Requests.Aurh;
using Microsoft.AspNetCore.Mvc;

namespace GymSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService service) : ControllerBase
    {
        [HttpPost("login")]
        public async Task<IActionResult> LogIn(LogIn request)
        {
            var result = await service.LogInAsync(request);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register( Register request)
        {
            var result = await service.RegisterAsync(request);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);

            return Ok(result);
        }
        [HttpPost("addrole")]
        public async Task<IActionResult> AddRole( AddRoleModel model)
        {
            var result = await service.AddRoleAsync(model);
            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);
            return Ok();
        }
        [HttpPost("refreshtoken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Refresh Token is missing");

            var result = await service.RefreshTokenAsync(refreshToken);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);
            if (!string.IsNullOrEmpty(result.RefreshToken))
                SetRefreshTokenInCookie(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(result);
        }
        [HttpPost("revoketoken")]
        public async Task<IActionResult> RevokeToken( RevokeTokenModel model)
        {
            var token = model.Token ?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required");
            var result = await service.RevokeTokenAsync(token);
            if (!result)
                return NotFound("Token not found");
            return Ok("Token revoked");
        }

        private void SetRefreshTokenInCookie(string refreshToken, DateTime expireOn)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = expireOn.ToLocalTime()
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }

    }

}
