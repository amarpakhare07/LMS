using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LMS.Domain.Models;
using Microsoft.AspNetCore.Authorization;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService quizService;
        public QuizController(IQuizService quizService)
        {
            this.quizService = quizService;
        }

      
        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateQuizAsync([FromBody] CreateQuizDto createQuizDto)
        {
            var quizDto = await quizService.CreateQuizAsync(createQuizDto);
            if(quizDto == null) return BadRequest("Could not create quiz");
            return Ok(quizDto);
        }

        [HttpGet("get-by-quizId/{quizId}")]
       
        public async Task<IActionResult> GetQuizByIdAsync([FromRoute] int quizId)
        {
            var quiz = await quizService.GetQuizByIdAsync(quizId);
            if (quiz == null) return NotFound();
            return Ok(quiz);
        }

        [HttpGet("get-by-courseId/{courseId}")]
        [Authorize(Roles = "Student, Instructor")]
        public async Task<IActionResult> GetQuizzesByCourseAsync([FromRoute] int courseId)
        {
            var quizzes = await quizService.GetQuizzesByCourseAsync(courseId);
            return Ok(quizzes);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteQuizAsync([FromRoute] int id)
        {
            var success = await quizService.DeleteQuizAsync(id);
            if (success)
            {
                return NoContent();
            }
            return NotFound();
        }

    }
}
