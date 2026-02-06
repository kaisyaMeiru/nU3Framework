using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace nU3.Server.Connectivity.Services
{
    /// <summary>
    /// 서버 측 파일 전송 및 파일 시스템 작업을 담당하는 서비스입니다.
    /// 
    /// 주요 역할:
    /// - 서버 저장소(홈 디렉토리)를 기준으로 파일 및 디렉토리의 생성/삭제/이동/조회 기능 제공
    /// - 파일 업로드(쓰기) 및 다운로드(읽기)를 비동기적으로 처리
    /// - 경로 해석(상대/절대) 및 모든 동작에 대해 로깅 수행
    /// 
    /// 설계/운영상 주의사항:
    /// - 현재 ResolvePath는 단순 결합과 절대 경로 통과 처리를 수행합니다. 운영 환경에서는 디렉토리 트래버설(../) 방지,
    ///   허용 경로 화이트리스트, 권한 검사 등을 반드시 추가해야 합니다.
    /// - 파일 IO는 예외가 발생할 수 있으므로 호출자는 false/null 반환값을 통해 실패를 감지해야 합니다.
    /// - 대용량 파일 처리 시 메모리 사용과 스트리밍 방식을 고려하세요(현재 SaveFileAsync는 바이트 배열을 사용합니다).
    /// </summary>
    public class ServerFileTransferService
    {
        // 기본 홈 디렉토리(설정으로 대체 가능)
        public static string _serverHomeDirectory = string.Empty;
        private static bool _homeDirectoryInitialized;
        private static readonly object _homeDirectoryLock = new();
        private readonly ILogger<ServerFileTransferService> _logger;
        private bool _initializedFromConfig;

        /// <summary>
        /// 생성자: IConfiguration에서 HomeDirectory를 읽어 초기화합니다.
        /// </summary>
        /// <param name="logger">로깅 인스턴스</param>
        /// <param name="configuration">앱 설정(IConfiguration) - ServerFileTransfer:HomeDirectory 키를 사용</param>
        public ServerFileTransferService(ILogger<ServerFileTransferService> logger, IConfiguration configuration)
        {
            _logger = logger;

            InitializeHomeDirectoryFromConfig(configuration);

            // 홈 디렉토리가 없으면 생성
            try
            {
                if (!Directory.Exists(_serverHomeDirectory))
                {
                    Directory.CreateDirectory(_serverHomeDirectory);
                    _logger.LogInformation("서버 홈 디렉토리를 생성했습니다: {HomePath}", _serverHomeDirectory);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "서버 홈 디렉토리 초기화에 실패했습니다: {HomePath}", _serverHomeDirectory);
            }
        }

        private void InitializeHomeDirectoryFromConfig(IConfiguration configuration)
        {
            if (_homeDirectoryInitialized)
            {
                return;
            }

            lock (_homeDirectoryLock)
            {
                if (_homeDirectoryInitialized)
                {
                    return;
                }

                try
                {
                    if (configuration != null)
                    {
                        var configuredPath = configuration["ServerFileTransfer:HomeDirectory"];
                        if (!string.IsNullOrWhiteSpace(configuredPath))
                        {
                            _serverHomeDirectory = configuredPath;
                            _initializedFromConfig = true;
                            _logger.LogInformation("서버 홈 디렉토리를 설정에서 초기화했습니다: {HomePath}", _serverHomeDirectory);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("IConfiguration이 null입니다. 기본 홈 디렉토리를 사용합니다: {HomePath}", _serverHomeDirectory);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "서버 홈 디렉토리 설정을 읽는 중 오류가 발생했습니다. 기본 경로를 사용합니다: {HomePath}", _serverHomeDirectory);
                }

                _homeDirectoryInitialized = true;
            }
        }

        /// <summary>
        /// 서버 홈 디렉토리를 설정합니다.
        /// 실제 운영 환경에서는 입력값 검증(경로 허용 여부, 권한 검사 등)을 수행해야 합니다.
        /// </summary>
        /// <param name="isUseHomePath">홈 경로 사용 플래그(현재는 사용 여부 로깅 목적)</param>
        /// <param name="serverHomePath">설정할 홈 경로(절대 또는 상대)</param>
        /// <returns>성공하면 true, 실패하면 false</returns>
        public bool SetHomeDirectory(bool isUseHomePath, string serverHomePath)
        {
            _logger.LogInformation("홈 디렉토리 설정 호출: UseHome={UseHome}, Path={Path}", isUseHomePath, serverHomePath);
            if (!string.IsNullOrWhiteSpace(serverHomePath))
            {
                _serverHomeDirectory = serverHomePath;
                if (!Directory.Exists(_serverHomeDirectory))
                {
                    try
                    {
                        Directory.CreateDirectory(_serverHomeDirectory);
                        _logger.LogInformation("홈 디렉토리를 생성했습니다: {Path}", _serverHomeDirectory);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "홈 디렉토리 생성 실패: {Path}", _serverHomeDirectory);
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// 현재 서버 홈 디렉토리 경로를 반환합니다.
        /// </summary>
        /// <returns>홈 디렉토리의 전체 경로 문자열</returns>
        public string GetHomeDirectory()
        {
            return _serverHomeDirectory;
        }

        /// <summary>
        /// 입력된 경로를 실제 파일 시스템 경로로 해석합니다.
        /// 상대 경로나 ~로 시작하는 경로는 홈 디렉토리에 매핑합니다.
        /// 운영 환경에서는 디렉토리 트래버설(../) 방지 및 접근 제어가 필요합니다.
        /// </summary>
        /// <param name="path">클라이언트에서 전달한 경로(상대/절대/빈 문자열)</param>
        /// <returns>해석된 절대 파일 시스템 경로</returns>
        private string ResolvePath(string path)
        {
            string resolvedPath;
            if (string.IsNullOrEmpty(path))
            {
                resolvedPath = _serverHomeDirectory;
            }
            else if (Path.IsPathRooted(path))
            {
                // 절대 경로일 경우 그대로 사용. 실환경에서는 허용 여부 검사 필요
                resolvedPath = path;
            }
            else
            {
                // 상대 경로는 홈 디렉토리에 결합
                resolvedPath = Path.Combine(_serverHomeDirectory, path);
            }

            _logger.LogDebug("경로 해석: 입력='{Input}' -> 해석='{Resolved}'", path, resolvedPath);
            return resolvedPath;
        }

        /// <summary>
        /// 디렉토리를 생성합니다. 이미 존재하면 true를 반환합니다.
        /// </summary>
        /// <param name="fullPath">생성할 디렉토리 경로(상대 또는 절대)</param>
        /// <returns>성공 여부</returns>
        public bool CreateDirectory(string fullPath)
        {
            try
            {
                string path = ResolvePath(fullPath);
                _logger.LogInformation("디렉토리 생성 시도: {Path}", path);

                if (Directory.Exists(path))
                {
                    _logger.LogInformation("디렉토리가 이미 존재합니다: {Path}", path);
                    return true;
                }

                Directory.CreateDirectory(path);
                _logger.LogInformation("디렉토리 생성 성공: {Path}", path);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "디렉토리 생성 실패: {Path}", fullPath);
                return false;
            }
        }

        /// <summary>
        /// 디렉토리 존재 여부를 반환합니다.
        /// </summary>
        /// <param name="fullPath">확인할 경로</param>
        /// <returns>존재하면 true</returns>
        public bool ExistDirectory(string fullPath)
        {
            string path = ResolvePath(fullPath);
            return Directory.Exists(path);
        }

        /// <summary>
        /// 디렉토리를 재귀적으로 삭제합니다.
        /// </summary>
        /// <param name="fullPath">삭제할 경로</param>
        /// <returns>성공 여부</returns>
        public bool DeleteDirectory(string fullPath)
        {
            try
            {
                string path = ResolvePath(fullPath);
                _logger.LogInformation("디렉토리 삭제 시도: {Path}", path);

                if (!Directory.Exists(path)) return false;
                Directory.Delete(path, true);
                _logger.LogInformation("디렉토리 삭제 성공: {Path}", path);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "디렉토리 삭제 실패: {Path}", fullPath);
                return false;
            }
        }

        /// <summary>
        /// 디렉토리 이름(경로) 변경
        /// </summary>
        /// <param name="sourcePath">원본 경로</param>
        /// <param name="destPath">대상 경로</param>
        /// <returns>성공 여부</returns>
        public bool RenameDirectory(string sourcePath, string destPath)
        {
            try
            {
                string src = ResolvePath(sourcePath);
                string dst = ResolvePath(destPath);

                _logger.LogInformation("디렉토리 이름 변경 시도: {Src} -> {Dst}", src, dst);

                if (Directory.Exists(src) && !Directory.Exists(dst))
                {
                    Directory.Move(src, dst);
                    _logger.LogInformation("디렉토리 이름 변경 성공");
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "디렉토리 이름 변경 실패");
                return false;
            }
        }

        /// <summary>
        /// 지정 경로의 파일 목록(파일명만)을 반환합니다.
        /// </summary>
        /// <param name="fullPath">조회할 경로</param>
        /// <returns>파일명 리스트</returns>
        public List<string> GetFileList(string fullPath)
        {
            try
            {
                string path = ResolvePath(fullPath);
                if (Directory.Exists(path))
                {
                    return Directory.GetFiles(path).Select(Path.GetFileName).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "파일 목록 조회 중 오류: {Path}", fullPath);
            }
            return new List<string>();
        }

        /// <summary>
        /// 지정 경로의 하위 디렉토리 이름 목록을 반환합니다.
        /// </summary>
        /// <param name="fullPath">조회할 경로</param>
        /// <returns>하위 디렉토리명 리스트</returns>
        public List<string> GetSubDirectoryList(string fullPath)
        {
            try
            {
                string path = ResolvePath(fullPath);
                if (Directory.Exists(path))
                {
                    return Directory.GetDirectories(path).Select(Path.GetFileName).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "하위 디렉토리 목록 조회 중 오류: {Path}", fullPath);
            }
            return new List<string>();
        }

        // ==========================
        // 파일 입출력
        // ==========================

        /// <summary>
        /// 서버에 파일을 저장합니다(업로드 처리).
        /// 주의: 현재 메서드는 전체 파일 바이트를 메모리로 받습니다. 대용량 파일은 스트리밍 방식으로 변경 권장.
        /// </summary>
        /// <param name="serverPath">서버 내 저장 경로(상대 또는 절대)</param>
        /// <param name="data">파일 바이트(바이트 배열)</param>
        /// <returns>성공 시 true, 실패 시 false</returns>
        public async Task<bool> SaveFileAsync(string serverPath, byte[] data)
        {
            try
            {
                string path = ResolvePath(serverPath);
                string dir = Path.GetDirectoryName(path);

                _logger.LogInformation("파일 저장 시도: 대상={Path}, 크기={Size} 바이트", path, data?.Length ?? 0);

                if (!Directory.Exists(dir))
                {
                    _logger.LogInformation("파일의 부모 디렉토리를 생성합니다: {Dir}", dir);
                    Directory.CreateDirectory(dir);
                }

                await File.WriteAllBytesAsync(path, data);
                _logger.LogInformation("파일 저장 완료: {Path}", path);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "파일 저장 중 오류: {Path}", serverPath);
                return false;
            }
        }

        /// <summary>
        /// 서버에서 파일을 읽어 바이트 배열로 반환합니다(다운로드 처리).
        /// 실패 시 null을 반환할 수 있습니다.
        /// </summary>
        /// <param name="serverPath">서버 내 파일 경로</param>
        /// <returns>파일 바이트 배열 또는 null</returns>
        public async Task<byte[]> ReadFileAsync(string serverPath)
        {
            try
            {
                string path = ResolvePath(serverPath);
                _logger.LogInformation("파일 읽기 시도: 대상={Path}", path);

                if (!File.Exists(path))
                {
                    _logger.LogWarning("파일을 찾을 수 없습니다: {Path}", path);
                    return null;
                }

                var data = await File.ReadAllBytesAsync(path);
                _logger.LogInformation("파일 읽기 완료: {Path}, 크기={Size} 바이트", path, data.Length);
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "파일 읽기 중 오류: {Path}", serverPath);
                return null;
            }
        }

        /// <summary>
        /// 파일 존재 여부 확인
        /// </summary>
        /// <param name="fullFilePath">확인할 파일 경로</param>
        /// <returns>존재하면 true</returns>
        public bool ExistFile(string fullFilePath)
        {
            string path = ResolvePath(fullFilePath);
            return File.Exists(path);
        }

        /// <summary>
        /// 파일 삭제
        /// </summary>
        /// <param name="fullFilePath">삭제할 파일 경로</param>
        /// <returns>성공 여부</returns>
        public bool DeleteFile(string fullFilePath)
        {
            try
            {
                string path = ResolvePath(fullFilePath);
                if (File.Exists(path))
                {
                    File.Delete(path);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "파일 삭제 중 오류: {Path}", fullFilePath);
                return false;
            }
        }

        /// <summary>
        /// 파일 복사
        /// </summary>
        /// <param name="sourceFullPath">원본 경로</param>
        /// <param name="destFullPath">대상 경로</param>
        /// <param name="overWrite">대상 파일이 존재할 경우 덮어쓸지 여부</param>
        /// <returns>성공 여부</returns>
        public bool CopyFile(string sourceFullPath, string destFullPath, bool overWrite = true)
        {
            try
            {
                string src = ResolvePath(sourceFullPath);
                string dst = ResolvePath(destFullPath);
                if (File.Exists(src))
                {
                    File.Copy(src, dst, overWrite);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "파일 복사 중 오류: {Src} -> {Dst}", sourceFullPath, destFullPath);
                return false;
            }
        }

        /// <summary>
        /// 파일 이동
        /// </summary>
        /// <param name="sourceFullPath">원본 경로</param>
        /// <param name="destFullPath">대상 경로</param>
        /// <param name="overWrite">대상 파일이 존재할 경우 덮어쓸지 여부</param>
        /// <returns>성공 여부</returns>
        public bool MoveFile(string sourceFullPath, string destFullPath, bool overWrite = true)
        {
            try
            {
                string src = ResolvePath(sourceFullPath);
                string dst = ResolvePath(destFullPath);

                if (File.Exists(src))
                {
                    if (File.Exists(dst))
                    {
                        if (overWrite) File.Delete(dst);
                        else return false;
                    }
                    File.Move(src, dst);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "파일 이동 중 오류: {Src} -> {Dst}", sourceFullPath, destFullPath);
                return false;
            }
        }

        /// <summary>
        /// 파일 크기 조회
        /// </summary>
        /// <param name="fileFullPath">조회할 파일 경로</param>
        /// <returns>파일 크기(바이트) 또는 -1(오류)</returns>
        public long GetFileSize(string fileFullPath)
        {
            try
            {
                string path = ResolvePath(fileFullPath);
                if (File.Exists(path))
                {
                    return new FileInfo(path).Length;
                }
                return -1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "파일 크기 조회 중 오류: {Path}", fileFullPath);
                return -1;
            }
        }
    }
}
