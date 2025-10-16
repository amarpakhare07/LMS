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
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository courseRepository;

        public CourseService(ICourseRepository courseRepository)
        {
            this.courseRepository = courseRepository;
        }

        public async Task<CourseDto> CreateCourseAsync(CreateCourseDto createCourseDto)
        {
            var course = new Course
            {
                Title = createCourseDto.Title,
                Description = createCourseDto.Description,
                Syllabus = createCourseDto.Syllabus,
                Level = createCourseDto.Level,
                Language = createCourseDto.Language,
                Duration = createCourseDto.Duration,
                ThumbnailURL = createCourseDto.ThumbnailURL,
                CategoryID = createCourseDto.CategoryID,
                Published = createCourseDto.Published
            };

            var addedCourse = await courseRepository.AddCourseAsync(course);
            return new CourseDto
            {
                CourseID = addedCourse.CourseID,
                Title = addedCourse.Title,
                Description = addedCourse.Description,
                Syllabus = addedCourse.Syllabus,
                Level = addedCourse.Level,
                Language = addedCourse.Language,
                Duration = addedCourse.Duration,
                ThumbnailURL = addedCourse.ThumbnailURL,
                CategoryID = addedCourse.CategoryID,
                Published = addedCourse.Published,
                Rating = addedCourse.Rating,
                ReviewCount = addedCourse.ReviewCount
            };
        }

        public async Task<CourseDto?> GetCourseByIdAsync(int id)
        {
            var course = await courseRepository.GetCourseByIdAsync(id);
            if (course == null) return null;
            return new CourseDto
            {
                CourseID = course.CourseID,
                Title = course.Title,
                Description = course.Description,
                Syllabus = course.Syllabus,
                Level = course.Level,
                Language = course.Language,
                Duration = course.Duration,
                ThumbnailURL = course.ThumbnailURL,
                CategoryID = course.CategoryID,
                Published = course.Published,
                Rating = course.Rating,
                ReviewCount = course.ReviewCount
            };
        }

        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            var courses = await courseRepository.GetAllCoursesAsync();
            var courseDtos = new List<CourseDto>();
            foreach (var course in courses) 
            {
                courseDtos.Add(new CourseDto
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Description = course.Description,
                    Syllabus = course.Syllabus,
                    Level = course.Level,
                    Language = course.Language,
                    Duration = course.Duration,
                    ThumbnailURL = course.ThumbnailURL,
                    CategoryID = course.CategoryID,
                    Published = course.Published,
                    Rating = course.Rating,
                    ReviewCount = course.ReviewCount
                });
            }
            return courseDtos;
        }

        public async Task<bool> UpdateCourseAsync(CourseDto courseDto)
        {
            var course = await courseRepository.GetCourseByIdAsync(courseDto.CourseID);
            if (course == null) return false;

            course.Title = courseDto.Title;
            course.Description = courseDto.Description;
            course.Syllabus = courseDto.Syllabus;
            course.Level = courseDto.Level;
            course.Language = courseDto.Language;
            course.Duration = courseDto.Duration;
            course.ThumbnailURL = courseDto.ThumbnailURL;
            course.CategoryID = courseDto.CategoryID;
            course.Published = courseDto.Published;

            return await courseRepository.UpdateCourseAsync(course);
        }

        public async Task<bool> DeleteCourseAsync(int id)
        {
            return await courseRepository.DeleteCourseAsync(id);
        }

        public Task<bool> UpdateCourseStatusAsync(int courseId, UpdateCourseStatusDto updateCourseStatusDto)
        {
            var course = courseRepository.GetCourseByIdAsync(courseId).Result;
            if (course == null) return Task.FromResult(false);

            course.Published = updateCourseStatusDto.Published;

            return courseRepository.UpdateCourseStatusAsync(course);
        }
    }
}
