using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentRepository _enrollmentRepository;
        public EnrollmentController(IEnrollmentRepository enrollmentRepository)
        {
            _enrollmentRepository = enrollmentRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Student")]  // Only students can enroll in courses
        public async Task<IActionResult> EnrollUser([FromBody] EnrollRequestDto requestEnrollmentDto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;


            if (requestEnrollmentDto == null)
                {
                return BadRequest("Invalid Course.");
            }
            // Simulate enrollment logic here
            var result = await _enrollmentRepository.EnrollUserAsync(requestEnrollmentDto.CourseId, int.Parse(userId));

            if (!result)
                return BadRequest("Enrollment failed.");

            return Ok(new { Message = "User enrolled successfully." });
        }

        [HttpGet("is-enrolled/{courseId}")]
        [Authorize(Roles = "Student,Instructor,Admin")]  // Students, Instructors, and Admins can check enrollment status
        public async Task<IActionResult> IsUserEnrolledAsync([FromRoute] int courseId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            var isEnrolled = await _enrollmentRepository.IsUserEnrolledAsync(courseId, int.Parse(userId));
            return Ok(new { isEnrolled });
        }


        [HttpGet("user/courses")]
        [Authorize(Roles = "Student,Instructor,Admin")]  // Students, Instructors, and Admins can view enrolled courses
        public async Task<IActionResult> GetEnrolledCourses()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid user ID.");
            }

           var courses = await _enrollmentRepository.GetEnrolledCoursesAsync(int.Parse(userId));

            return Ok(courses);
        }

    }
}
