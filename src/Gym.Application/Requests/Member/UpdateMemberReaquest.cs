namespace Gym.Application.Requests.Member
{
    public sealed record UpdateMemberReaquest(
        Guid Id,
        string Name,
        string Email,
        string PhoneNumber
    )
    {
    }
}
