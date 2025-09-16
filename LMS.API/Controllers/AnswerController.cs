using Microsoft.AspNetCore.Mvc;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Services.Interfaces;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnswerController : ControllerBase
    {
        private readonly IAnswerService answerService;

        public AnswerController(IAnswerService answerService)
        {
            this.answerService = answerService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAnswer([FromBody] CreateAnswerDto createAnswerDto)
        {
            ////if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            var result = await answerService.CreateAnswerAsync(createAnswerDto);
            return Ok(result);
        }

       
    }
}
