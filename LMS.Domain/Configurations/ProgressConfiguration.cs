// Fluent API configuration for Progress
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Models;

namespace Lms.Data.EntityConfigurations {
    public class ProgressConfiguration : IEntityTypeConfiguration<Progress> {
        public void Configure(EntityTypeBuilder<Progress> builder) {
            // Configuration goes here
        }
    }
}