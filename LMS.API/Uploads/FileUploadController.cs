using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUpload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileUploadController : ControllerBase
    {
        private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");

        public FileUploadController()
        {
            if (!Directory.Exists(_uploadPath))
                Directory.CreateDirectory(_uploadPath);
        }

        
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            //Change the File Name == > Save the Input File with a different name
            string filePath = Path.Combine(_uploadPath, file.FileName);

            //If you want the file to be convererted to BLOB and store in DB 
            //DB Sotring Code.. .
          

            //If you want the file to be stored in Uploads Folder Only
            //Follow the the option given below

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { FileName = file.FileName, Message = "File uploaded successfully." });
        }

        
        [HttpGet("display/{fileName}")]
        public IActionResult DisplayFile(string fileName)
        {
            var filePath = Path.Combine(_uploadPath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("File not found.");

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var contentType = "application/octet-stream"; // You can customize based on extension

            return File(fileBytes, contentType, fileName);
        }
    }
}
