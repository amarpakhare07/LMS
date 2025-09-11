// Fluent API configuration for Prerequisite
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LMS.Domain.Models;

namespace Lms.Data.EntityConfigurations {
    public class PrerequisiteConfiguration : IEntityTypeConfiguration<Prerequisite> {
        public void Configure(EntityTypeBuilder<Prerequisite> builder) {
            // Configuration goes here
        }
    }
}