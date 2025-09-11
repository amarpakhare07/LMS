// Fluent API configuration for QuizScore
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Models;

namespace Lms.Data.EntityConfigurations {
    public class QuizScoreConfiguration : IEntityTypeConfiguration<QuizScore> {
        public void Configure(EntityTypeBuilder<QuizScore> builder) {
            // Configuration goes here
        }
    }
}