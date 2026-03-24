
namespace Gym.Application.Requests.Aurh
{
    public sealed record Register
    {
        public string DisplayName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? PhoneNumber { get; set; }


    }
}
