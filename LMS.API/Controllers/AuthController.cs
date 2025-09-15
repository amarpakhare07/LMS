using Infrastructure.DTO;
using LMS.Domain;
using LMS.Domain.Enums;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        public readonly IUserManagementRepository _userManagementRepository;
        private readonly IEmailSenderRepository _email;


        public AuthController(JwtService jwtService,IUserManagementRepository userManagementRepository)
        {
            _jwtService = jwtService;
            _userManagementRepository = userManagementRepository;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginRequest)
        {
            // This is just a placeholder. In a real application, you would validate the user credentials.
            var response = await _jwtService.Authenticate(loginRequest);
            
            if (response == null)
                return Unauthorized(new { Message = "Invalid email or password" });

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerRequest)
        {
            if (registerRequest == null)
            {
                return BadRequest(new { Message = "Invalid user data" });
            }
            var user = await _userManagementRepository.RegisterUserAsync(registerRequest);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "User registration failed" });
            }
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("register/instructor")]
        public async Task<IActionResult> Register([FromBody] RegisterInstructorDto registerInstructorDto)
        {
            if (registerInstructorDto == null)
            {
                return BadRequest(new { Message = "Invalid user data" });
            }
            var user = await _userManagementRepository.RegisterUserAsync(registerInstructorDto);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "User registration failed" });
            }
            return Ok(user);
        }

        [AllowAnonymous]
        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            if (email == null || string.IsNullOrEmpty(email))
            {
                return BadRequest("Email is required.");
            }

            var user = await _userManagementRepository.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound();
            }

            user.ResetToken = Guid.NewGuid().ToString();
            user.ResetTokenExpiry = DateTime.UtcNow.AddMinutes(10);
            await _userManagementRepository.UpdateUserOnlyAsync(user);

            await _email.SendResetLinkAsync(user.Email, user.ResetToken);

            return Ok(
                new
                {
                    message = "Reset link sent",
                    token = user.ResetToken,
                    email = user.Email
                }
            );
        }

        [HttpPost("resetPassword")]
        [Authorize(Roles = "Student,Instructor")]
        public async Task<IActionResult> Reset([FromBody] PasswordChangeDto data)
        {
            var user = await _userManagementRepository.FindByResetTokenAsync(data.token);

            if (user == null || user.ResetTokenExpiry < DateTime.UtcNow)
            {
                return BadRequest("Invalid or Expired token");
            }

            var passwordHasher = new PasswordHasher<PasswordChangeDto>();
            user.PasswordHash = passwordHasher.HashPassword(data, data.password);


            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            await _userManagementRepository.UpdateUserOnlyAsync(user);


            return Ok("Password updated");
        }
    }
}
