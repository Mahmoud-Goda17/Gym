
using System.Text.Json.Serialization;

namespace Gym.Application.Dtos.Auth
{
    public class UserResultDto
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public List<string> Roles { get; set; }
        public string Token { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
