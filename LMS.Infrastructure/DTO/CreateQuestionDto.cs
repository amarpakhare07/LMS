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
        
        [Required(ErrorMessage = "Question text is required.")]
        public string QuestionText { get; set; } = null!;
        public string? QuestionType { get; set; }
        [Required(ErrorMessage = "Options are required.")]
        public string? Options { get; set; }
        [Required(ErrorMessage = "Correct answer is required.")]
        public string? CorrectAnswer { get; set; }
        public int? Marks { get; set; }
    }

}
