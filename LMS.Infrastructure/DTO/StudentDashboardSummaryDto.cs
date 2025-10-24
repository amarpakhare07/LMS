using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.DTO
{
    public class StudentDashboardSummaryDto
    {
        public int EnrolledCoursesCount { get; set; }
        public int CompletedCoursesCount { get; set; }
        public double OverallProgressPercentage { get; set; } // Average progress of all enrolled courses
    }
}
