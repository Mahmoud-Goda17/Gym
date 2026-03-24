using System.Net.Mail;
using System.Text.RegularExpressions;
using Gym.Domain.Common;
using Gym.Domain.Results;

namespace Gym.Domain.Trainers
{
    public sealed class Trainer : AuditableEntity
    {
        public string FullName { get; private set; }
        public string Email { get; private set; }
        public string Phone { get; private set; }
        public decimal HourlyRate { get; private set; }
        public bool IsActive { get; private set; }

        private Trainer() { }
        private Trainer(Guid id, string fullName, string email, string phone, decimal hourlyRate)
            : base(id)
        {
            FullName = fullName;
            Email = email;
            Phone = phone;
            HourlyRate = hourlyRate;
            IsActive = true;
        }
        public static Result<Trainer> Create(Guid id, string fullName, string email, string phone, decimal hourlyRate)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return TrainerErrors.FullNameRequired;
            }
            if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^\+?\d{7,15}$"))
            {
                return TrainerErrors.InvalidPhoneNumber;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                return TrainerErrors.EmailRequired;
            }
            if (hourlyRate <= 0)
            {
                return TrainerErrors.InvalidHourlyRate;

            }
            try
            {
                _ = new MailAddress(email);
            }
            catch
            {
                return TrainerErrors.EmailInvalid;
            }
            return new Trainer(id, fullName, email, phone, hourlyRate);
        }
        public Result<Updated> Update(string fullName, string email, string phone, decimal hourlyRate)
        {
            if (!IsActive)
            {
                return TrainerErrors.TrainerInactive;
            }
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return TrainerErrors.FullNameRequired;
            }
            if (string.IsNullOrWhiteSpace(phone) || !Regex.IsMatch(phone, @"^\+?\d{7,15}$"))
            {
                return TrainerErrors.InvalidPhoneNumber;
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                return TrainerErrors.EmailRequired;
            }
            if (hourlyRate <= 0)
            {
                return TrainerErrors.InvalidHourlyRate;
            }
            try
            {
                _ = new MailAddress(email);
            }
            catch
            {
                return TrainerErrors.EmailInvalid;
            }
            FullName = fullName;
            Email = email;
            Phone = phone;
            HourlyRate = hourlyRate;
            return Result.Updated;
        }
        public Result<Updated> Deactivate()
        {
            if (!IsActive)
            {
                return TrainerErrors.TrainerAlreadyInactive;
            }
            IsActive = false;
            return Result.Updated;
        }
        public Result<Updated> Activate()
        {
            if (IsActive)
            {
                return TrainerErrors.TrainerAlreadyActive;
            }
            IsActive = true;
            return Result.Updated;

        }
    }
}
