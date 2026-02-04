using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nU3.Connectivity;
using nU3.Server.Connectivity.Services;
using System.ComponentModel.DataAnnotations;

namespace nU3.Server.Host.Controllers.Connectivity
{
    [ApiController]
    [Route("api/v1/files")]
    public class FileTransferController : ControllerBase
    {
        private readonly ServerFileTransferService _fileService;
        private readonly ILogger<FileTransferController> _logger;

        public FileTransferController(ServerFileTransferService fileService, ILogger<FileTransferController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        [HttpGet("directory")]
        public IActionResult GetHomeDirectory()
        {
            _logger.LogInformation("API 호출: GetHomeDirectory"); // API Call: GetHomeDirectory
            return Ok(_fileService.GetHomeDirectory());
        }

        [HttpPost("directory/config")]
        public IActionResult SetHomeDirectory([FromBody] DirectoryConfigDto config)
        {
            _logger.LogInformation("API 호출: SetHomeDirectory (UseHome: {IsUseHomePath}, Path: {ServerHomePath})", config.IsUseHomePath, config.ServerHomePath); // API Call: SetHomeDirectory
            var result = _fileService.SetHomeDirectory(config.IsUseHomePath, config.ServerHomePath);
            return Ok(result);
        }

        [HttpGet("list")]
        public IActionResult GetFileList([FromQuery] string path)
        {
            _logger.LogInformation("API 호출: GetFileList (Path: {Path})", path); // API Call: GetFileList
            return Ok(_fileService.GetFileList(path));
        }

        [HttpGet("subdirectories")]
        public IActionResult GetSubDirectoryList([FromQuery] string path)
        {
            _logger.LogInformation("API 호출: GetSubDirectoryList (Path: {Path})", path); // API Call: GetSubDirectoryList
            return Ok(_fileService.GetSubDirectoryList(path));
        }

        [HttpPost("directory/create")]
        public IActionResult CreateDirectory([FromQuery] string path)
        {
            _logger.LogInformation("API 호출: CreateDirectory (Path: {Path})", path); // API Call: CreateDirectory
            return Ok(_fileService.CreateDirectory(path));
        }

        [HttpPost("directory/rename")]
        public IActionResult RenameDirectory([FromBody] RenameDirectoryDto request)
        {
            _logger.LogInformation("API 호출: RenameDirectory ({Source} -> {Dest})", request.SourcePath, request.DestPath); // API Call: RenameDirectory
            return Ok(_fileService.RenameDirectory(request.SourcePath, request.DestPath));
        }

        [HttpGet("directory/exists")]
        public IActionResult ExistDirectory([FromQuery] string path)
        {
            _logger.LogInformation("API 호출: ExistDirectory (Path: {Path})", path); // API Call: ExistDirectory
            return Ok(_fileService.ExistDirectory(path));
        }

        [HttpDelete("directory")]
        public IActionResult DeleteDirectory([FromQuery] string path)
        {
            _logger.LogInformation("API 호출: DeleteDirectory (Path: {Path})", path); // API Call: DeleteDirectory
            return Ok(_fileService.DeleteDirectory(path));
        }

        // ============================
        // File Operations (Stream)
        // ============================

        [HttpPost("upload")]
        [DisableRequestSizeLimit] // Allow large file uploads
        public async Task<IActionResult> UploadFile([FromForm] FileUploadModel model)
        {
            if (model.File == null || model.File.Length == 0)
                return BadRequest("파일이 업로드되지 않았습니다."); // No file uploaded.

            _logger.LogInformation("API 호출: UploadFile (ServerPath: {ServerPath}, Size: {Size})", model.ServerPath, model.File.Length); // API Call: UploadFile

            using (var memoryStream = new MemoryStream())
            {
                await model.File.CopyToAsync(memoryStream);
                var data = memoryStream.ToArray();
                // In a real high-perf scenario, pass stream directly instead of byte[]
                var result = await _fileService.SaveFileAsync(model.ServerPath, data);
                return result ? Ok() : StatusCode(500, "파일 저장 실패"); // Failed to save file.
            }
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadFile([FromQuery] string serverPath)
        {
            _logger.LogInformation("API 호출: DownloadFile (ServerPath: {ServerPath})", serverPath); // API Call: DownloadFile

            var data = await _fileService.ReadFileAsync(serverPath);
            if (data == null) return NotFound("파일을 찾을 수 없습니다."); // File not found.

            // Determine content type via extension (optional, defaulting to octet-stream)
            return File(data, "application/octet-stream", Path.GetFileName(serverPath));
        }

        [HttpGet("exists")]
        public IActionResult ExistFile([FromQuery] string path)
        {
            _logger.LogInformation("API 호출: ExistFile (Path: {Path})", path); // API Call: ExistFile
            return Ok(_fileService.ExistFile(path));
        }

        [HttpDelete]
        public IActionResult DeleteFile([FromQuery] string path)
        {
            _logger.LogInformation("API 호출: DeleteFile (Path: {Path})", path); // API Call: DeleteFile
            return Ok(_fileService.DeleteFile(path));
        }

        [HttpGet("size")]
        public IActionResult GetFileSize([FromQuery] string path)
        {
            _logger.LogInformation("API 호출: GetFileSize (Path: {Path})", path); // API Call: GetFileSize
            return Ok(_fileService.GetFileSize(path));
        }
    }

    public class DirectoryConfigDto
    {
        [Required]
        public bool IsUseHomePath { get; set; }
        
        [Required]
        public string ServerHomePath { get; set; } = string.Empty;
    }

    public class FileUploadModel
    {
        [Required]
        public IFormFile File { get; set; } = null!;  // Non-nullable for Swagger
        
        [Required]
        public string ServerPath { get; set; } = string.Empty;
    }

    public class RenameDirectoryDto
    {
        [Required]
        public string SourcePath { get; set; } = string.Empty;

        [Required]
        public string DestPath { get; set; } = string.Empty;
    }
}
