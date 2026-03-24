
using Microsoft.AspNetCore.Identity;

namespace Gym.Application.Identity
{
    public class AppUser : IdentityUser
    {
        public string DisplayName { get; set; } = string.Empty;
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
