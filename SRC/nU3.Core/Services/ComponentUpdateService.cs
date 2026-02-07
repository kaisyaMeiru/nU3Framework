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
    /// 컴포넌트 업데이트 서비스 (레거시/독립형)
    /// </summary>
    public class ComponentUpdateService
    {
        private readonly IComponentRepository _componentRepo;
        private readonly string _installBasePath;
        private readonly string _downloadCachePath;

        public string InstallBasePath => _installBasePath;
        public string DownloadCachePath => _downloadCachePath;

        public event EventHandler<ComponentUpdateProgressEventArgs>? ProgressChanged;
        public event EventHandler<ComponentUpdatedEventArgs>? ComponentUpdated;

        public ComponentUpdateService(IComponentRepository componentRepo, string? installBasePath = null)
        {
            _componentRepo = componentRepo ?? throw new ArgumentNullException(nameof(componentRepo));
            _installBasePath = installBasePath ?? AppDomain.CurrentDomain.BaseDirectory;
            _downloadCachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "nU3.Framework", "Cache", "Components");

            if (!Directory.Exists(_downloadCachePath))
                Directory.CreateDirectory(_downloadCachePath);
        }

        public string GetInstallPath(ComponentMstDto component)
        {
            if (component == null) throw new ArgumentNullException(nameof(component));
            var relativePath = string.IsNullOrEmpty(component.InstallPath) ? component.FileName : Path.Combine(component.InstallPath, component.FileName);
            return Path.Combine(_installBasePath, relativePath);
        }

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

        public List<ComponentVerDto> CheckForUpdates()
        {
            var installed = GetInstalledComponents();
            return _componentRepo.CheckForUpdates(installed);
        }

        public async Task<ComponentUpdateResult> UpdateAllAsync(IProgress<ComponentUpdateProgressEventArgs>? progress = null, CancellationToken cancellationToken = default)
        {
            var result = new ComponentUpdateResult();
            var updates = CheckForUpdates();

            if (updates.Count == 0)
            {
                result.Success = true;
                result.Message = "모든 컴포넌트가 최신입니다.";
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
                        NewVersion = update.Version,
                        Success = true
                    });
                }
                catch (Exception ex)
                {
                    result.FailedComponents.Add((update.ComponentId, ex.Message));
                    ComponentUpdated?.Invoke(this, new ComponentUpdatedEventArgs { ComponentId = update.ComponentId, Success = false, Error = ex.Message });
                }
                completed++;
            }

            result.Success = result.FailedComponents.Count == 0;
            result.Message = result.Success ? $"{result.UpdatedComponents.Count}개 완료" : "오류 발생";
            return result;
        }

        public async Task UpdateComponentAsync(ComponentVerDto version, CancellationToken cancellationToken = default)
        {
            var component = _componentRepo.GetComponent(version.ComponentId);
            if (component == null) throw new InvalidOperationException($"Component not found: {version.ComponentId}");

            string cacheFile = Path.Combine(_downloadCachePath, component.GroupName ?? "Other", component.FileName);
            
            var directory = Path.GetDirectoryName(cacheFile);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory)) Directory.CreateDirectory(directory);

            if (File.Exists(version.StoragePath)) File.Copy(version.StoragePath, cacheFile, true);
            else throw new FileNotFoundException($"Source file not found: {version.StoragePath}");

            string installPath = GetInstallPath(component);
            var instDir = Path.GetDirectoryName(installPath);
            if (!string.IsNullOrEmpty(instDir) && !Directory.Exists(instDir)) Directory.CreateDirectory(instDir);
            File.Copy(cacheFile, installPath, true);
        }

        private static string GetFileVersion(string filePath)
        {
            try { return System.Reflection.AssemblyName.GetAssemblyName(filePath).Version?.ToString() ?? "1.0.0.0"; }
            catch { return System.Diagnostics.FileVersionInfo.GetVersionInfo(filePath).FileVersion ?? "1.0.0.0"; }
        }

        private static string CalculateFileHash(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = File.OpenRead(filePath);
            var hash = sha256.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }

    public class ComponentUpdateProgressEventArgs : EventArgs
    {
        public int TotalComponents { get; set; }
        public int CurrentIndex { get; set; }
        public string? CurrentComponentId { get; set; }
        public string? CurrentComponentName { get; set; }
        public string? Phase { get; set; }
        public int PercentComplete { get; set; }
    }

    public class ComponentUpdatedEventArgs : EventArgs
    {
        public string? ComponentId { get; set; }
        public string? OldVersion { get; set; }
        public string? NewVersion { get; set; }
        public bool Success { get; set; }
        public string? Error { get; set; }
    }
}