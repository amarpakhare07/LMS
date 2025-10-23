using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CreateQuizScore(int quizId, [FromBody] CreateQuizScoreDto createQuizScoreDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (createQuizScoreDto == null || createQuizScoreDto.QuizID != quizId || createQuizScoreDto.UserID != int.Parse(userId))
            {
                return BadRequest();
            }

            var result = await _quizScoreService.CreateQuizScoreAsync(createQuizScoreDto);
            return Ok(result);
        }

        [HttpGet("course/{courseId}/user/{userId}")]
        public async Task<IActionResult> GetQuizScoresByCourse(int courseId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var quizScores = await _quizScoreService.GetQuizScoresByCourseAsync(courseId, int.Parse(userId));
            return Ok(quizScores);
        }
    }
}
