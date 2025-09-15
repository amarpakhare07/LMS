using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService courseService;

        public CourseController(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        // Implementation goes here
        [HttpPost]
        //[AllowAnonymous]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto createCourseDto)
        {
            var course = await courseService.CreateCourseAsync(createCourseDto);
            return CreatedAtAction(nameof(GetCourseById), new { id = course.CourseID }, course);
        }

        [HttpGet("{id}")]
        //[AllowAnonymous]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpGet]
        //[AllowAnonymous]
        public async Task<IActionResult> GetAllCourses()
        //public async Task<ActionResult<IEnumerable<CourseDto>>> GetAllCourses()
        {
            var courses = await courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseDto courseDto)
        {
            if (id != courseDto.CourseID)
            {
                return BadRequest("Course ID mismatch");
            }

            var success = await courseService.UpdateCourseAsync(courseDto);
            if (success != null)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var success = await courseService.DeleteCourseAsync(id);
            if (success)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
