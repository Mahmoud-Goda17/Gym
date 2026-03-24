
using Gym.Domain.Members;
using Gym.Domain.Members.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.Infrastrucure.Data.Configurations
{
    public class MemberConfiguration : IEntityTypeConfiguration<Member>
    {
        public void Configure(EntityTypeBuilder<Member> builder)
        {

            builder.HasKey(m => m.Id);

            builder.Property(m => m.FullName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(m => m.Email)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(m => m.Phone)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(m => m.IsActive)
                .IsRequired();

            builder.HasIndex(m => m.Email);
            builder.HasIndex(m => m.Phone);

            builder.HasIndex(m => new { m.Email, m.Phone })
                .IsUnique();

            builder.Navigation(m => m.Attendances)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(typeof(MemberShip), "_memberships")
                .WithOne()
               .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m=>m.Attendances)
                .WithOne()
                .HasForeignKey(a => a.MeberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
