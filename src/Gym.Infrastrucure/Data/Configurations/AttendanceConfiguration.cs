
using Gym.Domain.Members.Attendances;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.Infrastrucure.Data.Configurations
{
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(a => a.CheckInTime)
                .IsRequired();

            builder.Property(a => a.CheckOutTime);
        }
    }
}
