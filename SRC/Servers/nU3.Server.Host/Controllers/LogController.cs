using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace nU3.Server.Host.Controllers
{
    /// <summary>
    /// 클라이언트에서 전송된 로그 파일을 수집하고 서버에 저장하는 컨트롤러입니다.
    /// 
    /// 주요 책임:
    /// - 클라이언트로부터 업로드된 로그 파일(.log, .txt 등) 및 압축 파일(.gz)을 수신
    /// - 압축 파일은 해제하여 저장하며, 비압축 파일은 그대로 저장
    /// - 업로드 이벤트를 서버 로그에 기록하고 별도의 업로드 일지 파일에 기록
    /// 
    /// 보안 및 운영 주의사항:
    /// - 입력 파일명에 대한 검증/정규화가 필요합니다(현재는 기본 파일명만 사용).
    /// - 업로드 파일 크기 제한은 서버 설정(IIS/Kestrel/Reverse Proxy)에서 제어하세요.
    /// - 민감한 로그는 별도 암호화/권한 관리가 필요합니다.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LogController : ControllerBase
    {
        // 로그 파일을 저장할 기본 경로 (공용 애플리케이션 데이터 아래)
        private readonly string _logStoragePath;
        private readonly ILogger<LogController> _logger;

        /// <summary>
        /// 생성자: 서버의 로그 저장 경로를 초기화합니다.
        /// 기본 경로: %CommonApplicationData%\nU3.Framework\ServerLogs\ClientLogs
        /// </summary>
        public LogController(ILogger<LogController> logger)
        {
            _logger = logger;
            _logStoragePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "nU3.Framework",
                "ServerLogs",
                "ClientLogs"
            );

            // 저장 디렉토리가 없으면 생성합니다.
            if (!Directory.Exists(_logStoragePath))
            {
                Directory.CreateDirectory(_logStoragePath);
                _logger.LogInformation("로그 저장 경로 생성: {Path}", _logStoragePath);
            }
            else
            {
                _logger.LogDebug("로그 저장 경로: {Path}", _logStoragePath);
            }
        }

        /// <summary>
        /// 클라이언트 로그 파일 업로드 처리 엔드포인트
        /// - 업로드는 multipart/form-data의 파일 필드로 전달됩니다.
        /// - .gz 파일은 압축 해제 후 저장됩니다.
        /// - 업로드 성공 시 업로드 결과와 저장된 파일 정보를 반환합니다.
        /// </summary>
        [HttpPost("upload")]
        public async Task<IActionResult> UploadLog([FromForm] LogUploadModel model)
        {
            try
            {
                if (model.File == null || model.File.Length == 0)
                {
                    _logger.LogWarning("업로드 요청에 파일이 포함되어 있지 않음");
                    return BadRequest("파일이 제공되지 않았습니다.");
                }

                var uploadedFileName = Path.GetFileName(model.File.FileName);
                var originalSize = model.File.Length;
                var isCompressed = uploadedFileName.EndsWith(".gz", StringComparison.OrdinalIgnoreCase);

                _logger.LogInformation("로그 파일 수신: {FileName} ({Size} 바이트, 압축: {IsCompressed})", 
                    uploadedFileName, originalSize, isCompressed);

                string finalFileName;
                long finalSize;

                if (isCompressed)
                {
                    // 압축 해제 후 저장
                    finalFileName = Path.GetFileNameWithoutExtension(uploadedFileName);
                    var filePath = GetUniqueFilePath(finalFileName);

                    using (var compressedStream = model.File.OpenReadStream())
                    using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    using (var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await gzipStream.CopyToAsync(outputStream);
                        finalSize = outputStream.Length;
                    }

                    // 압축 비율 로그 (finalSize가 0이면 계산하지 않음)
                    if (finalSize > 0)
                    {
                        var compressionRatio = (1 - (double)originalSize / finalSize) * 100;
                        _logger.LogInformation("압축 해제 완료: {CompressedSize} -> {DecompressedSize} ({Ratio:F1}% 압축)", 
                            FormatFileSize(originalSize), FormatFileSize(finalSize), compressionRatio);
                    }
                    else
                    {
                        _logger.LogWarning("압축 해제 후 파일 크기가 0입니다: {File}", finalFileName);
                    }
                }
                else
                {
                    // 비압축 파일은 그대로 저장
                    finalFileName = uploadedFileName;
                    var filePath = GetUniqueFilePath(finalFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await model.File.CopyToAsync(stream);
                        finalSize = stream.Length;
                    }
                }

                // 업로드 기록을 서버 로그 파일에 남김
                await LogUploadRecord(finalFileName, originalSize, finalSize, isCompressed);

                _logger.LogInformation("로그 업로드 처리 완료: {File} (저장크기: {Size})", finalFileName, finalSize);

                return Ok(new 
                { 
                    success = true, 
                    fileName = finalFileName,
                    uploadedSize = originalSize,
                    storedSize = finalSize,
                    compressed = isCompressed,
                    message = "로그 파일 업로드 성공"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "로그 파일 업로드 중 오류 발생");
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = $"로그 업로드 오류: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// 감사지 로그 업로드 처리 엔드포인트
        /// - audit 로그는 별도 디렉토리에 저장됩니다.
        /// </summary>
        [HttpPost("upload-audit")]
        public async Task<IActionResult> UploadAuditLog([FromForm] LogUploadModel model)
        {
            try
            {
                if (model.File == null || model.File.Length == 0)
                {
                    _logger.LogWarning("감사지 업로드 요청에 파일이 없음");
                    return BadRequest("파일이 제공되지 않았습니다.");
                }

                var auditStoragePath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "nU3.Framework",
                    "ServerLogs",
                    "ClientAudits"
                );

                if (!Directory.Exists(auditStoragePath))
                {
                    Directory.CreateDirectory(auditStoragePath);
                    _logger.LogInformation("감사지 로그 저장 경로 생성: {Path}", auditStoragePath);
                }

                var uploadedFileName = Path.GetFileName(model.File.FileName);
                var originalSize = model.File.Length;
                var isCompressed = uploadedFileName.EndsWith(".gz", StringComparison.OrdinalIgnoreCase);

                _logger.LogInformation("감사지 로그 수신: {FileName} ({Size} 바이트, 압축: {IsCompressed})", 
                    uploadedFileName, originalSize, isCompressed);

                string finalFileName;
                long finalSize;

                if (isCompressed)
                {
                    finalFileName = Path.GetFileNameWithoutExtension(uploadedFileName);
                    var filePath = GetUniqueFilePath(finalFileName, auditStoragePath);

                    using (var compressedStream = model.File.OpenReadStream())
                    using (var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    using (var outputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await gzipStream.CopyToAsync(outputStream);
                        finalSize = outputStream.Length;
                    }
                }
                else
                {
                    finalFileName = uploadedFileName;
                    var filePath = GetUniqueFilePath(finalFileName, auditStoragePath);

                    using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await model.File.CopyToAsync(stream);
                        finalSize = stream.Length;
                    }
                }

                _logger.LogInformation("감사지 로그 업로드 완료: {File} (저장크기: {Size})", finalFileName, finalSize);

                return Ok(new 
                { 
                    success = true, 
                    fileName = finalFileName,
                    uploadedSize = originalSize,
                    storedSize = finalSize,
                    compressed = isCompressed,
                    message = "감사지 로그 업로드 성공"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "감사지 로그 업로드 중 오류 발생");
                return StatusCode(500, new 
                { 
                    success = false, 
                    message = $"감사지 로그 업로드 오류: {ex.Message}" 
                });
            }
        }

        /// <summary>
        /// 서버에 저장된 로그 파일의 요약 정보를 반환합니다(파일 수, 총 크기 등).
        /// </summary>
        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            try
            {
                var logFiles = Directory.GetFiles(_logStoragePath, "*.log");
                var totalSize = logFiles.Sum(f => new FileInfo(f).Length);

                return Ok(new
                {
                    success = true,
                    logPath = _logStoragePath,
                    fileCount = logFiles.Length,
                    totalSize = totalSize,
                    totalSizeMB = Math.Round(totalSize / 1024.0 / 1024.0, 2)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "로그 정보 조회 중 오류 발생");
                return StatusCode(500, new
                {
                    success = false,
                    message = $"로그 정보 조회 오류: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// 동일한 파일명이 존재할 경우 충돌을 피하기 위해 고유 파일 경로를 생성합니다.
        /// 타임스탬프를 파일명에 추가하여 중복을 방지합니다.
        /// </summary>
        private string GetUniqueFilePath(string fileName, string? basePath = null)
        {
            basePath ??= _logStoragePath;
            var filePath = Path.Combine(basePath, fileName);

            if (System.IO.File.Exists(filePath))
            {
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                var ext = Path.GetExtension(fileName);
                var timestamp = DateTime.Now.ToString("HHmmss");
                fileName = $"{fileNameWithoutExt}_{timestamp}{ext}";
                filePath = Path.Combine(basePath, fileName);
            }

            return filePath;
        }

        /// <summary>
        /// 서버 측 업로드 기록을 로그 파일에 남깁니다 (일일 로그 파일로 분리).
        /// </summary>
        private async Task LogUploadRecord(string fileName, long uploadedSize, long storedSize, bool compressed)
        {
            try
            {
                var serverLog = Path.Combine(_logStoragePath, $"_UploadLog_{DateTime.Now:yyyyMMdd}.log");
                var compressionInfo = compressed ? $" (압축: {FormatFileSize(uploadedSize)} -> {FormatFileSize(storedSize)})" : "";
                await System.IO.File.AppendAllTextAsync(serverLog, 
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] 업로드: {fileName} ({FormatFileSize(storedSize)}){compressionInfo}\n");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "업로드 기록 로그 작성 실패");
            }
        }

        /// <summary>
        /// 바이트 단위 파일 크기를 사람이 읽기 쉬운 문자열로 변환합니다.
        /// </summary>
        private string FormatFileSize(long bytes)
        {
            if (bytes < 1024)
                return $"{bytes} B";
            else if (bytes < 1024 * 1024)
                return $"{bytes / 1024.0:F2} KB";
            else
                return $"{bytes / (1024.0 * 1024.0):F2} MB";
        }
    }

    /// <summary>
    /// 업로드 모델
    /// </summary>
    public class LogUploadModel
    {
        /// <summary>
        /// 업로드할 로그 파일 (압축 가능)
        /// </summary>
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
