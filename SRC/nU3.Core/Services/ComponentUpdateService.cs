using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using nU3.Core.Repositories;
using nU3.Models;

namespace nU3.Core.Services
{
    /// <summary>
    /// 클라이언트 측 컴포넌트 업데이트 서비스입니다.
    /// 
    /// 기능:
    /// - 로컬에 설치된 컴포넌트 목록을 검사하고 서버(Repository)에 등록된 활성 버전과 비교하여 업데이트가 필요한 항목을 식별합니다.
    /// - 누락된(미설치) 필수 컴포넌트를 확인합니다.
    /// - 컴포넌트를 다운로드(캐시)하고 설치(배포)합니다.
    /// - 설치 무결성 검증(파일 해시 비교)을 수행합니다.
    /// - 전체 업데이트 작업의 진행률 이벤트와 개별 컴포넌트 업데이트 완료 이벤트를 발행합니다.
    /// 
    /// 설계 고려사항:
    /// - 다운로드는 현재 단순 파일 복사로 구현되어 있으나, 필요 시 HTTP/파일전송 클라이언트로 대체 가능합니다.
    /// - 파일 해시(SHA256)를 사용하여 전송 무결성을 검증합니다.
    /// - 설치 중 파일 사용 중 문제가 발생하면 재시도 로직을 적용합니다.
    /// - UI와 통합 시 IProgress<T>와 이벤트를 통해 진행률을 전달합니다.
    /// </summary>
    public class ComponentUpdateService
    {
        private readonly IComponentRepository _componentRepo;
        private readonly string _installBasePath;
        private readonly string _downloadCachePath;

        /// <summary>
        /// 설치 기본 경로(예: 애플리케이션 실행 경로)
        /// </summary>
        public string InstallBasePath => _installBasePath;

        /// <summary>
        /// 다운로드 임시 캐시 경로
        /// </summary>
        public string DownloadCachePath => _downloadCachePath;

        /// <summary>
        /// 업데이트 진행률 변경 시 발생하는 이벤트
        /// </summary>
        public event EventHandler<ComponentUpdateProgressEventArgs>? ProgressChanged;

        /// <summary>
        /// 개별 컴포넌트 업데이트 완료(또는 실패) 시 발생하는 이벤트
        /// </summary>
        public event EventHandler<ComponentUpdatedEventArgs>? ComponentUpdated;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="componentRepo">컴포넌트 메타데이터에 접근하는 리포지토리</param>
        /// <param name="installBasePath">설치 기본 경로(생략 시 AppDomain.CurrentDomain.BaseDirectory)</param>
        public ComponentUpdateService(IComponentRepository componentRepo, string? installBasePath = null)
        {
            _componentRepo = componentRepo ?? throw new ArgumentNullException(nameof(componentRepo));

            // BaseInstallPath = 실행 파일이 있는 루트 디렉토리
            _installBasePath = installBasePath ?? AppDomain.CurrentDomain.BaseDirectory;

            _downloadCachePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "nU3.Framework", "Cache", "Components");

            if (!Directory.Exists(_downloadCachePath))
                Directory.CreateDirectory(_downloadCachePath);
        }

        /// <summary>
        /// 컴포넌트의 설치 대상 전체 경로를 계산하여 반환합니다.
        /// InstallPath가 비어있으면 파일명만 사용하고, 있으면 base + installPath + fileName을 조합합니다.
        /// </summary>
        /// <param name="component">컴포넌트 메타데이터</param>
        /// <returns>설치될 전체 파일 경로</returns>
        public string GetInstallPath(ComponentMstDto component)
        {
            if (component == null)
                throw new ArgumentNullException(nameof(component));

            var relativePath = string.IsNullOrEmpty(component.InstallPath)
                ? component.FileName
                : Path.Combine(component.InstallPath, component.FileName);

            return Path.Combine(_installBasePath, relativePath);
        }

        /// <summary>
        /// 설치 경로 미리보기 생성 (UI에서 사용)
        /// </summary>
        public string GetInstallPathPreview(string installPath, string fileName)
        {
            var relativePath = string.IsNullOrEmpty(installPath)
                ? fileName
                : Path.Combine(installPath, fileName);
            return Path.Combine(_installBasePath, relativePath);
        }

