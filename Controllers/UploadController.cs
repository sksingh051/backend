using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Phase_07_Poc_01.Controllers
{
    public class UploadImageRequest
    {
        public IFormFile File { get; set; }
    }

    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IWebHostEnvironment environment, ILogger<UploadController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpPost, Route(Phase_07_Poc_01.Static.ApiRoutes.Upload.UploadBase)]
        [Authorize(Roles = Phase_07_Poc_01.Static.Roles.Admin)]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
        {
            var file = request.File;
            _logger.LogInformation("Received file upload request");

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("File upload failed: No file uploaded or file is empty");
                return BadRequest("No file uploaded");
            }

            _logger.LogInformation("Uploading file: {FileName}, Size: {FileSize} bytes", file.FileName, file.Length);

            var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var fileUrl = $"/uploads/{uniqueFileName}";
            
            _logger.LogInformation("File uploaded successfully: {FileUrl}", fileUrl);
            return Ok(new { Url = fileUrl });
        }
    }
}
