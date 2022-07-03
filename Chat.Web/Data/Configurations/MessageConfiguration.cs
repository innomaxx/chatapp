
using Chat.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chat.Web.Data.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");

            builder.Property(message => message.Content).IsRequired().HasMaxLength(500);

            builder.HasOne(message => message.ToRoom)
                .WithMany(room => room.Messages)
                .HasForeignKey(message => message.ToRoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
