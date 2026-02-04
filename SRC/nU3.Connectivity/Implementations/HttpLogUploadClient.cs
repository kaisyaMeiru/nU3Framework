using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace nU3.Connectivity.Implementations
{
    /// <summary>
    /// 로그 업로드 서비스를 HTTP/REST로 구현한 클라이언트입니다.
    /// 
    /// 책임:
    /// - 로컬에 저장된 애플리케이션 로그(.log)와 감사 로그(.log/.json)를 서버로 업로드
    /// - 업로드 시 파일 크기에 따라 GZip으로 압축하여 전송(옵션)
    /// - 업로드 후 로컬 파일 삭제 옵션 제공
    /// - 대기 중 로그 전체 업로드 및 현재 로그 즉시 업로드 기능
    /// 
    /// 사용 예:
    /// var client = new HttpLogUploadClient("https://server:5001");
    /// await client.UploadAllPendingLogsAsync();
    /// </summary>
    public class HttpLogUploadClient : ILogUploadService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _logDirectory;
        private readonly Action<string, string>? _logCallback;
        private readonly bool _enableCompression;
        private bool _autoUploadEnabled;
        private System.Threading.Timer? _autoUploadTimer;

        /// <summary>
        /// 기본 생성자: 서버 URL과 선택적 로그 디렉토리/콜백을 받아 클라이언트를 초기화합니다.
        /// </summary>
        /// <param name="baseUrl">서버 기본 URL (예: "https://localhost:64229"). 끝의 '/'는 내부에서 제거됩니다.</param>
        /// <param name="logDirectory">로컬 로그 디렉토리 경로(선택). null이면 AppData\nU3.Framework\LOG를 사용.</param>
        /// <param name="logCallback">로그 출력용 콜백(level, message). UI 또는 파일 로깅에 연결 가능.</param>
        /// <param name="enableCompression">업로드 시 Gzip 압축 사용 여부(기본값: true).</param>
        public HttpLogUploadClient(
            string baseUrl, 
            string? logDirectory = null,
            Action<string, string>? logCallback = null,
            bool enableCompression = true)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            _logCallback = logCallback;
            _enableCompression = enableCompression;
            
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl),
                // 네트워크 환경에 따라 응답 지연이 클 수 있으므로 비교적 긴 타임아웃을 설정
                Timeout = TimeSpan.FromMinutes(5)
            };

            // 기본 로그 경로: %AppData%\nU3.Framework\LOG
            _logDirectory = logDirectory ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "nU3.Framework",
                "LOG"
            );
        }

        /// <summary>
        /// 커스텀 HttpClient를 사용하는 생성자입니다. (테스트 또는 DI 시 사용)
        /// </summary>
        /// <param name="httpClient">외부에서 제공하는 HttpClient 인스턴스</param>
        /// <param name="baseUrl">서버 기본 URL</param>
        public HttpLogUploadClient(
            HttpClient httpClient, 
            string baseUrl,
            string? logDirectory = null,
            Action<string, string>? logCallback = null,
            bool enableCompression = true)
        {
            _httpClient = httpClient;
            _baseUrl = baseUrl.TrimEnd('/');
            _logCallback = logCallback;
            _enableCompression = enableCompression;
            
            _logDirectory = logDirectory ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "nU3.Framework",
                "LOG"
            );
        }

        /// <summary>
        /// 로컬에 있는 지정된 로그 파일을 서버로 업로드합니다.
        /// - 파일이 존재하지 않으면 false 반환
        /// - 압축 옵션이 켜져 있고 파일이 충분히 크면 GZip 압축 후 업로드
        /// - 업로드 성공 시 deleteAfterUpload가 true이면 로컬 파일 삭제 시도
        /// </summary>
        /// <param name="localFilePath">업로드할 로컬 파일 전체 경로</param>
        /// <param name="deleteAfterUpload">업로드 후 로컬 파일을 삭제할지 여부</param>
        /// <returns>업로드 성공 여부</returns>
        public async Task<bool> UploadLogFileAsync(string localFilePath, bool deleteAfterUpload = false)
        {
            try
            {
                if (!File.Exists(localFilePath))
                {
                    Log("Warning", $"로그 파일을 찾을 수 없습니다: {localFilePath}");
                    return false;
                }

                var fileName = Path.GetFileName(localFilePath);
                var originalSize = new FileInfo(localFilePath).Length;
                Log("Info", $"로그 파일 업로드 시작: {fileName} ({FormatFileSize(originalSize)})");

                using (var content = new MultipartFormDataContent())
                {
                    byte[] fileData = await File.ReadAllBytesAsync(localFilePath);
                    byte[] uploadData;
                    string uploadFileName;
                    string contentType;

                    if (_enableCompression && ShouldCompress(originalSize))
                    {
                        // 파일을 메모리에서 GZip으로 압축
                        uploadData = CompressData(fileData);
                        uploadFileName = fileName + ".gz";
                        contentType = "application/gzip";
                        
                        var compressedSize = uploadData.Length;
                        var compressionRatio = (1 - (double)compressedSize / originalSize) * 100;
                        Log("Info", $"압축됨: {FormatFileSize(originalSize)} → {FormatFileSize(compressedSize)} ({compressionRatio:F1}% 감소)");
                    }
                    else
                    {
                        // 압축하지 않고 원본 바이트를 그대로 전송
                        uploadData = fileData;
                        uploadFileName = fileName;
                        // 단순 텍스트 로그는 text/plain
                        contentType = "text/plain";
                    }

                    // 바이트 배열을 Multipart로 추가
                    var fileContent = new ByteArrayContent(uploadData);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    content.Add(fileContent, "File", uploadFileName);

                    // 서버 엔드포인트 호출
                    var response = await _httpClient.PostAsync("/api/log/upload", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Log("Info", $"로그 파일 업로드 성공: {fileName}");

                        if (deleteAfterUpload)
                        {
                            try
                            {
                                File.Delete(localFilePath);
                                Log("Info", $"로컬 로그 파일 삭제됨: {fileName}");
                            }
                            catch (Exception ex)
                            {
                                // 삭제 실패는 치명적이지 않으므로 Warning으로 로깅
                                Log("Warning", $"로컬 로그 파일 삭제 실패: {ex.Message}");
                            }
                        }

                        return true;
                    }
                    else
                    {
                        // 서버 응답 코드 포함하여 실패 원인 로깅
                        Log("Error", $"로그 파일 업로드 실패: {response.StatusCode}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // 예외 발생 시 에러 로깅
                Log("Error", $"로그 파일 업로드 중 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 감사 로그 파일을 업로드합니다. (포맷이 JSON인 경우 contentType을 application/json으로 설정)
        /// 동작은 UploadLogFileAsync와 유사하지만 엔드포인트가 /api/log/upload-audit 입니다.
        /// </summary>
        public async Task<bool> UploadAuditLogAsync(string localFilePath, bool deleteAfterUpload = false)
        {
            try
            {
                if (!File.Exists(localFilePath))
                {
                    Log("Warning", $"감사 로그 파일을 찾을 수 없습니다: {localFilePath}");
                    return false;
                }

                var fileName = Path.GetFileName(localFilePath);
                var originalSize = new FileInfo(localFilePath).Length;
                Log("Info", $"감사 로그 업로드 시작: {fileName} ({FormatFileSize(originalSize)})");

                using (var content = new MultipartFormDataContent())
                {
                    byte[] fileData = await File.ReadAllBytesAsync(localFilePath);
                    byte[] uploadData;
                    string uploadFileName;
                    string contentType;

                    if (_enableCompression && ShouldCompress(originalSize))
                    {
                        uploadData = CompressData(fileData);
                        uploadFileName = fileName + ".gz";
                        contentType = "application/gzip";
                        
                        var compressedSize = uploadData.Length;
                        var compressionRatio = (1 - (double)compressedSize / originalSize) * 100;
                        Log("Info", $"압축됨: {FormatFileSize(originalSize)} → {FormatFileSize(compressedSize)} ({compressionRatio:F1}% 감소)");
                    }
                    else
                    {
                        uploadData = fileData;
                        uploadFileName = fileName;
                        // 감사 로그는 JSON 포맷일 가능성이 있으므로 application/json 사용
                        contentType = "application/json";
                    }

                    var fileContent = new ByteArrayContent(uploadData);
                    fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(contentType);
                    content.Add(fileContent, "File", uploadFileName);

                    var response = await _httpClient.PostAsync("/api/log/upload-audit", content);

                    if (response.IsSuccessStatusCode)
                    {
                        Log("Info", $"감사 로그 업로드 성공: {fileName}");

                        if (deleteAfterUpload)
                        {
                            try
                            {
                                File.Delete(localFilePath);
                                Log("Info", $"로컬 감사 로그 삭제됨: {fileName}");
                            }
                            catch (Exception ex)
                            {
                                Log("Warning", $"로컬 감사 로그 삭제 실패: {ex.Message}");
                            }
                        }

                        return true;
                    }
                    else
                    {
                        Log("Error", $"감사 로그 업로드 실패: {response.StatusCode}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error", $"감사 로그 업로드 중 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 로그 디렉토리에서 오늘 날짜 이전의 모든 로그 파일을 찾아 업로드합니다.
        /// 업로드 성공 시 로컬 파일을 삭제합니다.
        /// </summary>
        public async Task<bool> UploadAllPendingLogsAsync()
        {
            try
            {
                if (!Directory.Exists(_logDirectory))
                {
                    Log("Warning", $"로그 디렉토리를 찾을 수 없습니다: {_logDirectory}");
                    return false;
                }

                // 오늘 날짜를 파일명 필터에서 제외하여 진행 중인 로그는 제외
                var today = DateTime.Now.ToString("yyyyMMdd");
                var logFiles = Directory.GetFiles(_logDirectory, "*.log")
                    .Where(f => !Path.GetFileName(f).Contains(today))
                    .ToList();

                if (logFiles.Count == 0)
                {
                    Log("Info", "업로드할 대기 중인 로그가 없습니다");
                    return true;
                }

                Log("Info", $"업로드할 로그 파일 수: {logFiles.Count}");

                var successCount = 0;
                foreach (var logFile in logFiles)
                {
                    if (await UploadLogFileAsync(logFile, deleteAfterUpload: true))
                    {
                        successCount++;
                    }

                    // 서버 부하를 줄이기 위해 업로드 사이에 짧은 딜레이를 둡니다.
                    await Task.Delay(500);
                }

                Log("Info", $"업로드 완료: {successCount}/{logFiles.Count} 파일");
                return successCount > 0;
            }
            catch (Exception ex)
            {
                Log("Error", $"대기 중인 로그 업로드 중 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 현재(오늘) 로그 파일 중 최근에 수정된 파일을 찾아 바로 업로드합니다.
        /// </summary>
        public async Task<bool> UploadCurrentLogImmediatelyAsync()
        {
            try
            {
                // 오늘 날짜의 로그 파일을 파일명으로 필터링
                var today = DateTime.Now.ToString("yyyyMMdd");
                var currentLogFile = Directory.GetFiles(_logDirectory, $"*{today}*.log")
                    .OrderByDescending(f => new FileInfo(f).LastWriteTime)
                    .FirstOrDefault();

                if (currentLogFile != null && File.Exists(currentLogFile))
                {
                    Log("Info", "현재 로그 파일 업로드 시작");
                    return await UploadLogFileAsync(currentLogFile, deleteAfterUpload: false);
                }

                Log("Warning", "현재 로그 파일을 찾을 수 없습니다");
                return false;
            }
            catch (Exception ex)
            {
                Log("Error", $"현재 로그 업로드 중 오류: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 자동 업로드를 활성화하거나 비활성화합니다.
        /// 활성화 시 매일 02:00에 UploadAllPendingLogsAsync를 호출하도록 타이머를 설정합니다.
        /// </summary>
        /// <param name="enable">true이면 활성화, false이면 비활성화</param>
        public void EnableAutoUpload(bool enable)
        {
            _autoUploadEnabled = enable;

            if (enable)
            {
                _autoUploadTimer?.Dispose();
                _autoUploadTimer = new System.Threading.Timer(
                    async _ => await AutoUploadCallback(),
                    null,
                    GetTimeUntil2AM(),
                    TimeSpan.FromDays(1)
                );

                Log("Info", "자동 로그 업로드 활성화 (매일 02:00)");
            }
            else
            {
                _autoUploadTimer?.Dispose();
                _autoUploadTimer = null;
                Log("Info", "자동 로그 업로드 비활성화");
            }
        }

        /// <summary>
        /// 현재 시각으로부터 다음 02:00까지의 TimeSpan을 계산합니다.
        /// (현재가 02:00 이후면 다음날 02:00를 계산)
        /// </summary>
        private TimeSpan GetTimeUntil2AM()
        {
            var now = DateTime.Now;
            var next2AM = DateTime.Today.AddHours(2);
            
            if (now >= next2AM)
            {
                next2AM = next2AM.AddDays(1);
            }

            return next2AM - now;
        }

        /// <summary>
        /// 타이머 콜백: 자동 업로드를 수행합니다.
        /// 예외는 내부에서 캐치하여 로깅합니다.
        /// </summary>
        private async Task AutoUploadCallback()
        {
            try
            {
                Log("Info", "자동 로그 업로드 시작");
                await UploadAllPendingLogsAsync();
            }
            catch (Exception ex)
            {
                Log("Error", $"자동 로그 업로드 중 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 파일 크기가 압축의 이득을 얻을 수 있는지 판단합니다.
        /// 현재는 1KB 초과인 경우에만 압축하도록 설정되어 있습니다.
        /// </summary>
        private bool ShouldCompress(long fileSize)
        {
            // 1KB보다 큰 파일만 압축
            // 매우 작은 파일은 압축의 이득이 없을 수 있음
            return fileSize > 1024;
        }

        /// <summary>
        /// 바이트 배열을 GZip으로 압축하여 반환합니다.
        /// 메모리 스트림을 사용하므로 큰 파일의 경우 메모리 사용량에 주의해야 합니다.
        /// </summary>
        private byte[] CompressData(byte[] data)
        {
            using var outputStream = new MemoryStream();
            using (var gzipStream = new GZipStream(outputStream, CompressionLevel.Optimal))
            {
                gzipStream.Write(data, 0, data.Length);
            }
            return outputStream.ToArray();
        }

        /// <summary>
        /// 바이트 단위를 사람이 읽기 쉬운 문자열로 변환합니다. (B, KB, MB)
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

        /// <summary>
        /// 내부 로깅 호출. 콜백이 제공된 경우에만 호출됩니다.
        /// level: Info/Warning/Error 등의 문자열 (필요시 enum으로 변경 고려)
        /// message: 출력할 메시지
        /// </summary>
        private void Log(string level, string message)
        {
            _logCallback?.Invoke(level, message);
        }

        /// <summary>
        /// IDisposable 구현: 타이머 해제
        /// </summary>
        public void Dispose()
        {
            _autoUploadTimer?.Dispose();
        }
    }
}
