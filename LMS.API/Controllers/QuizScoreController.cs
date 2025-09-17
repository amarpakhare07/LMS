using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizScoreController : ControllerBase
    {
        private readonly IQuizScoreService _quizScoreService;

        public QuizScoreController(IQuizScoreService quizScoreService)
        {
            _quizScoreService = quizScoreService;
        }

        [HttpPost("{quizId}/user/{userId}")]
        public async Task<IActionResult> CreateQuizScore(int quizId, int userId, [FromBody] CreateQuizScoreDto createQuizScoreDto)
        {
            if (createQuizScoreDto == null || createQuizScoreDto.QuizID != quizId || createQuizScoreDto.UserID != userId)
            {
                return BadRequest();
            }

            var result = await _quizScoreService.CreateQuizScoreAsync(createQuizScoreDto);
            return Ok(result);
        }

        [HttpGet("course/{courseId}/user/{userId}")]
        public async Task<IActionResult> GetQuizScoresByCourse(int courseId, int userId)
        {
            var quizScores = await _quizScoreService.GetQuizScoresByCourseAsync(courseId, userId);
            return Ok(quizScores);
        }
    }
}
