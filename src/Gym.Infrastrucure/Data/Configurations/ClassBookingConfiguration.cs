
using Gym.Domain.ClassBooking;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.Infrastrucure.Data.Configurations
{
    public class ClassBookingConfiguration : IEntityTypeConfiguration<ClassBooking>
    {
        public void Configure(EntityTypeBuilder<ClassBooking> builder)
        {

            builder.HasKey(cb => cb.Id);

            builder.Property(cb => cb.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(cb => cb.BookedAt)
                .IsRequired();

            builder.HasOne(cb => cb.member)
                .WithMany()
                .HasForeignKey(cb => cb.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cb => cb.gymClass)
                .WithMany()
                .HasForeignKey(cb => cb.ClassId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
