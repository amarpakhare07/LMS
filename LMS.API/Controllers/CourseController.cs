﻿using LMS.Domain.Models;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService courseService;
        private readonly IFileUploadService _fileUploadService;
        private readonly FileUploadLimits _limits;
        private readonly IUserManagementRepository _userManagementRepository;
        


        public CourseController(ICourseService courseService, IFileUploadService fileService,
            IOptions<FileUploadLimits> limits, IUserManagementRepository _userManagementRepository)
        {
            this.courseService = courseService;
            _fileUploadService = fileService;
            _limits = limits.Value;
            this._userManagementRepository = _userManagementRepository;
            
        }

        // Implementation goes here
        [HttpPost]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> CreateCourse([FromBody] CreateCourseDto createCourseDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            // findinG from db
            var user = await _userManagementRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User not found" });
            var course = await courseService.CreateCourseAsync(createCourseDto,userId);
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

        [HttpPut("status/{courseId}")]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> UpdateCourseStatus(int courseId, [FromBody] UpdateCourseStatusDto updateCourseStatusDto)
        {
            var success = await courseService.UpdateCourseStatusAsync(courseId,updateCourseStatusDto);
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
        [HttpPost("{courseId}/document")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UploadCourseDocument(int courseId, IFormFile file)
        {
            if (file == null)
                return BadRequest(new { Message = "No file received" });

            try
            {
                var fileUrl = await _fileUploadService.SaveFileAsync(file, _limits.DocumentMaxSize);
                var originalFileName = file.FileName;

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();

                var success = await courseService.UpdateCourseMaterialAsync(courseId, fileUrl, originalFileName);
                if (!success)
                    return NotFound(new { Message = "CourseInstructor record not found" });

                return Ok(new { Url = fileUrl, FileName = originalFileName, Message = "Course material uploaded and saved" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}