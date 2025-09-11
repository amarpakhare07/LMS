// Fluent API configuration for Enrollment
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Models;

namespace Lms.Data.EntityConfigurations {
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment> {
        public void Configure(EntityTypeBuilder<Enrollment> builder) {
            // Configuration goes here
        }
    }
}