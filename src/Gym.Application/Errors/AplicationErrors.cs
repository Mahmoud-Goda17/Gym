
using Gym.Domain.Results;

namespace Gym.Application.Errors
{
    public static class AplicationErrors
    {
        public static Error NotFound(string entityName, Guid id) =>
            Error.NotFound($"{entityName}.NotFound", $"{entityName} with ID '{id}' was not found.");
    }
}
