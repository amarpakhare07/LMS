using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Infrastructure.Services.Interfaces;


namespace LMS.Infrastructure.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository quizRepository;
        public QuizService(IQuizRepository quizRepository)
        {
            this.quizRepository = quizRepository;
        }

        public async Task<IEnumerable<QuizDto>> GetQuizzesByCourseAsync(int courseId)
        {
            var quizzes = await quizRepository.GetQuizzesByCourseAsync(courseId);
            if (quizzes == null) return new List<QuizDto>();

            var quizDtos = new List<QuizDto>();
            foreach(var quizDto in quizzes)
            {
                quizDtos.Add(new QuizDto
                {
                    QuizID = quizDto.QuizID,
                    CourseID = quizDto.CourseID,
                    Title = quizDto.Title,
                    TotalMarks = quizDto.TotalMarks,
                    TimeLimit = quizDto.TimeLimit,
                    CreatedAt = quizDto.CreatedAt,
                    AttemptsAllowed = quizDto.AttemptsAllowed,

                });

            }
            return quizDtos;
        }

        public async Task<QuizDto?> GetQuizByIdAsync(int quizId)
        {
            var quiz = await quizRepository.GetQuizByIdAsync(quizId);
            if (quiz == null) return null;
            return new QuizDto
            {
                QuizID = quiz.QuizID,
                CourseID = quiz.CourseID,
                Title = quiz.Title,
                TotalMarks = quiz.TotalMarks,
                TimeLimit = quiz.TimeLimit,
                CreatedAt = quiz.CreatedAt,
                AttemptsAllowed = quiz.AttemptsAllowed,
            };
        }

        public async Task<CreateQuizDto> CreateQuizAsync(CreateQuizDto createQuizDto)
        {
            var newQuiz = new Quiz
            {
                CourseID = createQuizDto.CourseID,
                Title = createQuizDto.Title,
                TotalMarks = createQuizDto.TotalMarks,
                TimeLimit = createQuizDto.TimeLimit,
                AttemptsAllowed = createQuizDto.AttemptsAllowed,
                CreatedAt = DateTime.UtcNow,
            };
            var createdQuiz = await quizRepository.CreateQuizAsync(newQuiz);
            return createQuizDto;
        }

        public async Task<bool> DeleteQuizAsync(int quizId)
        {
            return await quizRepository.DeleteQuizAsync(quizId) != null;
        }
    }
}