        /// <summary>
        /// 현재 시스템에 설치되어 있는 컴포넌트 목록을 반환합니다.
        /// 각 항목에는 설치된 경로, 버전, 파일 해시 등이 포함됩니다.
        /// </summary>
        public List<ClientComponentDto> GetInstalledComponents()
        {
            var installed = new List<ClientComponentDto>();
            var serverComponents = _componentRepo.GetAllComponents();

            foreach (var component in serverComponents)
            {
                var installPath = GetInstallPath(component);
                if (File.Exists(installPath))
                {
                    var fileInfo = new FileInfo(installPath);
                    installed.Add(new ClientComponentDto
                    {
                        ComponentId = component.ComponentId,
                        InstalledVersion = GetFileVersion(installPath),
                        InstalledPath = installPath,
                        InstalledDate = fileInfo.LastWriteTime,
                        FileHash = CalculateFileHash(installPath)
                    });
                }
            }

            return installed;
        }

        /// <summary>
        /// 업데이트가 필요한 컴포넌트 목록을 반환합니다. (서버의 활성 버전과 비교)
        /// </summary>
        public List<ComponentVerDto> CheckForUpdates()
        {
            var installed = GetInstalledComponents();
            return _componentRepo.CheckForUpdates(installed);
        }

        /// <summary>
        /// 누락된 필수 컴포넌트 목록을 반환합니다.
        /// 서버에 등록되어 있고 클라이언트에 없으며 IsRequired가 true인 항목만 필터링합니다.
        /// </summary>
        public List<ComponentVerDto> GetMissingComponents()
        {
            var installed = GetInstalledComponents();
            return _componentRepo.GetMissingComponents(installed)
                .Where(c => _componentRepo.GetComponent(c.ComponentId)?.IsRequired == true)
                .ToList();
        }

        /// <summary>
        /// 모든 업데이트를 다운로드하여 설치합니다.
        /// - progress: 전체 진행률 리포트
        /// - cancellationToken: 취소 지원
        /// </summary>
        public async Task<ComponentUpdateResult> UpdateAllAsync(
            IProgress<ComponentUpdateProgressEventArgs>? progress = null,
            CancellationToken cancellationToken = default)
        {
            var result = new ComponentUpdateResult();
            var updates = CheckForUpdates();

            if (updates.Count == 0)
            {
                result.Success = true;
                result.Message = "모든 컴포넌트가 최신 버전입니다.";
                return result;
            }

            int completed = 0;
            int total = updates.Count;

            foreach (var update in updates.OrderBy(u => _componentRepo.GetComponent(u.ComponentId)?.Priority ?? 999))
            {
                cancellationToken.ThrowIfCancellationRequested();

                var progressArgs = new ComponentUpdateProgressEventArgs
                {
                    TotalComponents = total,
                    CurrentIndex = completed,
                    CurrentComponentId = update.ComponentId,
                    CurrentComponentName = update.ComponentName,
                    Phase = "Downloading",
                    PercentComplete = (int)((completed * 100.0) / total)
                };
                progress?.Report(progressArgs);
                ProgressChanged?.Invoke(this, progressArgs);

                try
                {
                    await UpdateComponentAsync(update, cancellationToken);
                    result.UpdatedComponents.Add(update.ComponentId);

                    ComponentUpdated?.Invoke(this, new ComponentUpdatedEventArgs
                    {
                        ComponentId = update.ComponentId,
                        OldVersion = GetInstalledVersion(update.ComponentId),
                        NewVersion = update.Version,
                        Success = true
                    });
                }
                catch (Exception ex)
                {
                    result.FailedComponents.Add((update.ComponentId, ex.Message));

                    ComponentUpdated?.Invoke(this, new ComponentUpdatedEventArgs
                    {
                        ComponentId = update.ComponentId,
                        Success = false,
                        Error = ex.Message
                    });
                }

                completed++;
            }

            result.Success = result.FailedComponents.Count == 0;
            result.Message = result.Success 
                ? $"{result.UpdatedComponents.Count}개 컴포넌트 업데이트 완료"
                : $"{result.UpdatedComponents.Count}개 업데이트, {result.FailedComponents.Count}개 실패";

            return result;
        }

        /// <summary>
        /// 단일 컴포넌트를 다운로드하고 설치합니다.
        /// - 저장소의 StoragePath에서 캐시로 다운로드
        /// - 해시 검증
        /// - 실제 설치(대상 위치로 복사)
        /// </summary>
        public async Task UpdateComponentAsync(ComponentVerDto version, CancellationToken cancellationToken = default)
        {
            var component = _componentRepo.GetComponent(version.ComponentId);
            if (component == null)
                throw new InvalidOperationException($"Component not found: {version.ComponentId}");

            // Download to cache
            string cacheFile = Path.Combine(_downloadCachePath, component.GroupName ?? "Other", component.FileName);
            await DownloadToCacheAsync(version.StoragePath, cacheFile, cancellationToken);

            // Verify hash
            string downloadedHash = CalculateFileHash(cacheFile);
            if (!string.Equals(downloadedHash, version.FileHash, StringComparison.OrdinalIgnoreCase))
            {
                File.Delete(cacheFile);
                throw new InvalidOperationException("File hash mismatch - download may be corrupted");
            }

            // Install to target path
            string installPath = GetInstallPath(component);
            await InstallFromCacheAsync(cacheFile, installPath, cancellationToken);
        }

