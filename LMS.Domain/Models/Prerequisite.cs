using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Models
{
    public class Prerequisite
    {
        public int PrerequisiteID { get; set; }
        public int CourseID { get; set; }
        public int PrerequisiteCourseID { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public Course Course { get; set; } = null!;
        public Course PrerequisiteCourse { get; set; } = null!;
    }
}
