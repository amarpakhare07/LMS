using LMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repository.Interfaces
{
    public interface IQuizScoreRepository
    {
        Task<IEnumerable<QuizScore>> GetQuizScoresByCourseAsync(int courseId, int userId);
        Task<QuizScore> CreateQuizScoreAsync(QuizScore quizScore);
    }
}
