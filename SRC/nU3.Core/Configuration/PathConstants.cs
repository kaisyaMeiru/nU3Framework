using nU3.Core.Services;
using System;
using System.IO;

namespace nU3.Core.Configuration
{
    /// <summary>
    /// 시스템 전반에서 사용되는 파일 경로 상수를 정의합니다.
    /// Bootstrapper와 Shell 간의 경로 일관성을 보장합니다.
    /// </summary>
    public static class PathConstants
    {
        internal const string PatchDirectoryStr = "Patch";
        internal const string ModuleDirectoryStr = "Modules";
        internal const string CacheDirectoryStr = "Cache";
        internal const string ServerStorageDirectoryStr = "ServerStorage";
        internal const string FrameWorkDirectoryStr = "nU3.Framework";
        internal const string ShadowDirectoryStr = "Shadow";

        /// <summary>
        /// AppData 루트 경로: %AppData%\nU3.Framework
        /// </summary>
        public static string AppDataRoot =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), FrameWorkDirectoryStr);

        /// <summary>
        /// 모듈 다운로드 캐시 경로: %AppData%\nU3.Framework\Cache
        /// </summary>
        public static string CacheDirectory => Path.Combine(AppDataRoot, CacheDirectoryStr);

        /// <summary>
        /// 서버 저장소 캐시 경로 (구버전 호환용): %AppData%\nU3.Framework\ServerStorage
        /// </summary>
        public static string ServerStorageDirectory => Path.Combine(AppDataRoot, ServerStorageDirectoryStr);
        
        /// <summary>
        /// 런타임 모듈 폴더 경로를 반환합니다.
        /// 기본값: {BaseDirectory}\Modules
        /// </summary>
        public static string GetRuntimeModulesPath(string baseDirectory) =>
            Path.Combine(baseDirectory, ModuleDirectoryStr);
    }
}