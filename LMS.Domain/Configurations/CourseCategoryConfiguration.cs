// Fluent API configuration for Category
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Models;

namespace LMS.Domain.Configurations {
    public class CourseCategoryConfiguration : IEntityTypeConfiguration<CourseCategory> {
        public void Configure(EntityTypeBuilder<CourseCategory> builder) {
            // Configuration goes here
        }
    }
}