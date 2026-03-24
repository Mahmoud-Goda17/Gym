
using Microsoft.EntityFrameworkCore;

namespace Gym.Application.Identity
{
    [Owned]
    public class RefreshToken
    {
        public string Token { get; set; }
        public DateTime ExpireOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpireOn;
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokeOn { get; set; }
        public bool IsActive => RevokeOn == null && !IsExpired;

    }
}
