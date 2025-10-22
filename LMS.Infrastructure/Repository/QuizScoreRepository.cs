﻿using LMS.Domain;
using LMS.Domain.Models;
using LMS.Infrastructure.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repository
{
    public class QuizScoreRepository : IQuizScoreRepository
    {
        private readonly LmsDbContext _dbContext;

        public QuizScoreRepository(LmsDbContext context)
        {
            _dbContext = context;
        }

        public async Task<IEnumerable<QuizScore>> GetQuizScoresByCourseAsync(int courseId, int userId)
        {
            return await _dbContext.QuizScores
                .Include(qs => qs.Quiz)
                .Where(qs => qs.Quiz.CourseID == courseId && qs.UserID == userId)
                .OrderByDescending(qs => qs.CreatedAt)
                .ToListAsync();
        }

        public async Task<QuizScore> CreateQuizScoreAsync(QuizScore quizScore)
        {
            _dbContext.QuizScores.Add(quizScore);
            await _dbContext.SaveChangesAsync();
            return quizScore;
        }

        public async Task<QuizScore> GetQuizScoreAsync(int userId, int quizId, int attemptNumber)
        {
            return await _dbContext.QuizScores
                .FirstOrDefaultAsync(q => q.UserID == userId
                                       && q.QuizID == quizId
                                       && q.AttemptNumber == attemptNumber);
        }
    }
}