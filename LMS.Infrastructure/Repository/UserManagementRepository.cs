using LMS.Domain;
using LMS.Domain.Models;
using LMS.Infrastructure.Repository.Interfaces;
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
        private readonly LmsDbContext _dbcontext;
        public UserManagementRepository(LmsDbContext context)
        {
            _dbcontext = context;
        }

        

        // Change the method signature to accept int instead of Guid
        public async Task<User> GetByIdAsync(int userId)
        {
            return await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserID == userId);
        }

        public async Task<bool> UpdateBioAsync(int UserId, string newBio)
        {
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserID == UserId);
            if (user == null)
            {
                return false;
            }
            else
            {
                user.Bio = newBio;
                user.UpdatedAt = DateTime.UtcNow;
                await _dbcontext.SaveChangesAsync();
                return true;
            }

        }


        // delete-->soft delete --> only flag change
        public async Task<bool> DeleteUserasync(int userId)
        {
            // first find the user if not then error
            var user = await _dbcontext.Users.FirstOrDefaultAsync(u => u.UserID == userId);
            if(user == null)
            {
                return false;
            }
            // if found the change the flag
            user.IsDeleted = true;
            user.UpdatedAt = DateTime.UtcNow;

            // then update the db
            _dbcontext.Users.Update(user);
            await _dbcontext.SaveChangesAsync();
            return true;



        }

        public async Task<List<Enrollment>> GetUserEnrolledCourses(int userId)
        {
            return await _dbcontext.Enrollments
                .Where(e => e.UserID == userId && !e.IsDeleted)
                .Include(e => e.Course)
                .ToListAsync();
        }

    }
}
