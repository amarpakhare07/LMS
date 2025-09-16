using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Domain.Models;
using LMS.Infrastructure.DTO;

namespace LMS.Infrastructure.Services.Interfaces
{
    public interface IQuizService
    {
        Task<IEnumerable<QuizDto>> GetQuizzesByCourseAsync(int courseId);
        Task<QuizDto?> GetQuizByIdAsync(int quizId);
        Task<CreateQuizDto> CreateQuizAsync(CreateQuizDto quizDto);
     
        Task<bool> DeleteQuizAsync(int quizId);
    }
}
