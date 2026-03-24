
using Gym.Domain.ClassBooking;
using Gym.Domain.GymClasss;
using Gym.Domain.Members;
using Gym.Domain.Members.Attendances;
using Gym.Domain.Members.Memberships;
using Gym.Domain.Payment;
using Gym.Domain.Trainers;
using Microsoft.EntityFrameworkCore;

namespace Gym.Application.Interfaces
{
    public interface IAppDbContext
    {
        public DbSet<Member> Members { get; }
        public DbSet<ClassBooking> ClassBooking { get; }  
        public DbSet<GymClass> GymClasses { get; }
        public DbSet<Trainer> Trainers { get; }
        public DbSet<Attendance> Attendances { get; }
        public DbSet<MemberShip> MemberShips { get; }
        public DbSet<Payment> Payments { get; }  

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
