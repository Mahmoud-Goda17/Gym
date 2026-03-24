
using Gym.Domain.Trainers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.Infrastrucure.Data.Configurations
{
    public class TrainerConfiguration : IEntityTypeConfiguration<Trainer>
    {
        public void Configure(EntityTypeBuilder<Trainer> builder)
        {

            builder.HasKey(t => t.Id);

            builder.Property(t => t.FullName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.Email)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(t => t.Phone)
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(m => m.Email);
            builder.HasIndex(m => m.Phone);

            builder.HasIndex(m => new { m.Email, m.Phone })
                .IsUnique();


            builder.Property(t => t.HourlyRate)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        }
    }
}
