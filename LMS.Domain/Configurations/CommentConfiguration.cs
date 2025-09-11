// Fluent API configuration for Comment
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Models;

namespace Lms.Data.EntityConfigurations {
    public class CommentConfiguration : IEntityTypeConfiguration<Comment> {
        public void Configure(EntityTypeBuilder<Comment> builder) {
            // Configuration goes here
        }
    }
}