using LMS.Domain;
using LMS.Domain.Models;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repository
{
    public class RegisterUserRepository : IRegisterUserRepository
    {
        private readonly LmsDbContext _dbContext;
        private readonly PasswordHashing _passwordHashing;
        public RegisterUserRepository(LmsDbContext dbContext, PasswordHashing passwordHashing)
        {
            _dbContext = dbContext;
            _passwordHashing = passwordHashing;
        }

        public async Task<RegisterDto> RegisterUserAsync(RegisterDto registerDto)
        {
            if (registerDto == null)
            {
                throw new ArgumentNullException(nameof(registerDto), "Register data cannot be null");
            }
            // Here you would typically add code to save the user to a database.
            var userDto = new RegisterDto
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                Password = _passwordHashing.HashPassword(registerDto.Password)
            };
            User user = new User
            {
                Name = registerDto.Name,
                Email = registerDto.Email,
                PasswordHash = userDto.Password
            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return registerDto;
        }
    }
}
