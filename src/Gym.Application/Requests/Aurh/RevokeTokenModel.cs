using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gym.Application.Requests.Aurh
{
    public sealed record RevokeTokenModel
    {
        public string? Token { get; set; }
    }
}
