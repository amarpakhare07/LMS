using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Infrastructure.Services.Interfaces;
using LMS.Domain.Models;

namespace LMS.Infrastructure.Services
{
    public class AnswerService : IAnswerService
    {
        private readonly IAnswerRepository answerRepository;
        private readonly IQuestionRepository questionRepository;
        public AnswerService(IAnswerRepository answerRepository, IQuestionRepository questionRepository)
        {
            this.answerRepository = answerRepository;
            this.questionRepository = questionRepository;
        }

        public async Task<CreateAnswerDto> CreateAnswerAsync(CreateAnswerDto createAnswerDto, int userId)
        {
            var answer = new Answer
            {
                QuestionID = createAnswerDto.QuestionID,
                UserID = userId,
                Response = createAnswerDto.Response,
            };
            var question = await questionRepository.GetQuestionByIdAsync(createAnswerDto.QuestionID);
            if(question == null)
            {
                throw new Exception("Question not found");
            }
            if(question.CorrectAnswer != null && createAnswerDto.Response != null)
            {
                answer.IsCorrect = question.CorrectAnswer.Equals(createAnswerDto.Response, StringComparison.OrdinalIgnoreCase);
                answer.MarksAwarded = answer.IsCorrect == true ? question.Marks : 0;
            }
            var createdAnswer = await answerRepository.CreateAnswerAsync(answer);
            return new CreateAnswerDto
            {
                QuestionID = createdAnswer.QuestionID,
                Response = createdAnswer.Response,
                IsCorrect = (bool)createdAnswer.IsCorrect
            };
        }
    }
}
