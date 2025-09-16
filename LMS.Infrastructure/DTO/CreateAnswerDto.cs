using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LMS.Infrastructure.DTO
{
    public class CreateAnswerDto
    {
        [Required]
        public int QuestionID { get; set; }
        [Required]
        public int UserID { get; set; }
        public string? Response { get; set; }
        public bool IsCorrect { get; set; } = false;
    }
}
