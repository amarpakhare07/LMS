using LMS.Domain.Enums;
using LMS.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain
{
    public class LmsDbContext : DbContext
    {
        public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options) { }

        //public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CourseCategory> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<CourseInstructor> CourseInstructors { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Progress> Progresses { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<QuizScore> QuizScores { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(LmsDbContext).Assembly);
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserID = 1, // You must provide a primary key
                    Name = "Admin",
                    Email = "admin@cognizant.com",
                    PasswordHash = "admin@123",
                    IsActive = true,
                    Role = UserRole.Admin,
                    IsDeleted = false
                    // Omit properties with default values or that are nullable
                }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Properties.Any(p => p.Metadata.Name == "CreatedAt"))
                    {
                        var prop = entry.Property("CreatedAt");
                        if (prop.CurrentValue is null or DateTime { Year: <= 1 })
                            prop.CurrentValue = now;
                    }
                }
                if (entry.State == EntityState.Modified)
                {
                    if (entry.Properties.Any(p => p.Metadata.Name == "UpdatedAt"))
                    {
                        entry.Property("UpdatedAt").CurrentValue = now;
                    }
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
