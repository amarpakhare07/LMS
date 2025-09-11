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

            public int? TotalLessons { get; set; }

            public DateTime CreatedAt { get; set; }

            public DateTime? UpdatedAt { get; set; }

            public bool IsDeleted { get; set; }


        /// Because a Course can have multiple enrollments- one to many
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();


        // Because a Course can have multiple Quizzes- one to many
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();




        //public CourseCategory CourseCategory { get; set; } = null!;

        //public ICollection<CourseInstructor> CourseInstructors { get; set; } = new List<CourseInstructor>();

        //public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();

        //public ICollection<Prerequisite> Prerequisites { get; set; } = new List<Prerequisite>();

        //public ICollection<Prerequisite> IsPrerequisiteFor { get; set; } = new List<Prerequisite>();

        //public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        //public ICollection<Progress> Progresses { get; set; } = new List<Progress>();



        //public ICollection<Comment> Comments { get; set; } = new List<Comment>();




    }
}
