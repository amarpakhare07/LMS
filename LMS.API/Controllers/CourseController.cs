using LMS.Domain.Models;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Services;
using LMS.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService courseService;
        private readonly IFileUploadService _fileUploadService;
        private readonly FileUploadLimits _limits;

        public CourseController(ICourseService courseService, IFileUploadService fileService,
            IOptions<FileUploadLimits> limits)
        {
            this.courseService = courseService;
            _fileUploadService = fileService;
            _limits = limits.Value;

        }

        // Implementation goes here
        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto createCourseDto)
        {
            var course = await courseService.CreateCourseAsync(createCourseDto);
            return CreatedAtAction(nameof(GetCourseById), new { id = course.CourseID }, course);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCourseById(int id)
        {
            var course = await courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllCourses()
        //public async Task<ActionResult<IEnumerable<CourseDto>>> GetAllCourses()
        {
            var courses = await courseService.GetAllCoursesAsync();
            return Ok(courses);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Instructor")]
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
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var success = await courseService.DeleteCourseAsync(id);
            if (success)
            {
                return NoContent();
            }
            return NotFound();
        }


        // COURSE MATERIAL UPLOAD
        [HttpPost("course/document")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UploadDocument(IFormFile file)
        {
            if (file == null)
                return BadRequest(new { Message = "No file received." });

            try
            {
                var fileName = await _fileUploadService.SaveFileAsync(file, _limits.DocumentMaxSize);
                return Ok(new { FileName = fileName, Message = "Document uploaded successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }


        }
    }
}