        /// <summary>
        /// 설치된 컴포넌트들의 무결성을 검사하고 문제(누락 또는 해시 불일치)를 반환합니다.
        /// </summary>
        public List<(string ComponentId, string Issue)> VerifyIntegrity()
        {
            var issues = new List<(string, string)>();
            var serverVersions = _componentRepo.GetActiveVersions();

            foreach (var version in serverVersions)
            {
                var component = _componentRepo.GetComponent(version.ComponentId);
                if (component == null) continue;

                var installPath = GetInstallPath(component);

                if (!File.Exists(installPath))
                {
                    if (component.IsRequired)
                        issues.Add((version.ComponentId, "Required component missing"));
                    continue;
                }

                var localHash = CalculateFileHash(installPath);
                if (!string.Equals(localHash, version.FileHash, StringComparison.OrdinalIgnoreCase))
                {
                    issues.Add((version.ComponentId, "File hash mismatch"));
                }
            }

            return issues;
        }

        #region Private Methods

        private string? GetInstalledVersion(string componentId)
        {
            var component = _componentRepo.GetComponent(componentId);
            if (component == null) return null;

            var installPath = GetInstallPath(component);
            if (!File.Exists(installPath)) return null;

            return GetFileVersion(installPath);
        }

        private static string GetFileVersion(string filePath)
        {
            try
            {
                var assemblyName = System.Reflection.AssemblyName.GetAssemblyName(filePath);
                return assemblyName.Version?.ToString() ?? "1.0.0.0";
            }
            catch
            {
                var versionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(filePath);
                return versionInfo.FileVersion ?? "1.0.0.0";
            }
        }

        private async Task DownloadToCacheAsync(string sourcePath, string cachePath, CancellationToken cancellationToken)
        {
            var directory = Path.GetDirectoryName(cachePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // For now, simple file copy (can be replaced with HTTP download)
            if (File.Exists(sourcePath))
            {
                await Task.Run(() => File.Copy(sourcePath, cachePath, true), cancellationToken);
            }
            else
            {
                // TODO: Download from server using ConnectivityManager
                throw new FileNotFoundException($"Source file not found: {sourcePath}");
            }
        }

        private async Task InstallFromCacheAsync(string cacheFile, string installPath, CancellationToken cancellationToken)
        {
            var directory = Path.GetDirectoryName(installPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            // Handle file in use
            int retries = 3;
            while (retries > 0)
            {
                try
                {
                    await Task.Run(() => File.Copy(cacheFile, installPath, true), cancellationToken);
                    return;
                }
                catch (IOException) when (retries > 1)
                {
                    retries--;
                    await Task.Delay(500, cancellationToken);
                }
            }

            throw new IOException($"Cannot install {Path.GetFileName(installPath)} - file may be in use");
        }

        private static string CalculateFileHash(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = sha256.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        #endregion
    }

    #region Event Args and Result Models

    /// <summary>
    /// 컴포넌트 업데이트 진행률 이벤트 인자
    /// </summary>
    public class ComponentUpdateProgressEventArgs : EventArgs
    {
        public int TotalComponents { get; set; }
        public int CurrentIndex { get; set; }
        public string? CurrentComponentId { get; set; }
        public string? CurrentComponentName { get; set; }
        public string? Phase { get; set; }
        public int PercentComplete { get; set; }
    }

    /// <summary>
    /// 개별 컴포넌트 업데이트 결과 이벤트 인자
    /// </summary>
    public class ComponentUpdatedEventArgs : EventArgs
    {
        public string? ComponentId { get; set; }
        public string? OldVersion { get; set; }
        public string? NewVersion { get; set; }
        public bool Success { get; set; }
        public string? Error { get; set; }
    }

    /// <summary>
    /// 컴포넌트 업데이트 전체 작업 결과 모델
    /// </summary>
    public class ComponentUpdateResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<string> UpdatedComponents { get; set; } = new();
        public List<(string ComponentId, string Error)> FailedComponents { get; set; } = new();
    }

    #endregion
}
