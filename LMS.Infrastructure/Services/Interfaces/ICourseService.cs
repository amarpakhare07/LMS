using LMS.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Services.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto);
        Task<CourseDto> GetCourseByIdAsync(int id);
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<bool> UpdateCourseAsync(CourseDto courseDto);
        Task<bool> UpdateCourseStatusAsync(int courseId, UpdateCourseStatusDto updateCourseStatusDto);
        Task<bool> DeleteCourseAsync(int id);
    }
}
