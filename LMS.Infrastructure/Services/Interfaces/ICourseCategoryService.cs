using LMS.Domain.Models;
using LMS.Infrastructure.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Services.Interfaces
{
    public interface ICourseCategoryService
    {
        Task<CourseCategory> CreateCategoryAsync(CreateCourseCategoryDto createCourseCategoryDto);
        Task<CourseCategory?> GetCategoryByIdAsync(int id);
        Task<List<CourseCategory>> GetAllCategoriesAsync();
        Task<CourseCategory?> UpdateCategoryAsync(int id, CourseCategoryDto courseCategoryDto);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
