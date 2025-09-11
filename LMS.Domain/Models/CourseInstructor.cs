using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Domain.Models
{
    public class CourseInstructor
    {
<<<<<<< HEAD
        public int CourseID { get; set; }
        public int UserID { get; set; }
        public DateTime AssignedAt { get; set; }

        public Course Course { get; set; } = null!;
=======
        public int UserID { get; set; }
        public DateTime AssignedAt { get; set; }

        public ICollection<Course> Courses { get; set; }
>>>>>>> 984598c1cab6b415a8f1a8296d9b85523e7a220f
        public User User { get; set; } = null!;
    }
}
