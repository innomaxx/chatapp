
using Chat.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Web.Data.Configurations
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.ToTable("Rooms");

            builder.Property(room => room.Name).IsRequired().HasMaxLength(100);

            builder.HasOne(room => room.Admin)
                .WithMany(user => user.Rooms)
                .IsRequired();
        }
    }
}
