using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using LMS.Domain.Models;

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
        //[Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateQuizAsync([FromBody] CreateQuizDto createQuizDto)
        {
            var quiz = await quizService.CreateQuizAsync(createQuizDto);
            if(quiz == null) return BadRequest("Could not create quiz");
            return Ok(quiz);
        }

        [HttpGet("{id}")]
       
        public async Task<IActionResult> GetQuizByIdAsync(int quizId)
        {
            var quiz = await quizService.GetQuizByIdAsync(quizId);
            if (quiz == null) return NotFound();
            return Ok(quiz);
        }

        [HttpGet]
       
        public async Task<IActionResult> GetQuizzesByCourseAsync(int courseId)
        {
            var quizzes = await quizService.GetQuizzesByCourseAsync(courseId);
            return Ok(quizzes);
        }


        [HttpDelete("{id}")]
        //[Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteQuizAsync(int id)
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
