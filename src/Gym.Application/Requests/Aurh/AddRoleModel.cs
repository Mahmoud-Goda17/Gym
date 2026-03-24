
namespace Gym.Application.Requests.Aurh
{
    public sealed record  AddRoleModel
    {
        public string UserId { get; set; }
        public string Role { get; set; }

    }
}
