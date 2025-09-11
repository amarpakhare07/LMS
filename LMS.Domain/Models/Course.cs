using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LMS.Domain.Models
{
    public class Course
    {
  

            public int CourseID { get; set; }

            public string Title { get; set; } = null!;

            public string? Description { get; set; }

            public string? Syllabus { get; set; }

            public string? Level { get; set; }

            public string? Language { get; set; }

            public int? Duration { get; set; }

            public string? ThumbnailURL { get; set; }

            public int CategoryID { get; set; }

            public bool Published { get; set; }

            public double? Rating { get; set; }

            public int? ReviewCount { get; set; }

            public DateTime CreatedAt { get; set; }

            public DateTime? UpdatedAt { get; set; }

            public bool IsDeleted { get; set; }

        public ICollection<User> Users { get; set; } 
        public ICollection<Enrollment> Enrollments { get; set; } 

        public CourseCategory Category { get; set; } = null!;

        public CourseInstructor CourseInstructor { get; set; } 

        public ICollection<Lesson> Lessons { get; set; }

        public ICollection<Prerequisite> Prerequisites { get; set; } 

        public ICollection<Prerequisite> IsPrerequisiteFor { get; set; } 

        public ICollection<Progress> Progresses { get; set; } 

        public ICollection<Quiz> Quizzes { get; set; } 

        public ICollection<Comment> Comments { get; set; } 




    }
}
