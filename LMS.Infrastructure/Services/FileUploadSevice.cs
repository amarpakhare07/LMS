using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using LMS.Infrastructure.Services.Interfaces;

namespace LMS.Infrastructure.Services
{
    public class FileUploadService : IFileUploadService
    {
        private readonly string _uploadPath;

        public FileUploadService()
        {
            // Navigate up from bin/Debug to reach the solution root
            var solutionRoot = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName;

            // Point to LMS.API/Uploads folder
            _uploadPath = Path.Combine(solutionRoot, "LMS.API", "Uploads");

            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);
        }

        public async Task<string> SaveFileAsync(IFormFile file, long maxSizeBytes)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided.");

            if (file.Length > maxSizeBytes)
                throw new ArgumentException($"File size exceeds the limit of {maxSizeBytes / (1024 * 1024)} MB.");

            string filePath = Path.Combine(_uploadPath, file.FileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return file.FileName;
        }

        public byte[] GetFile(string fileName)
        {
            var filePath = Path.Combine(_uploadPath, fileName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException("File not found.");

            return File.ReadAllBytes(filePath);
        }
    }
}
