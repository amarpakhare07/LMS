using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LMS.Infrastructure.DTO
{
    public class CreateQuestionDto
    {
        [Required]
        public int QuizID { get; set; }

        [Required]
        public string QuestionText { get; set; } = null!;
        public string? QuestionType { get; set; }
        [Required]
        public string? Options { get; set; }
        [Required]
        public string? CorrectAnswer { get; set; }
        public int? Marks { get; set; }
    }

}
