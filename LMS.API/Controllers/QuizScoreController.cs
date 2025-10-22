using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

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

        [HttpPost("submit")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitQuizScore([FromBody] CreateQuizScoreDto createQuizScoreDto)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("User ID not found in token.");

            // Validate user ID matches
            if (createQuizScoreDto.UserID != userId)
                return BadRequest("User ID mismatch.");

            // Validate inputs
            if (createQuizScoreDto.QuizID <= 0 || createQuizScoreDto.AttemptNumber <= 0)
                return BadRequest("Invalid quiz ID or attempt number.");

            try
            {
                var result = await _quizScoreService.CreateQuizScoreAsync(createQuizScoreDto);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred: " + ex.Message);
            }
        }

        [HttpGet("course/{courseId}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetQuizScoresByCourse(int courseId)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("User ID not found in token.");

            var quizScores = await _quizScoreService.GetQuizScoresByCourseAsync(courseId, userId);
            return Ok(quizScores);
        }
    }
}