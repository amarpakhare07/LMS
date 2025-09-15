using LMS.Domain;
using LMS.Infrastructure.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace LMS.Infrastructure.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        private readonly LmsDbContext _dbContext;
        private readonly PasswordHashing _passwordHashing;
        public JwtService(IConfiguration configuration, LmsDbContext dbContext, PasswordHashing passwordHashing)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _passwordHashing = passwordHashing;
        }

        public async Task<LoginResponseModelDto> Authenticate(LoginDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return null;

            }
            var user = await _dbContext.Users.FirstOrDefaultAsync(user => user.Email == request.Email);
            if (user == null)
            {
                return null;
            }

            bool passcheck;
            if (request.Email == "admin@cognizant.com")
            {
                passcheck = request.Password == user.PasswordHash;
            }
            else
            {
                passcheck = _passwordHashing.VerifyPassword(user.PasswordHash, request.Password);
            }

            if (!passcheck)
            {
                return null;
            }

            // Adding the login actice stamp
            user.IsActive = true;

            // Inserting the last login time
            user.LastLogin = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();


            var issuer = _configuration["JwtConfig:Issuer"];
            var audience = _configuration["JwtConfig:Audience"];
            var key = _configuration["JwtConfig:Key"];
            var tokenValidityMins = _configuration.GetValue<int>("JwtConfig:TokenValidityMins");
            var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(tokenValidityMins);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                     new Claim(JwtRegisteredClaimNames.Name, request.Email),
                     new Claim(ClaimTypes.Role, user.Role.ToString()),
                     new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())                 }
                ),
                Expires = tokenExpiryTimeStamp,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                SecurityAlgorithms.HmacSha512Signature),
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(securityToken);
            return new LoginResponseModelDto
            {
                AccessToken = accessToken,
                EmailPasswordCredential = request.Email,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds,
                Role = user.Role.ToString(),
                UserId = user.UserID,
                Email = user.Email
            };
        }

    }
}
