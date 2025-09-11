// Fluent API configuration for CourseInstructor
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Models;

namespace Lms.Data.EntityConfigurations {
    public class CourseInstructorConfiguration : IEntityTypeConfiguration<CourseInstructor> {
        public void Configure(EntityTypeBuilder<CourseInstructor> builder) {
            // Configuration goes here

            builder.HasKey(ci => new { ci.CourseID, ci.UserID });

            // Required Timestamp
            builder.Property(ci => ci.AssignedAt)
                   .IsRequired();

            // Relationships
            builder.HasMany(ci => ci.Courses)
                   .WithOne(c => c.CourseInstructor)
                   .HasForeignKey(ci => ci.CourseID)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ci => ci.User)
                   .WithOne(u => u.CourseInstructor)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}