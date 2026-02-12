using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace nU3.Core.Interfaces
{
    /// <summary>
    /// 파일 전송 중 암호화/복호화 로직을 전달하는 델리게이트입니다.
    /// </summary>
    public delegate byte[] SecurityFunction(byte[] data);

    /// <summary>
    /// 파일 전송 서비스 인터페이스
    /// 동기 및 비동기 방식의 파일/디렉토리 작업을 지원합니다.
    /// </summary>
    public interface IFileTransferService
    {
        // ==========================
        // 디렉토리 작업
        // ==========================
        bool SetHomeDirectory(bool isUseHomePath, string serverHomePath);
        Task<bool> SetHomeDirectoryAsync(bool isUseHomePath, string serverHomePath);

        string GetHomeDirectory();
        Task<string> GetHomeDirectoryAsync();

        bool CreateDirectory(string fullPath);
        Task<bool> CreateDirectoryAsync(string fullPath);

        bool ExistDirectory(string fullPath);
        Task<bool> ExistDirectoryAsync(string fullPath);

        bool DeleteDirectory(string fullPath);
        Task<bool> DeleteDirectoryAsync(string fullPath);

        bool RenameDirectory(string sourcePath, string destPath);
        Task<bool> RenameDirectoryAsync(string sourcePath, string destPath);

        List<string> GetFileList(string fullPath);
        Task<List<string>> GetFileListAsync(string fullPath);

        List<string> GetSubDirectoryList(string fullPath);
        Task<List<string>> GetSubDirectoryListAsync(string fullPath);

        // ==========================
        // 파일 작업
        // ==========================
        bool UploadFile(string localPath, string serverPath, SecurityFunction? encryptionFunc = null);
        Task<bool> UploadFileAsync(string localPath, string serverPath, SecurityFunction? encryptionFunc = null);

        bool UploadFileWithSecurity(string localPath, string serverPath, SecurityFunction? encryptionFunc = null, string encServerPath = "");
        Task<bool> UploadFileWithSecurityAsync(string localPath, string serverPath, SecurityFunction? encryptionFunc = null, string encServerPath = "");

        bool DownloadFile(string serverPath, string localPath, SecurityFunction? decryptionFunc = null);
        Task<bool> DownloadFileAsync(string serverPath, string localPath, SecurityFunction? decryptionFunc = null);

        bool DownloadFileWithSecurity(string serverPath, string localPath, SecurityFunction? decryptionFunc = null, string encLocalPath = "");
        Task<bool> DownloadFileWithSecurityAsync(string serverPath, string localPath, SecurityFunction? decryptionFunc = null, string encLocalPath = "");

        bool ExistFile(string fullFilePath);
        Task<bool> ExistFileAsync(string fullFilePath);

        bool DeleteFile(string fullFilePath);
        Task<bool> DeleteFileAsync(string fullFilePath);

        bool CopyFile(string sourceFullPath, string destFullPath, bool overWrite = true);
        Task<bool> CopyFileAsync(string sourceFullPath, string destFullPath, bool overWrite = true);

        bool MoveFile(string sourceFullPath, string destFullPath, bool overWrite = true);
        Task<bool> MoveFileAsync(string sourceFullPath, string destFullPath, bool overWrite = true);

        long GetFileSize(string fileFullPath);
        Task<long> GetFileSizeAsync(string fileFullPath);
    }
}
