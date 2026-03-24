
using Gym.Application.Identity;
using Gym.Application.Interfaces;
using Gym.Domain.ClassBooking;
using Gym.Domain.GymClasss;
using Gym.Domain.Members;
using Gym.Domain.Members.Attendances;
using Gym.Domain.Members.Memberships;
using Gym.Domain.Payment;
using Gym.Domain.Trainers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gym.Infrastrucure.Data
{
    public class AppDbContext:IdentityDbContext<AppUser>, IAppDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Member> Members => Set<Member>();

        public DbSet<ClassBooking> ClassBooking => Set<ClassBooking>();

        public DbSet<GymClass> GymClasses => Set<GymClass>();

        public DbSet<Trainer> Trainers => Set<Trainer>();

        public DbSet<Attendance> Attendances => Set<Attendance>();

        public DbSet<MemberShip> MemberShips => Set<MemberShip>();

        public DbSet<Payment> Payments => Set<Payment>();
        Task<int> IAppDbContext.SaveChangesAsync(CancellationToken ct)
            => base.SaveChangesAsync(ct);
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);
        }

    }
}
