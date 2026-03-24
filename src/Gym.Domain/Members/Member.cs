
using System.Net.Mail;
using System.Text.RegularExpressions;
using Gym.Domain.Common;
using Gym.Domain.Members.Attendances;
using Gym.Domain.Members.Attendances.Enums;
using Gym.Domain.Members.Memberships;
using Gym.Domain.Members.Memberships.Enums;
using Gym.Domain.Results;

namespace Gym.Domain.Members
{
    public sealed class Member : AuditableEntity
    {
        public string? FullName { get; private set; }
        public string? Email { get; private set; }
        public string? Phone { get; private set; }
        public bool IsActive { get; private set; }

        private readonly List<MemberShip> _memberships = [];
        public IEnumerable<MemberShip> memberShips => _memberships.AsReadOnly();

        private readonly List<Attendance> _attendances = [];
        public IEnumerable<Attendance> Attendances => _attendances.AsReadOnly();

        private Member()
        {
        }
        private Member(Guid id, string fullName, string email, string phone)
            : base(id)
        {
            FullName = fullName;
            Email = email;
            Phone = phone;
            IsActive = true;
        }
        public static Result<Member> Create(Guid id, string fullName, string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return MemberErrors.FullNameRequired;
            }
            if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^\+?\d{7,15}$"))
            {
                return MemberErrors.InvalidPhoneNumber;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                return MemberErrors.EmailRequired;
            }
            try
            {
                _ = new MailAddress(email);
            }
            catch
            {
                return MemberErrors.EmailInvalid;
            }

            return new Member(id, fullName, email, phone);
        }
        public Result<MemberShip> AddMemberShip(MembershipType type, DateTimeOffset startDate)
        {
            if (!Enum.IsDefined(type))
            {
                return MemberShipErrors.InvalidMembershipType;
            }

            if (!IsActive)
            {
                return MemberErrors.InactiveMember;
            }

            if (_memberships.Any(m => m.Type == type && m.Status == MembershipStatus.Active))
            {
                return MemberShipErrors.DuplicateActiveMembership;
            }
            var endDate = type switch
            {
                MembershipType.Monthly => startDate.AddMonths(1),
                MembershipType.Quarterly => startDate.AddMonths(3),
                MembershipType.Yearly => startDate.AddYears(1),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
            var membership = new MemberShip(Guid.NewGuid(), type, startDate, endDate);

            _memberships.Add(membership);
            return membership;
        }
        public Result<Attendance> CheckIn(DateTimeOffset checkInTime)
        {
            if (!IsActive)
            {
                return MemberErrors.InactiveMember;
            }
            if (_attendances.Any(a => a.status == Status.CheckedIn
                 && a.CheckInTime.Date == checkInTime.Date))
            {
                return AttendanceErrors.AlreadyCheckedIn;
            }
            if (!_memberships.Any(m =>
                m.Status == MembershipStatus.Active &&
                m.StartDate <= checkInTime &&
                m.EndDate >= checkInTime))
            {
                return MemberShipErrors.NoActiveMembership;
            }
            var attendance = new Attendance(Guid.NewGuid(), Id, checkInTime);
            _attendances.Add(attendance);
            return attendance;
        }
        public Result<Updated> Active()
        {
            if (IsActive)
            {
                return MemberErrors.AlreadyActive;
            }
            IsActive = true;
            return Result.Updated;
        }
        public Result<Deleted> DeActive()
        {
            if (!IsActive)
            {
                return MemberErrors.AlreadyInactive;
            }
            IsActive = false;
            return Result.Deleted;
        }
        public Result<Updated> Update(string fullName, string email, string phone)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return MemberErrors.FullNameRequired;
            }
            if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^\+?\d{7,15}$"))
            {
                return MemberErrors.InvalidPhoneNumber;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                return MemberErrors.EmailRequired;
            }
            try
            {
                _ = new MailAddress(email);
            }
            catch
            {
                return MemberErrors.EmailInvalid;
            }

            FullName = fullName;
            Email = email;
            Phone = phone;
            return Result.Updated;
        }
        public Result<Updated> Updatememberships(List<MemberShip> incomingmembership)
        {
            _memberships.RemoveAll(existing => incomingmembership.All(v => v.Id != existing.Id));

            foreach (var incoming in incomingmembership)
            {
                var existing = _memberships.FirstOrDefault(v => v.Id == incoming.Id);
                if (existing is null)
                {
                    _memberships.Add(incoming);
                }
                else
                {
                    var updatemembershipResult = existing.Update(incoming.Type, incoming.StartDate, incoming.EndDate, incoming.Status);

                    if (updatemembershipResult.IsError)
                    {
                        return updatemembershipResult.Errors;
                    }
                }
            }

            return Result.Updated;
        }
        public Result<Updated> CheckOut(DateTimeOffset checkOutTime)
        {
            var activeAttendance = _attendances.LastOrDefault(a => a.status == Status.CheckedIn);
            if (activeAttendance is null)
            {
                return AttendanceErrors.NoActiveCheckIn;
            }
            var checkOutResult = activeAttendance.CheckOut(checkOutTime);
            if (checkOutResult.IsError)
            {
                return checkOutResult.Errors;
            }
            return Result.Updated;
        }
        public Result<Updated> FrozenMempership(Guid Membershipid)
        {
            if (!IsActive)
            {
                return MemberErrors.InactiveMember;
            }
            var membership = _memberships.FirstOrDefault(ms => ms.Id == Membershipid && ms.Status == MembershipStatus.Active);
            var freez= membership.freezMembership();
            if (freez.IsError)
            {
                return freez.Errors;
            }
            return Result.Updated;
        }
        public Result<Updated> DeFrozenMempership(Guid Membershipid)
        {
            if (!IsActive)
            {
                return MemberErrors.InactiveMember;
            }
            var membership = _memberships.FirstOrDefault(ms => ms.Id == Membershipid && ms.Status == MembershipStatus.Frozen);
            var defreez= membership.DeFreezMemberShip();
            if (defreez.IsError)
            {
                return defreez.Errors;
            }
            return Result.Updated;
        }
    }
}
