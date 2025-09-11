// Fluent API configuration for Quiz
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Models;

namespace Lms.Data.EntityConfigurations {
    public class QuizConfiguration : IEntityTypeConfiguration<Quiz> {
        public void Configure(EntityTypeBuilder<Quiz> builder) {
            // Configuration goes here
        }
    }
}