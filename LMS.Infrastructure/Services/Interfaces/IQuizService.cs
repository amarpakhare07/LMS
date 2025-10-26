using System.Collections.Generic;
using System.Threading.Tasks;
using LMS.Infrastructure.DTO;

namespace LMS.Infrastructure.Services.Interfaces
{
    public interface IQuizService
    {
        Task<IEnumerable<QuizDto>> GetQuizzesByCourseAsync(int courseId);
        Task<QuizDto?> GetQuizByIdAsync(int quizId);
        Task<QuizDto> CreateQuizAsync(CreateQuizDto quizDto);
        Task<bool> DeleteQuizAsync(int quizId);
        Task<StartQuizResponseDto> StartQuizAsync(int quizId, int userId);

        Task<IEnumerable<QuizSummaryDto>> GetQuizSummariesByUserAsync(int userId);


    }
}