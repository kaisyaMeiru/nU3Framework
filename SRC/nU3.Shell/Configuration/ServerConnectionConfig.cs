using System;
using System.IO;
using System.Text.Json;

namespace nU3.Shell.Configuration
{
    /// <summary>
    /// 서버 연결 설정
    /// </summary>
    public class ServerConnectionConfig
    {
        public bool Enabled { get; set; }
        public string BaseUrl { get; set; } = "https://localhost:64229";
        public int Timeout { get; set; } = 300;
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// appsettings.json에서 서버 연결 설정을 로드합니다
        /// </summary>
        public static ServerConnectionConfig Load()
        {
            try
            {
                var configFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (!File.Exists(configFile))
                {
                    return GetDefault();
                }

                var json = File.ReadAllText(configFile);
                using var doc = JsonDocument.Parse(json);

                if (!doc.RootElement.TryGetProperty("ServerConnection", out var serverConfig))
                {
                    return GetDefault();
                }

                return new ServerConnectionConfig
                {
                    Enabled = GetBoolValue(serverConfig, "Enabled", true),
                    BaseUrl = GetStringValue(serverConfig, "BaseUrl", "https://localhost:64229"),
                    Timeout = GetIntValue(serverConfig, "Timeout", 300),
                    RetryCount = GetIntValue(serverConfig, "RetryCount", 3),
                };
            }
            catch
            {
                return GetDefault();
            }
        }

        private static ServerConnectionConfig GetDefault()
        {
            return new ServerConnectionConfig
            {
                Enabled = true,
                BaseUrl = "https://localhost:64229",
                Timeout = 300,
                RetryCount = 3,
            };
        }

        private static string GetStringValue(JsonElement element, string key, string defaultValue)
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

        private static bool GetBoolValue(JsonElement element, string key, bool defaultValue)
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

        private static int GetIntValue(JsonElement element, string key, int defaultValue)
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
    }
}
