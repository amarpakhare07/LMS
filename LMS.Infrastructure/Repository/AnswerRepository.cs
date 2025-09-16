using LMS.Infrastructure.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LMS.Domain.Models;
using LMS.Domain;

namespace LMS.Infrastructure.Repository
{
    public class AnswerRepository : IAnswerRepository
    {
        private readonly LmsDbContext _context;

        public AnswerRepository(LmsDbContext context)
        {
            _context = context;
        }
        public async Task<Answer> CreateAnswerAsync(Answer answer)
         {
            await _context.Answers.AddAsync(answer);    
            await _context.SaveChangesAsync();
            return answer;
        }
     
    }
}
