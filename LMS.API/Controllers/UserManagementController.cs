using LMS.Domain.Models;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

using LMS.Infrastructure.DTO;
using LMS.Domain.Enums;


namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {

        // UsermanagementRepo is private declared inside controller and its type is interface(IUserRepo)
        public readonly IUserManagementRepository _userManagementRepository;


        public readonly IFileUploadService _fileService;
        private readonly FileUploadLimits _limits;

        public UserManagementController(
            IUserManagementRepository userManagementRepository,
            IFileUploadService fileService,
            IOptions<FileUploadLimits> limits)
        {
            _userManagementRepository = userManagementRepository;
            _fileService = fileService;
            _limits = limits.Value;
        }


        // GET USER FULL PROFILEE //
        [HttpGet("me")]
        [Authorize]  // ensures only logged in user can acccess
        public async Task<IActionResult> GetMyProfile()
        {
            // get from jwt claim--> claim(a simple card with detail to verify the legit user)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
                return Unauthorized();

            var userId = int.Parse(userIdClaim);

            // findinG from db
            var user = await _userManagementRepository.GetByIdAsync(userId);
            if (user == null)
                return NotFound(new { Message = "User not found" });

            //Map to DTO
            var profile = new UserProfileDto
            {
                UserId = user.UserID,
                Name = user.Name,
                Email = user.Email,
                Bio = user.Bio,

                ProfilePicture = user.ProfilePicture,
                LastLogin = user.LastLogin,  // refering from user model
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted
            };

            return Ok(profile);

        }
        // UPDATING THE BIO
        [HttpPut("me/bio")]
        [Authorize]
        public async Task<IActionResult> UpdateBio([FromBody] UserUpdateProfileBioDto bio)
        {
            var userIdClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdClaims, out int userID))
            {
                var user = await _userManagementRepository.GetByIdAsync(userID);
                if (user == null)
                {
                    return NotFound(new { Message = "User not found" });
                }
                user.Bio = bio.Bio;
                user.UpdatedAt = DateTime.UtcNow;

                await _userManagementRepository.UpdateBioAsync(user.UserID, user.Bio);
                //return Ok(new { Message = "Bio updated successfully" });
                return Ok(new { Message = "Bio updated", UpdatedAt = user.UpdatedAt });

            }
            else
            {
                return Unauthorized();
            }
        }


        // Delete the user means changing the flag only
        [HttpDelete("me")]
        [Authorize]
        public async Task<IActionResult> DeleteProfile() //Dont need to pass the userId inside as a prompt as we are extracting it from the JWT
        {
            var userIdClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaims, out int userId)) return Unauthorized();
            var deleted = await _userManagementRepository.DeleteUserasync(userId);
            if (!deleted) return NotFound(new { Message = "user not found" });
            return Ok(new { Message = "User deleted successfully" });
        }


        // Showing courses enrolled by user

        [HttpGet("me/courses")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserEnrolledCourse()
        {
            var userIdClaims = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaims, out int userId)) return Unauthorized();
            var enrollments = await _userManagementRepository.GetUserEnrolledCourses(userId);
            if (enrollments == null || !enrollments.Any())
            {
                return NotFound(new { Message = "No enrolled courses found" });
            }
            // Map to DTO
            var enrolledCourses = enrollments.Select(e => new UserEnrolledCourse
            {
                CourseID = e.Course.CourseID,
                Title = e.Course.Title,
                Description = e.Course.Description,
                Level = e.Course.Level,
                Language = e.Course.Language,
                Duration = e.Course.Duration,
                ThumbnailURL = e.Course.ThumbnailURL,
                EnrollmentDate = e.EnrollmentDate,
                CompletionStatus = e.CompletionStatus
            }).ToList();
            return Ok(enrolledCourses);
        }



        // Upload profile picture
        [HttpPost("me/profilePicture")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            try
            {
                var fileName = await _fileService.SaveFileAsync(file, _limits.ProfileImageMaxSize);
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                    return Unauthorized();
                var user = await _userManagementRepository.GetByIdAsync(userId);
                await _userManagementRepository.UpdateProfilePictureAsync(user.UserID, fileName);


                return Ok(new { FileName = fileName, Message = "Profile image uploaded successfully." + fileName });



            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        #region Admin Functionalities

        // Get all users
        [HttpGet("admin/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManagementRepository.GetAllUsersAsync();
            var userDtos = users.Select(user => new UserProfileDto
            {
                UserId = user.UserID,
                Name = user.Name,
                Email = user.Email,
                Bio = user.Bio,
                ProfilePicture = user.ProfilePicture,
                LastLogin = user.LastLogin,
                Role = (int)user.Role,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted
            }).ToList();
            return Ok(userDtos);
        }

        [HttpGet("admin/users/role/{role}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            if (!Enum.TryParse<UserRole>(role, true, out var userRole))
            {
                return BadRequest(new { Message = "Invalid role specified" });
            }
            var users = await _userManagementRepository.GetUsersByRoleAsync(userRole);
            var userDtos = users.Select(user => new UserProfileDto
            {
                UserId = user.UserID,
                Name = user.Name,
                Email = user.Email,
                Bio = user.Bio,
                ProfilePicture = user.ProfilePicture,
                Role = (int)user.Role,
                LastLogin = user.LastLogin,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive,
                IsDeleted = user.IsDeleted
            }).ToList();
            return Ok(userDtos);
        }

        [HttpPost("admin/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto newUser)
        {
            var existingUser = await _userManagementRepository.GetUserByEmailAsync(newUser.Email);
            if (existingUser != null)
            {
                return Conflict(new { Message = "Email already in use" });
            }
            var created = await _userManagementRepository.CreateUserAsync(newUser);
            if (!created)
            {
                return StatusCode(500, new { Message = "Error creating user" });
            }
            return Ok(new { Message = "User created successfully" });
        }

        [HttpPut("admin/users/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusDto userStatusDto)
        {
            var user = await _userManagementRepository.GetUserByEmailAsync(userStatusDto.Email);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }
            user.IsActive = userStatusDto.IsActive;
            var updated = await _userManagementRepository.UpdateUserStatusAsync(user.Email, user.IsActive);
            if (!updated)
            {
                return StatusCode(500, new { Message = "Error updating user status" });
            }
            return Ok(new { Message = "User status updated successfully" });
        }

        [HttpDelete("admin/users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserDto deleteUserDto)
        {
            var user = await _userManagementRepository.GetUserByEmailAsync(deleteUserDto.Email);
            if (user == null)
            {
                return NotFound(new { Message = "User not found" });
            }
            var deleted = await _userManagementRepository.DeleteUserAsync(user.Email);
            if (!deleted)
            {
                return StatusCode(500, new { Message = "Error deleting user" });
            }
            return Ok(new { Message = "User deleted successfully" });
        }

        #endregion

        #region Instructor Functionalities

        //[HttpGet("instructor/courses/count")]
        //[Authorize(Roles = "Instructor")]
        //public async Task<IActionResult> GetTotalCoursesByInstructor()
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (!int.TryParse(userIdClaim, out int instructorId))
        //        return Unauthorized();
        //    var totalCourses = await _userManagementRepository.GetTotalCoursesByInstructorAsync(instructorId);
        //    return Ok(new { TotalCourses = totalCourses });
        //}

        //[HttpGet("instructor/students/count")]
        //[Authorize(Roles = "Instructor")]
        //public async Task<IActionResult> GetTotalStudentsByInstructor()
        //{
        //    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (!int.TryParse(userIdClaim, out int instructorId))
        //        return Unauthorized();
        //    var totalStudents = await _userManagementRepository.GetTotalStudentsByInstructorAsync(instructorId);
        //    return Ok(new { TotalStudents = totalStudents });
        //}

        [HttpGet("instructor/analytics")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetInstructorAnalytics()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            // 🚨 4. CALL THE REPOSITORY TO GET ALL METRICS
            // The repository performs the logic for Total Students, Courses, and Videos.
            var metrics = await _userManagementRepository.GetInstructorStatisticsAsync(userId);

            if (metrics == null)
            {
                // Fallback returns 0 if no metrics are found
                return Ok(new { totalStudents = 0, totalCourses = 0, totalVideos = 0 });
            }

            // 🚨 5. RETURN AN ANONYMOUS OBJECT WITH CAMELCASE PROPERTIES
            return Ok(new
            {
                totalStudents = metrics.TotalStudents,
                totalCourses = metrics.TotalCourses,
                totalVideos = metrics.TotalVideos
            });

            #endregion


        }
    }
}
