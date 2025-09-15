using LMS.Domain;
using LMS.Domain.Enums;
using LMS.Domain.Models;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LMS.Infrastructure.Repository
{
    public class UserManagementRepository : IUserManagementRepository
    {
        private readonly LmsDbContext _dbContext;
        private readonly PasswordHashing _passwordHashing;
        public UserManagementRepository(LmsDbContext context, PasswordHashing passwordHashing)
        {
            _dbContext = context;
            _passwordHashing = passwordHashing;
        }

        #region Admin
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }
        public async Task<List<User>> GetUsersByRoleAsync(UserRole role)
        {
            return await _dbContext.Users.Where(u => u.Role == role).ToListAsync();
        }
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return _dbContext.Users.FirstOrDefault(u => u.Email == email);
        }
        public async Task<bool> CreateUserAsync(CreateUserDto user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            var hashedPassword = _passwordHashing.HashPassword(user.Password);
            var newUser = new User
            {
                Name = user.Name,
                Email = user.Email,
                PasswordHash = hashedPassword,
                Role = UserRole.Student, // Default role
                IsActive = true // Default status
            };
            _dbContext.Users.Add(newUser);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        public Task<bool> UpdateUserStatusAsync(User user)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteUserAsync(string email)
        {
            throw new NotImplementedException();
        }
        #endregion


        public async Task<RegisterDto> RegisterUserAsync(RegisterDto registerDto)
        {
            //if (registerDto == null)
            //{
            //    throw new ArgumentNullException(nameof(registerDto), "Register data cannot be null");
            //}
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

        public Task<User?> FindByEmailAsync(string email)
        {
            return _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> UpdateUserOnlyAsync(User user)
        {
            _dbContext.Users.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        public Task<User?> FindByResetTokenAsync(string email)
        {
            throw new NotImplementedException();
        }



        // Change the method signature to accept int instead of Guid
        public async Task<User> GetByIdAsync(int userId)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserID == userId);
        }

        public async Task<bool> UpdateBioAsync(int UserId, string newBio)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserID == UserId);
            if (user == null)
            {
                return false;
            }
            else
            {
                user.Bio = newBio;
                user.UpdatedAt = DateTime.UtcNow;
                await _dbContext.SaveChangesAsync();
                return true;
            }

        }


        // delete-->soft delete --> only flag change
        public async Task<bool> DeleteUserasync(int userId)
        {
            // first find the user if not then error
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.UserID == userId);
            if(user == null)
            {
                return false;
            }
            // if found the change the flag
            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;

            // then update the db
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return true;



        }

        public async Task<List<Enrollment>> GetUserEnrolledCourses(int userId)
        {
            return await _dbContext.Enrollments
                .Where(e => e.UserID == userId && !e.IsDeleted)
                .Include(e => e.Course)
                .ToListAsync();
        }

    }
}
