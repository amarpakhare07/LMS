using LMS.Infrastructure.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

        public UserManagementController(IUserManagementRepository userManagementRepository)
        {
            _userManagementRepository = userManagementRepository;  // This assigns the injected repository userRepo to the private field _userRepo
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
        [HttpPut("bio")]
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
    }
}
