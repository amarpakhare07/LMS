using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LMS.Infrastructure.DTO
{
    public class CreateQuizScoreDto
    {
        [Required]
        public int QuizID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int Score { get; set; }

        public int AttemptNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
