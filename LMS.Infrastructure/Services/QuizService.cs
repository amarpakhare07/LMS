using LMS.Domain.Models;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Services
{
    public class QuizService : IQuizService
    {
        private readonly IQuizRepository quizRepository;
        private readonly IAnswerRepository answerRepository;
        private readonly IQuizScoreRepository quizScoreRepository;  

        public QuizService(IQuizRepository quizRepository, IAnswerRepository answerRepository, IQuizScoreRepository quizScoreRepository)
        {
            this.quizRepository = quizRepository;
            this.answerRepository = answerRepository;
            this.quizScoreRepository = quizScoreRepository;
        }

        public async Task<IEnumerable<QuizDto>> GetQuizzesByCourseAsync(int courseId)
        {
            var quizzes = await quizRepository.GetQuizzesByCourseAsync(courseId);
            if (quizzes == null) return new List<QuizDto>();

            var quizDtos = new List<QuizDto>();
            foreach (var quizDto in quizzes)
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

        public async Task<QuizDto> CreateQuizAsync(CreateQuizDto createQuizDto)
        {
            var newQuiz = new Quiz
            {
                CourseID = createQuizDto.CourseID,
                Title = createQuizDto.Title,
                TotalMarks = createQuizDto.TotalMarks,
                TimeLimit = createQuizDto.TimeLimit,
                AttemptsAllowed = createQuizDto.AttemptsAllowed,
            };
            var createdQuiz = await quizRepository.CreateQuizAsync(newQuiz);
            return new QuizDto
            {
                QuizID = createdQuiz.QuizID,
                CourseID = createdQuiz.CourseID,
                Title = createdQuiz.Title,
                TotalMarks = createdQuiz.TotalMarks,
                TimeLimit = createdQuiz.TimeLimit,
                CreatedAt = createdQuiz.CreatedAt,
                AttemptsAllowed = createdQuiz.AttemptsAllowed,
            };
        }

        public async Task<bool> DeleteQuizAsync(int quizId)
        {
            return await quizRepository.DeleteQuizAsync(quizId) != null;
        }

        public async Task<StartQuizResponseDto> StartQuizAsync(int quizId, int userId)
        {
            // Get quiz details
            var quiz = await quizRepository.GetQuizByIdAsync(quizId);
            if (quiz == null)
                throw new Exception("Quiz not found.");

            // Get previous attempts
            var previousAnswers = await answerRepository.GetAnswersByQuizAndUserAsync(quizId, userId);
            int currentAttemptNumber = previousAnswers.Any()
                ? previousAnswers.Max(a => a.AttemptNumber) + 1
                : 1;

            // Check if attempts exceeded
            if (quiz.AttemptsAllowed.HasValue && currentAttemptNumber > quiz.AttemptsAllowed.Value)
            {
                throw new InvalidOperationException($"Maximum attempts ({quiz.AttemptsAllowed.Value}) reached for this quiz.");
            }

            return new StartQuizResponseDto
            {
                QuizID = quiz.QuizID,
                Title = quiz.Title,
                TotalMarks = quiz.TotalMarks,
                TimeLimit = quiz.TimeLimit,
                AttemptsAllowed = quiz.AttemptsAllowed,
                CurrentAttemptNumber = currentAttemptNumber,
                RemainingAttempts = quiz.AttemptsAllowed.HasValue
                    ? quiz.AttemptsAllowed.Value - currentAttemptNumber + 1
                    : null
            };
        }


        public async Task<IEnumerable<QuizSummaryDto>> GetQuizSummariesByUserAsync(int userId)
        {
            // NOTE: A robust solution would filter quizzes by the user's enrolled courses.
            // This implementation fetches all active quizzes and then checks scores for the user.
            var allQuizzes = await quizRepository.GetAllActiveQuizzesWithCourseAsync();

            var quizSummaries = new List<QuizSummaryDto>();
            int srNo = 1;

            foreach (var quiz in allQuizzes)
            {
                // Get user's score summary (highest score and max attempt)
                var scoreSummary = await quizScoreRepository.GetQuizScoreSummaryForUserAsync(quiz.QuizID, userId);

                int maxAttempt = scoreSummary.MaxAttemptNumber;

                // Treat null AttemptsAllowed as functionally unlimited (Int32.MaxValue)
                int attemptsAllowed = quiz.AttemptsAllowed ?? int.MaxValue;

                int attemptsLeft = attemptsAllowed - maxAttempt;

                // Ensure attemptsLeft is not negative
                if (attemptsLeft < 0) attemptsLeft = 0;

                // Try to get Course Title from the navigation property (assuming it exists and has a Title property)
                string courseTitle = (quiz.Course != null && !string.IsNullOrEmpty(quiz.Course.Title))
                                    ? quiz.Course.Title : "Course Title Not Found";

                quizSummaries.Add(new QuizSummaryDto
                {
                    SrNo = srNo++,
                    QuizID = quiz.QuizID,
                    QuizTitle = quiz.Title,
                    TotalMarks = quiz.TotalMarks,
                    HighestScore = scoreSummary.HighestScore,
                    AttemptsAllowed = quiz.AttemptsAllowed, // Send the original value (null or int)
                    AttemptsLeft = attemptsLeft,
                    CourseID = quiz.CourseID,
                    CourseTitle = courseTitle
                });
            }

            return quizSummaries;
        }

    }
}