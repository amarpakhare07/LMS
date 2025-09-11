// Fluent API configuration for CourseInstructor
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Models;

namespace Lms.Data.EntityConfigurations {
    public class CourseInstructorConfiguration : IEntityTypeConfiguration<CourseInstructor> {
        public void Configure(EntityTypeBuilder<CourseInstructor> builder) {
            // Configuration goes here
        }
    }
}