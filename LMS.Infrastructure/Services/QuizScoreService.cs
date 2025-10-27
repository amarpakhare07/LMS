﻿using LMS.Domain.Models;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Services
{
    public class QuizScoreService : IQuizScoreService
    {
        private readonly IQuizScoreRepository quizScoreRepository;
        private readonly IAnswerRepository answerRepository;
        private readonly IQuizRepository quizRepository;

        public QuizScoreService(
            IQuizScoreRepository quizScoreRepository,
            IAnswerRepository answerRepository,
            IQuizRepository quizRepository)
        {
            this.quizScoreRepository = quizScoreRepository;
            this.answerRepository = answerRepository;
            this.quizRepository = quizRepository;
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
                    AttemptNumber = quizScore.AttemptNumber,
                    CreatedAt = quizScore.CreatedAt
                });
            }

            return quizScoresDto;
        }

        public async Task<CreateQuizScoreDto> CreateQuizScoreAsync(CreateQuizScoreDto createQuizScoreDto)
        {
            // Step 1: Validate quiz exists
            var quiz = await quizRepository.GetQuizByIdAsync(createQuizScoreDto.QuizID);
            if (quiz == null)
                throw new Exception("Quiz not found.");

            // Step 2: Check if score already exists for this attempt
            var existingScore = await quizScoreRepository.GetQuizScoreAsync(
                createQuizScoreDto.UserID,
                createQuizScoreDto.QuizID,
                createQuizScoreDto.AttemptNumber
            );

            if (existingScore != null)
            {
                throw new InvalidOperationException("Score already calculated for this attempt.");
            }

            // Step 3: Get all answers for this specific attempt
            var allAnswers = await answerRepository.GetAnswersByQuizAndUserAsync(
                createQuizScoreDto.QuizID,
                createQuizScoreDto.UserID
            );

            var currentAttemptAnswers = allAnswers
                .Where(a => a.AttemptNumber == createQuizScoreDto.AttemptNumber)
                .ToList();

            if (!currentAttemptAnswers.Any())
            {
                throw new Exception("No answers found for this attempt.");
            }

            // Step 4: Calculate total score (answers already have MarksAwarded from AnswerService)
            int totalScore = currentAttemptAnswers.Sum(a => a.MarksAwarded ?? 0);

            // Step 5: Create QuizScore entry
            var quizScore = new QuizScore
            {
                UserID = createQuizScoreDto.UserID,
                QuizID = createQuizScoreDto.QuizID,
                Score = totalScore,
                AttemptNumber = createQuizScoreDto.AttemptNumber,
                CreatedAt = DateTime.UtcNow
            };

            var createdScore = await quizScoreRepository.CreateQuizScoreAsync(quizScore);

            return new CreateQuizScoreDto
            {
                QuizID = createdScore.QuizID,
                UserID = createdScore.UserID,
                Score = createdScore.Score,
                AttemptNumber = createdScore.AttemptNumber
            };
        }



        //public async Task<IEnumerable<CourseAverageScoreDto>> GetAverageCourseScoresByUserAsync(int userId)
        //{
        //    // This relies on the new repository method to efficiently calculate the average
        //    // of the *highest score* per quiz for all quizzes within each course.
        //    var averageScores = await quizScoreRepository.GetAverageCourseScoresByUserAsync(userId);
        //    return averageScores;
        //}


    }
}