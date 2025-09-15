using LMS.Domain.Models;
using LMS.Infrastructure.DTO;
using LMS.Infrastructure.Repository.Interfaces;
using LMS.Infrastructure.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Services
{
    public class CourseCategoryService : ICourseCategoryService
    {
        private readonly ICourseCategoryRepository categoryRepository;

        public CourseCategoryService(ICourseCategoryRepository categoryRepository)
        {
            this.categoryRepository = categoryRepository;
        }

        public async Task<CourseCategory> CreateCategoryAsync(CreateCategoryDto categoryDto)
        {
            var existingCategory = await categoryRepository.GetCategoryByNameAsync(categoryDto.Name);
            if (existingCategory != null)
            {
                throw new InvalidOperationException("Category with the same name already exists.");
            }
            var category = new CourseCategory
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };
            return await categoryRepository.CreateCategoryAsync(category);
        }
        public async Task<CourseCategory?> GetCategoryByIdAsync(int id)
        {
            return await categoryRepository.GetCategoryByIdAsync(id);
        }
        public async Task<List<CourseCategory>> GetAllCategoriesAsync()
        {
            return await categoryRepository.GetAllCategoriesAsync();
        }
        public async Task<CourseCategory?> UpdateCategoryAsync(int id, CreateCategoryDto categoryDto)
        {
            var existingCategory = await categoryRepository.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                return null;
            }
            var categoryWithSameName = await categoryRepository.GetCategoryByNameAsync(categoryDto.Name);
            if (categoryWithSameName != null && categoryWithSameName.CategoryID != id)
            {
                throw new InvalidOperationException("Category with the same name already exists.");
            }
            existingCategory.Name = categoryDto.Name;
            existingCategory.Description = categoryDto.Description;
            return await categoryRepository.UpdateCategoryAsync(existingCategory);
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var existingCategory = await categoryRepository.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                return false;
            }
            await categoryRepository.DeleteCategoryAsync(id);
            return true;
        }
    }
}
