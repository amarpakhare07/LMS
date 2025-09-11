// Fluent API configuration for Lesson
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Models;

namespace Lms.Data.EntityConfigurations {
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson> {
        public void Configure(EntityTypeBuilder<Lesson> builder) {
            // Configuration goes here
        }
    }
}