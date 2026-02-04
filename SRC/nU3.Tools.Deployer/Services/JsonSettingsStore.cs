using System;
using System.IO;
using System.Text.Json;

namespace nU3.Tools.Deployer.Services
{
    /// <summary>
    /// 간단한 JSON 기반 설정 저장소 유틸리티입니다.
    /// Deployer 도구의 설정(appsettings.json)에서 ModuleSettings 섹션을 읽고 쓰는 책임을 가집니다.
    /// </summary>
    public static class JsonSettingsStore
    {
        // 설정 파일명
        private const string FileName = "appsettings.json";

        /// <summary>
        /// 현재 실행 컨텍스트 기준으로 설정 파일의 전체 경로를 반환합니다.
        /// </summary>
        /// <returns>appsettings.json 파일의 전체 경로</returns>
        public static string GetSettingsPath()
        {
            return Path.Combine(AppContext.BaseDirectory, FileName);
        }

        /// <summary>
        /// appsettings.json에서 ModuleSettings 섹션을 읽어 ModuleSettings 객체로 반환합니다.
        /// 파일 또는 섹션이 존재하지 않으면 기본값(ModuleSettings의 새 인스턴스)을 반환합니다.
        /// </summary>
        /// <returns>로드된 ModuleSettings 인스턴스</returns>
        public static ModuleSettings LoadModuleSettings()
        {
            var path = GetSettingsPath();
            if (!File.Exists(path))
            {
                // 설정 파일이 없으면 기본 설정 반환
                return new ModuleSettings();
            }

            // 파일을 열어 JSON 문서로 파싱
            using var stream = File.OpenRead(path);
            using var doc = JsonDocument.Parse(stream);

            // ModuleSettings 섹션이 없으면 기본값 반환
            if (!doc.RootElement.TryGetProperty("ModuleSettings", out var moduleSettings))
            {
                return new ModuleSettings();
            }

            var settings = new ModuleSettings();

            // ModulesRootPath 값을 읽어 설정에 반영
            if (moduleSettings.TryGetProperty("ModulesRootPath", out var modulesRoot))
            {
                settings.ModulesRootPath = modulesRoot.GetString() ?? string.Empty;
            }

            return settings;
        }

        /// <summary>
        /// 주어진 ModuleSettings 객체를 appsettings.json의 ModuleSettings 섹션으로 직렬화하여 저장합니다.
        /// 기존 파일이 없으면 경로를 생성하고 새 파일을 생성합니다.
        /// </summary>
        /// <param name="settings">저장할 ModuleSettings 인스턴스</param>
        public static void SaveModuleSettings(ModuleSettings settings)
        {
            var path = GetSettingsPath();

            // 저장할 모델을 익명 타입으로 구성
            var model = new
            {
                ModuleSettings = new
                {
                    ModulesRootPath = settings.ModulesRootPath
                }
            };

            // 들여쓰기 옵션으로 JSON 직렬화
            var json = JsonSerializer.Serialize(model, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            // 디렉터리 존재 여부 확인 후 생성
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, json);
        }
    }
}
