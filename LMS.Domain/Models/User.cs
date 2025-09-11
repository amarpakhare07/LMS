using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LMS.Domain.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!; 
        public string? ProfilePicture { get; set; }
        public string? Bio { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public int RoleID { get; set; }
        public bool IsDeleted { get; set; }


        public Role Role { get; set; } = null!;
<<<<<<< HEAD
        public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<Progress> Progresses { get; set; } = new List<Progress>();
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
        public ICollection<QuizScore> QuizScores { get; set; } = new List<QuizScore>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Messages> Messages { get; set; } = new List<Messages>();
=======
        public ICollection<Course> Courses { get; set; }
        public ICollection<CourseInstructor> CourseInstructors { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Progress> Progresses { get; set; }
        public ICollection<Answer> Answers { get; set; } 
        public ICollection<QuizScore> QuizScores { get; set; } 
        public ICollection<Comment> Comments { get; set; } 
        public ICollection<Messages> Messages { get; set; } 
>>>>>>> 984598c1cab6b415a8f1a8296d9b85523e7a220f
    }
}
