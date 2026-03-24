
using Gym.Domain.GymClasss;
using Gym.Domain.Trainers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gym.Infrastrucure.Data.Configurations
{
    public class GymClassConfiguration : IEntityTypeConfiguration<GymClass>
    {
        public void Configure(EntityTypeBuilder<GymClass> builder)
        {

            builder.HasKey(g => g.Id);

            builder.Property(g => g.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(g => g.Capacity)
                .IsRequired();

            builder.Property(g => g.BookedCount)
                .IsRequired();

            builder.HasOne<Trainer>()
                .WithMany()
                .HasForeignKey(g => g.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
