
using Gym.Application.Dtos.Auth;
using Gym.Application.Requests.Aurh;

namespace Gym.Application.Interfaces
{
    public interface IAuthService
    {
        Task<UserResultDto> RegisterAsync(Register request);
        Task<UserResultDto> LogInAsync(LogIn request);
        Task<string> AddRoleAsync(AddRoleModel model);
        Task<UserResultDto> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);

    }
}
