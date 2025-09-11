// Fluent API configuration for Notification
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Models;

namespace LMS.Domain.Configurations {
    public class MessagesConfiguration : IEntityTypeConfiguration<Messages> {
        public void Configure(EntityTypeBuilder<Messages> builder) {
            // Configuration goes here

            builder.ToTable("messages");
            builder.HasKey(x => x.MessageID);
            builder.Property(x => x.MessageID).UseIdentityColumn();
            builder.Property(x => x.Message).IsRequired().HasColumnType("varchar(max)").IsUnicode(false);
            builder.Property(x => x.IsRead).HasDefaultValue(false);
            builder.Property(x => x.CreatedAt).HasColumnType("datetime2").HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.MessageType).HasMaxLength(50).IsUnicode(false);
            builder.Property(x => x.TargetURL).HasMaxLength(2048).IsUnicode(false);
            builder.Property(x => x.IsDeleted).HasDefaultValue(false);

            builder.HasOne(x => x.User)
                .WithMany(u => u.Messages)
                .HasForeignKey(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.UserID, x.IsRead, x.CreatedAt });
            builder.HasQueryFilter(x => !x.IsDeleted);

        }
    }
}