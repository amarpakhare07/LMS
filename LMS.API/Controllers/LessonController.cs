using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LessonController : ControllerBase
    {
        private readonly ILessonService lessonService;

        public LessonController(ILessonService lessonService)
        {
            this.lessonService = lessonService;
        }

        // Implementation goes here
        [HttpPost]
        //[Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateLessonAsync([FromBody] CreateLessonDto createLessonDto)
        {
            var lesson = await lessonService.CreateLessonAsync(createLessonDto);
            return Ok(lesson);
            //return CreatedAtAction(nameof(GetLessonByIdAsync), new { id = lesson.LessonID }, lesson);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetLessonByIdAsync(int lessonId)
        {
            var lesson = await lessonService.GetLessonByIdAsync(lessonId);
            if (lesson == null) return NotFound();
            return Ok(lesson);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetLessonsByCourseAsync(int courseId)
        {
            var lessons = await lessonService.GetLessonsByCourseAsync(courseId);
            return Ok(lessons);
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateLessonAsync(int id, [FromBody] LessonDto lessonDto)
        {
            //if (id != lessonDto.LessonID)
            //{
            //    return BadRequest("Lesson ID mismatch");
            //}

            var success = await lessonService.UpdateLessonAsync(lessonDto);
            if (success != null)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteLessonAsync(int id)
        {
            var success = await lessonService.DeleteLessonAsync(id);
            if (success==null)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}

