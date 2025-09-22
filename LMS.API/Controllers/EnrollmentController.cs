using LMS.Domain;
using LMS.Domain.Models;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            
            if (requestEnrollmentDto == null)
                {
                return BadRequest("Invalid enrollment data.");
            }
            // Simulate enrollment logic here
            var result = await _enrollmentRepository.EnrollUserAsync(requestEnrollmentDto);

            if (!result)
                return BadRequest("Enrollment failed.");

            return Ok("User enrolled successfully.");
        }

        [HttpPost("is-enrolled")]
        [Authorize(Roles = "Student,Instructor,Admin")]  // Students, Instructors, and Admins can check enrollment status
        public async Task<bool> IsUserEnrolledAsync(EnrollRequestDto requestEnrollmentDto)
        {
            return await _enrollmentRepository.IsUserEnrolledAsync(requestEnrollmentDto);
        }


        [HttpGet("user/{userId}/courses")]
        [Authorize(Roles = "Student,Instructor,Admin")]  // Students, Instructors, and Admins can view enrolled courses
        public async Task<IActionResult> GetEnrolledCourses(string userId)
        {



           var courses = await _enrollmentRepository.GetEnrolledCoursesAsync(int.Parse(userId));

            return Ok(courses);
        }

    }
}
