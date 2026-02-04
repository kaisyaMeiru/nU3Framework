using System;
using System.IO;
using System.Text.Json;

namespace nU3.Core.UI.Shell
{
    /// <summary>
    /// appsettings.json에서 셸 구성을 로드하기 위한 헬퍼 클래스입니다.
    /// </summary>
    public static class ShellConfiguration
    {
        private static JsonDocument? _cachedConfig;
        private static DateTime _cacheTime;
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

        /// <summary>
        /// appsettings.json에서 구성 문서를 로드합니다.
        /// </summary>
        public static JsonDocument? LoadConfiguration(bool useCache = true)
        {
            try
            {
                if (useCache && _cachedConfig != null && DateTime.Now - _cacheTime < CacheDuration)
                {
                    return _cachedConfig;
                }

                var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (!File.Exists(configFile))
                    return null;

                var json = File.ReadAllText(configFile);
                _cachedConfig = JsonDocument.Parse(json);
                _cacheTime = DateTime.Now;
                return _cachedConfig;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 구성 캐시를 지웁니다.
        /// </summary>
        public static void ClearCache()
        {
            _cachedConfig = null;
        }

        /// <summary>
        /// 구성에서 문자열 값을 가져옵니다.
        /// </summary>
        public static string? GetConfigValue(JsonElement parent, string section, string key)
        {
            try
            {
                if (parent.TryGetProperty(section, out var sectionElement))
                {
                    if (sectionElement.TryGetProperty(key, out var valueElement))
                    {
                        var value = valueElement.GetString();
                        return string.IsNullOrWhiteSpace(value) ? null : value;
                    }
                }
            }
            catch { }
            return null;
        }

        /// <summary>
        /// 구성에서 boolean 값을 가져옵니다.
        /// </summary>
        public static bool GetConfigBoolValue(JsonElement parent, string section, string key, bool defaultValue = false)
        {
            try
            {
                if (parent.TryGetProperty(section, out var sectionElement))
                {
                    if (sectionElement.TryGetProperty(key, out var valueElement))
                    {
                        return valueElement.GetBoolean();
                    }
                }
            }
            catch { }
            return defaultValue;
        }

        /// <summary>
        /// 구성에서 정수 값을 가져옵니다.
        /// </summary>
        public static int GetConfigIntValue(JsonElement parent, string section, string key, int defaultValue = 0)
        {
            try
            {
                if (parent.TryGetProperty(section, out var sectionElement))
                {
                    if (sectionElement.TryGetProperty(key, out var valueElement))
                    {
                        return valueElement.GetInt32();
                    }
                }
            }
            catch { }
            return defaultValue;
        }

        /// <summary>
        /// 에러 리포팅을 위한 이메일 설정을 로드합니다.
        /// </summary>
        public static EmailSettings? LoadEmailSettings()
        {
            try
            {
                var config = LoadConfiguration();
                if (config == null)
                    return null;

                if (!config.RootElement.TryGetProperty("ErrorReporting", out var errorReporting))
                    return null;

                if (!errorReporting.TryGetProperty("Enabled", out var enabled) || !enabled.GetBoolean())
                    return null;

                if (!errorReporting.TryGetProperty("Email", out var emailConfig))
                    return null;

                return new EmailSettings
                {
                    SmtpServer = GetStringProperty(emailConfig, "SmtpServer", "smtp.gmail.com"),
                    SmtpPort = GetIntProperty(emailConfig, "SmtpPort", 587),
                    EnableSsl = GetBoolProperty(emailConfig, "EnableSsl", true),
                    Username = GetStringProperty(emailConfig, "Username"),
                    Password = GetStringProperty(emailConfig, "Password"),
                    FromEmail = GetStringProperty(emailConfig, "FromEmail") ?? "",
                    FromName = GetStringProperty(emailConfig, "FromName", "nU3 Framework"),
                    ToEmail = GetStringProperty(emailConfig, "ToEmail") ?? "",
                    TimeoutMs = GetIntProperty(emailConfig, "TimeoutMs", 30000)
                };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 서버 연결 설정을 로드합니다.
        /// </summary>
        public static ServerConnectionSettings LoadServerConnectionSettings()
        {
            try
            {
                var config = LoadConfiguration();
                if (config == null)
                    return new ServerConnectionSettings();

                if (!config.RootElement.TryGetProperty("ServerConnection", out var serverConfig))
                    return new ServerConnectionSettings();

                return new ServerConnectionSettings
                {
                    Enabled = GetBoolProperty(serverConfig, "Enabled", false),
                    BaseUrl = GetStringProperty(serverConfig, "BaseUrl", "https://localhost:64229"),
                    Timeout = GetIntProperty(serverConfig, "Timeout", 300),
                    RetryCount = GetIntProperty(serverConfig, "RetryCount", 3),
                    AutoLogUpload = GetBoolProperty(serverConfig, "AutoLogUpload", true),
                    MaxConcurrentConnections = GetIntProperty(serverConfig, "MaxConcurrentConnections", 10)
                };
            }
            catch
            {
                return new ServerConnectionSettings();
            }
        }

        /// <summary>
        /// 로깅 구성 설정을 로드합니다.
        /// </summary>
        public static LoggingSettings LoadLoggingSettings()
        {
            try
            {
                var config = LoadConfiguration();
                if (config == null)
                    return new LoggingSettings();

                if (!config.RootElement.TryGetProperty("Logging", out var loggingConfig))
                    return new LoggingSettings();

                return new LoggingSettings
                {
                    Enabled = GetBoolProperty(loggingConfig, "Enabled", true),
                    LogDirectory = GetConfigValue(loggingConfig, "FileLogging", "LogDirectory"),
                    AuditDirectory = GetConfigValue(loggingConfig, "AuditLogging", "AuditDirectory"),
                    AutoUpload = GetConfigBoolValue(loggingConfig, "ServerUpload", "AutoUpload"),
                    UploadOnError = GetConfigBoolValue(loggingConfig, "ServerUpload", "UploadOnError"),
                    RetentionDays = GetConfigIntValue(loggingConfig, "FileLogging", "RetentionDays", 30)
                };
            }
            catch
            {
                return new LoggingSettings();
            }
        }

        #region Private Helpers

        private static string? GetStringProperty(JsonElement element, string key, string? defaultValue = null)
        {
            try
            {
                if (element.TryGetProperty(key, out var value))
                {
                    var str = value.GetString();
                    return string.IsNullOrWhiteSpace(str) ? defaultValue : str;
                }
            }
            catch { }
            return defaultValue;
        }

        private static bool GetBoolProperty(JsonElement element, string key, bool defaultValue = false)
        {
            try
            {
                if (element.TryGetProperty(key, out var value))
                {
                    return value.GetBoolean();
                }
            }
            catch { }
            return defaultValue;
        }

        private static int GetIntProperty(JsonElement element, string key, int defaultValue = 0)
        {
            try
            {
                if (element.TryGetProperty(key, out var value))
                {
                    return value.GetInt32();
                }
            }
            catch { }
            return defaultValue;
        }

        #endregion
    }

    #region Settings Classes

    /// <summary>
    /// 서버 연결 설정
    /// </summary>
    public class ServerConnectionSettings
    {
        public bool Enabled { get; set; }
        public string? BaseUrl { get; set; } = "https://localhost:64229";
        public int Timeout { get; set; } = 300;
        public int RetryCount { get; set; } = 3;
        public bool AutoLogUpload { get; set; } = true;
        public int MaxConcurrentConnections { get; set; } = 10;
    }

    /// <summary>
    /// 에러 리포팅을 위한 이메일 설정
    /// </summary>
    public class EmailSettings
    {
        public string SmtpServer { get; set; } = "smtp.gmail.com";
        public int SmtpPort { get; set; } = 587;
        public bool EnableSsl { get; set; } = true;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string FromEmail { get; set; } = "";
        public string FromName { get; set; } = "nU3 Framework";
        public string ToEmail { get; set; } = "";
        public int TimeoutMs { get; set; } = 30000;
    }

    /// <summary>
    /// 로깅 설정
    /// </summary>
    public class LoggingSettings
    {
        public bool Enabled { get; set; } = true;
        public string? LogDirectory { get; set; }
        public string? AuditDirectory { get; set; }
        public bool AutoUpload { get; set; }
        public bool UploadOnError { get; set; }
        public int RetentionDays { get; set; } = 30;
    }

    #endregion
}
