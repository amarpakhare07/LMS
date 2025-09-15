using LMS.Domain.Models;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

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
        [Authorize(Roles ="Admin")]
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
        [Authorize(Roles ="Student")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file)
        {
            try
            {
                var fileName = await _fileService.SaveFileAsync(file, _limits.ProfileImageMaxSize);
                return Ok(new { FileName = fileName, Message = "Profile image uploaded successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


    }
}
