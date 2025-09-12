using LMS.Domain;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IRegisterUserRepository registerUserRepository;
        public AuthController(JwtService jwtService, IRegisterUserRepository registerUserRepository)
        {
            _jwtService = jwtService;
            this.registerUserRepository = registerUserRepository;
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
            var user = await registerUserRepository.RegisterUserAsync(registerRequest);
            if (user == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "User registration failed" });
            }
            return Ok(user);

        }
    }
}
