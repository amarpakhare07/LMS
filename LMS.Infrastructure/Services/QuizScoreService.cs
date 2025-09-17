using LMS.Domain.Models;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Services
{
    public class QuizScoreService : IQuizScoreService

    {
        private readonly IQuizScoreRepository quizScoreRepository;
        private readonly IAnswerRepository answerRepository;
        private readonly IQuestionRepository questionRepository;

        public QuizScoreService(IQuizScoreRepository quizScoreRepository, IAnswerRepository answerRepository, IQuestionRepository questionRepository)
        {
            this.quizScoreRepository = quizScoreRepository;
            this.answerRepository = answerRepository;
            this.questionRepository = questionRepository;
        }
        public async Task<IEnumerable<QuizScoreDto>> GetQuizScoresByCourseAsync(int courseId, int userId)
        {
            var quizScores = await quizScoreRepository.GetQuizScoresByCourseAsync(courseId, userId);
            var quizScoresDto = new List<QuizScoreDto>();
            foreach (var quizScore in quizScores)
            {
                quizScoresDto.Add(new QuizScoreDto
                {
                    ScoreID = quizScore.ScoreID,
                    QuizID = quizScore.QuizID,
                    UserID = quizScore.UserID,
                    Score = quizScore.Score,
                    CreatedAt = quizScore.CreatedAt
                });
            }
            return quizScoresDto;  
        }

        public async Task<CreateQuizScoreDto> CreateQuizScoreAsync(CreateQuizScoreDto createQuizScoreDto)
        {
            var allAnswers = await answerRepository.GetAnswersByQuizAndUserAsync(createQuizScoreDto.QuizID, createQuizScoreDto.UserID);
            var latestAttempt = allAnswers.Max(a => a.AttemptNumber);
            var latestAnswers = allAnswers.Where(a => a.AttemptNumber == latestAttempt).ToList();
       
            var questions = await questionRepository.GetQuestionsByQuizIdAsync(createQuizScoreDto.QuizID);
            int totalScore = 0;
            foreach (var question in questions)
            {
                var answer = latestAnswers.FirstOrDefault(a => a.QuestionID == question.QuestionID);
                if (answer != null && answer.IsCorrect == true)
                {
                    totalScore += (int)question.Marks;
                }
            }
            var quizScore = new QuizScore
            {
                QuizID = createQuizScoreDto.QuizID,
                UserID = createQuizScoreDto.UserID,
                Score = totalScore,
                AttemptNumber = latestAttempt,
                CreatedAt = DateTime.UtcNow
            };
            await quizScoreRepository.CreateQuizScoreAsync(quizScore);
            return new CreateQuizScoreDto
            {
                QuizID = quizScore.QuizID,
                UserID = quizScore.UserID,
                Score = quizScore.Score,
                AttemptNumber = quizScore.AttemptNumber
            };
        }

    }
}
