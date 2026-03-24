
using Gym.Domain.Members.Memberships;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.Infrastrucure.Data.Configurations
{
    public class MemberShipConfiguration : IEntityTypeConfiguration<MemberShip>
    {
        public void Configure(EntityTypeBuilder<MemberShip> builder)
        {

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Type)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(m => m.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(m => m.StartDate)
                .IsRequired();

            builder.Property(m => m.EndDate)
                .IsRequired();

            builder.Property(m => m.IsFrozen)
                .IsRequired();

            builder.Navigation(m => m.Payments)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
