using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Microsoft.Extensions.Configuration; // 추가

namespace nU3.Bootstrapper.Services
{
    /// <summary>
    /// Custom URI Scheme을 등록/해제하는 서비스
    /// 예: nu3://open?screen=PatientList
    /// </summary>
    public class CustomUriManager
    {
        private readonly string _uriScheme;
        private readonly string _applicationPath;
        private readonly string _description;

        /// <summary>
        /// CustomUriManager 생성자
        /// </summary>
        /// <param name="uriScheme">URI 스킴 (예: "nu3")</param>
        /// <param name="applicationPath">실행 파일 경로</param>
        /// <param name="description">설명</param>
        public CustomUriManager(string uriScheme, string applicationPath, string description = "nU3 Framework Protocol")
        {
            _uriScheme = uriScheme ?? throw new ArgumentNullException(nameof(uriScheme));
            _applicationPath = applicationPath ?? throw new ArgumentNullException(nameof(applicationPath));
            _description = description;
        }

        /// <summary>
        /// Custom URI를 등록합니다.
        /// </summary>
        /// <returns>등록 성공 여부</returns>
        public bool Register()
        {
            try
            {
                FileLogger.SectionStart("Custom URI 등록");
                FileLogger.Info($"URI Scheme: {_uriScheme}://");
                FileLogger.Info($"실행 파일: {_applicationPath}");

                if (!File.Exists(_applicationPath))
                {
                    FileLogger.Error($"실행 파일을 찾을 수 없습니다: {_applicationPath}");
                    return false;
                }

                // HKEY_CURRENT_USER에 등록 (관리자 권한 불필요)
                using var key = Registry.CurrentUser.CreateSubKey($@"SOFTWARE\Classes\{_uriScheme}");
                if (key == null)
                {
                    FileLogger.Error("레지스트리 키 생성 실패");
                    return false;
                }

                // 기본 값 설정
                key.SetValue(string.Empty, $"URL:{_description}");
                key.SetValue("URL Protocol", string.Empty);

                // DefaultIcon 설정
                using (var iconKey = key.CreateSubKey("DefaultIcon"))
                {
                    iconKey?.SetValue(string.Empty, $"\"{_applicationPath}\",0");
                }

                // shell\open\command 설정
                using (var commandKey = key.CreateSubKey(@"shell\open\command"))
                {
                    commandKey?.SetValue(string.Empty, $"\"{_applicationPath}\" \"%1\"");
                }

                FileLogger.Info("Custom URI 등록 성공");
                FileLogger.SectionEnd("Custom URI 등록");

                return true;
            }
            catch (Exception ex)
            {
                FileLogger.Error("Custom URI 등록 중 오류", ex);
                FileLogger.SectionEnd("Custom URI 등록");
                return false;
            }
        }

        /// <summary>
        /// Custom URI 등록을 해제합니다.
        /// </summary>
        /// <returns>해제 성공 여부</returns>
        public bool Unregister()
        {
            try
            {
                FileLogger.SectionStart("Custom URI 해제");
                FileLogger.Info($"URI Scheme: {_uriScheme}://");

                Registry.CurrentUser.DeleteSubKeyTree($@"SOFTWARE\Classes\{_uriScheme}", false);

                FileLogger.Info("Custom URI 해제 성공");
                FileLogger.SectionEnd("Custom URI 해제");

                return true;
            }
            catch (Exception ex)
            {
                FileLogger.Error("Custom URI 해제 중 오류", ex);
                FileLogger.SectionEnd("Custom URI 해제");
                return false;
            }
        }

        /// <summary>
        /// Custom URI가 등록되어 있는지 확인합니다.
        /// </summary>
        /// <returns>등록 여부</returns>
        public bool IsRegistered()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey($@"SOFTWARE\Classes\{_uriScheme}");
                return key != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Custom URI를 테스트합니다.
        /// </summary>
        /// <param name="testUri">테스트할 URI (예: "nu3://test")</param>
        /// <returns>테스트 성공 여부</returns>
        public bool TestUri(string testUri)
        {
            try
            {
                FileLogger.Info($"Custom URI 테스트: {testUri}");

                var startInfo = new ProcessStartInfo
                {
                    FileName = testUri,
                    UseShellExecute = true
                };

                using var process = Process.Start(startInfo);
                
                FileLogger.Info("Custom URI 테스트 성공");
                return true;
            }
            catch (Exception ex)
            {
                FileLogger.Error($"Custom URI 테스트 실패: {testUri}", ex);
                return false;
            }
        }

        /// <summary>
        /// Configuration에서 Custom URI 설정을 읽어 등록
        /// </summary>
        public static bool RegisterFromConfiguration(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            try
            {
                var uriScheme = configuration.GetValue<string>("CustomUri:Scheme");
                var enabled = configuration.GetValue<bool>("CustomUri:Enabled", false);
                var description = configuration.GetValue<string>("CustomUri:Description", "nU3 Framework Protocol");

                if (!enabled || string.IsNullOrWhiteSpace(uriScheme))
                {
                    FileLogger.Info("Custom URI가 비활성화되었거나 설정되지 않았습니다.");
                    return false;
                }

                // Shell 실행 파일 경로 확인
                var shellPath = configuration.GetValue<string>("RuntimeDirectory");
                var shellExe = configuration.GetValue<string>("MainExecutable");

                if (string.IsNullOrWhiteSpace(shellPath) || string.IsNullOrWhiteSpace(shellExe))
                {
                    FileLogger.Warning("Shell 실행 파일 경로가 설정되지 않았습니다.");
                    return false;
                }

                var applicationPath = Path.Combine(shellPath, shellExe);

                var manager = new CustomUriManager(uriScheme, applicationPath, description);
                return manager.Register();
            }
            catch (Exception ex)
            {
                FileLogger.Error("Configuration에서 Custom URI 등록 중 오류", ex);
                return false;
            }
        }

        /// <summary>
        /// Configuration에서 Custom URI 설정을 읽어 해제
        /// </summary>
        public static bool UnregisterFromConfiguration(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            try
            {
                var uriScheme = configuration.GetValue<string>("CustomUri:Scheme");

                if (string.IsNullOrWhiteSpace(uriScheme))
                {
                    FileLogger.Info("Custom URI 스킴이 설정되지 않았습니다.");
                    return false;
                }

                var manager = new CustomUriManager(uriScheme, string.Empty);
                return manager.Unregister();
            }
            catch (Exception ex)
            {
                FileLogger.Error("Configuration에서 Custom URI 해제 중 오류", ex);
                return false;
            }
        }
    }
}
