// Fluent API configuration for Role
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Models;

namespace LMS.Domain.Configurations {
    public class RoleConfiguration : IEntityTypeConfiguration<Role> {
        public void Configure(EntityTypeBuilder<Role> builder) {
            // Configuration goes here

            builder.ToTable("roles");
            builder.HasKey(x => x.RoleID);
            builder.Property(x => x.RoleID)
                .UseIdentityColumn();
            builder.Property(x => x.RoleName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(x => x.Description)
                .HasColumnType("varchar(max)")
                .IsUnicode(false);
            builder.Property(x => x.CreatedAt)
                .HasColumnType("datetime2")
                .HasDefaultValueSql("GETUTCDATE()");
            builder.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2");
            builder.Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            builder.HasIndex(x => x.RoleName)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0");
            builder.HasQueryFilter(x => !x.IsDeleted);

        }
    }
}