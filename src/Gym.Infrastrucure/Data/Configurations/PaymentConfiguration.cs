
using Gym.Domain.Members.Memberships;
using Gym.Domain.Members;
using Gym.Domain.Payment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.Infrastrucure.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.Currency)
                .HasMaxLength(10)
                .IsRequired();

            builder.Property(p => p.Status)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(p => p.ExternalPaymentId)
                .HasMaxLength(200);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.HasOne<Member>()
                .WithMany()
                .HasForeignKey(p => p.MemberId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<MemberShip>()
                .WithMany(p=>p.Payments)
                .HasForeignKey(p => p.MembershipId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.ExternalPaymentId)
                .IsUnique(false);

            builder.HasIndex(p => new { p.MemberId, p.Status });
        }
    }
}
