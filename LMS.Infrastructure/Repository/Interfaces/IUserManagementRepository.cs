using LMS.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Repository.Interfaces
{
    public interface IUserManagementRepository
    {
        Task<User> GetByIdAsync(int UserId);

        Task<bool> UpdateBioAsync(int UserId, string newBio);

        Task<bool> DeleteUserasync(int UserId);

        Task<List<Enrollment>> GetUserEnrolledCourses(int UserId);
    }
}
