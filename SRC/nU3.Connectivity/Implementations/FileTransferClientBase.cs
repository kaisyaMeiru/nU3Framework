using System;
using nU3.Core.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace nU3.Connectivity.Implementations
{
    /// <summary>
    /// 파일 전송 서비스를 위한 클라이언트 기본 구현(추상).
    /// 
    /// 역할 및 책임:
    /// - 로컬 파일 시스템에서 파일을 읽고 쓰는 공통 로직을 제공
    /// - 암호화/복호화(선택 사항) 훅을 통해 전송 시 보안 처리 가능
    /// - 실제 원격 호출(업로드/다운로드/명령/조회)은 하위 클래스(Http, gRPC 등)가 Remote* 추상 메서드를 통해 구현
    /// 
    /// 설계 주의사항:
    /// - 본 클래스는 동기 API(예: UploadFile)를 비동기 API(UploadFileAsync)의 결과로 동기적으로 대기하여 구현합니다.
    ///   이로 인해 호출 스레드가 차단될 수 있으므로 UI 스레드에서 직접 호출하지 않도록 주의해야 합니다.
    /// - 큰 파일을 메모리로 읽어 전송하므로 메모리 사용량에 주의해야 합니다. 필요 시 스트리밍 방식으로 개선 고려.
    /// - Remote* 메서드의 구현은 네트워크/IO 예외 처리를 적절히 수행해야 하며, 이 클래스는 이러한 예외를 호출자에게 전달합니다.
    /// </summary>
    public abstract class FileTransferClientBase : IFileTransferService
    {
        // ----------------------------
        // 원격 호출을 구현해야 할 추상 메서드
        // ----------------------------
        /// <summary>
        /// 서버로 파일 바이트를 업로드합니다. 하위 클래스는 실제 전송 로직(HTTP/gRPC 등)을 구현해야 합니다.
        /// </summary>
        /// <param name="serverPath">서버 측 저장 경로(파일 시스템 경로 또는 논리 경로)</param>
        /// <param name="data">업로드할 파일 바이트 배열(전체)</param>
        /// <returns>업로드 성공 시 true, 실패 시 false 또는 예외 발생</returns>
        protected abstract Task<bool> RemoteUploadAsync(string serverPath, byte[] data);

        /// <summary>
        /// 서버에서 지정한 경로의 파일을 다운로드하여 바이트 배열로 반환합니다.
        /// 하위 클래스는 네트워크 호출 및 예외 처리를 구현해야 합니다.
        /// </summary>
        /// <param name="serverPath">서버 파일 경로</param>
        /// <returns>다운로드한 파일의 바이트 배열</returns>
        protected abstract Task<byte[]> RemoteDownloadAsync(string serverPath);

        /// <summary>
        /// 서버에 파일/디렉토리 관련 명령(예: CreateDirectory, DeleteFile 등)을 실행하도록 요청합니다.
        /// 하위 클래스는 명령을 적절한 REST 엔드포인트/원격 호출로 매핑하여 처리해야 합니다.
        /// </summary>
        /// <param name="command">실행할 명령 이름</param>
        /// <param name="args">명령에 필요한 인자 배열</param>
        /// <returns>명령 성공 여부</returns>
        protected abstract Task<bool> RemoteCommandAsync(string command, object[] args);

        /// <summary>
        /// 서버에 조회(쿼리) 요청을 보내고 제네릭 타입으로 결과를 반환합니다.
        /// 예: GetFileList -> List<string>, GetHomeDirectory -> string, GetFileSize -> long
        /// </summary>
        /// <typeparam name="T">예상되는 반환 타입</typeparam>
        /// <param name="command">조회 명령 이름</param>
        /// <param name="args">조회 인자</param>
        /// <returns>타입 T로 역직렬화된 결과</returns>
        protected abstract Task<T> RemoteQueryAsync<T>(string command, object[] args);

        // ----------------------------
        // 디렉토리 / 파일 관리 API (동기 및 비동기)
        // 동기 메서드는 내부적으로 비동기 메서드를 블록킹 방식으로 호출합니다.
        // ----------------------------

        /// <summary>
        /// 서버에서 사용할 홈 디렉토리 설정을 지정합니다. (동기 버전)
        /// 주의: 내부적으로 비동기 메서드를 동기적으로 대기하므로 호출 스레드가 차단됩니다.
        /// </summary>
        /// <param name="isUseHomePath">서버에서 홈 경로 사용 여부</param>
        /// <param name="serverHomePath">서버에 설정할 홈 경로</param>
        /// <returns>설정 성공 여부</returns>
        public bool SetHomeDirectory(bool isUseHomePath, string serverHomePath) => SetHomeDirectoryAsync(isUseHomePath, serverHomePath).Result;

        /// <summary>
        /// 서버에서 사용할 홈 디렉토리 설정을 지정합니다. (비동기 버전)
        /// </summary>
        /// <param name="isUseHomePath">서버에서 홈 경로 사용 여부</param>
        /// <param name="serverHomePath">서버에 설정할 홈 경로</param>
        /// <returns>설정 성공 여부</returns>
        public async Task<bool> SetHomeDirectoryAsync(bool isUseHomePath, string serverHomePath)
        {
            return await RemoteCommandAsync(nameof(SetHomeDirectory), new object[] { isUseHomePath, serverHomePath }).ConfigureAwait(false);
        }

        /// <summary>
        /// 서버에 설정된 홈 디렉토리 경로를 반환합니다. (동기 버전)
        /// </summary>
        /// <returns>서버 홈 디렉토리 경로 문자열</returns>
        public string GetHomeDirectory() => GetHomeDirectoryAsync().Result;

        /// <summary>
        /// 서버에 설정된 홈 디렉토리 경로를 반환합니다. (비동기 버전)
        /// </summary>
        /// <returns>서버 홈 디렉토리 경로 문자열</returns>
        public async Task<string> GetHomeDirectoryAsync()
        {
            return await RemoteQueryAsync<string>(nameof(GetHomeDirectory), null).ConfigureAwait(false);
        }

        /// <summary>
        /// 서버에 디렉토리를 생성합니다. (동기 버전)
        /// </summary>
        /// <param name="fullPath">생성할 전체 경로</param>
        /// <returns>생성 성공 여부</returns>
        public bool CreateDirectory(string fullPath) => CreateDirectoryAsync(fullPath).Result;

        /// <summary>
        /// 서버에 디렉토리를 생성합니다. (비동기 버전)
        /// </summary>
        /// <param name="fullPath">생성할 전체 경로</param>
        /// <returns>생성 성공 여부</returns>
        public async Task<bool> CreateDirectoryAsync(string fullPath)
        {
            return await RemoteCommandAsync(nameof(CreateDirectory), new object[] { fullPath }).ConfigureAwait(false);
        }

        /// <summary>
        /// 지정된 경로의 디렉토리 존재 여부를 확인합니다. (동기 버전)
        /// </summary>
        public bool ExistDirectory(string fullPath) => ExistDirectoryAsync(fullPath).Result;

        /// <summary>
        /// 지정된 경로의 디렉토리 존재 여부를 확인합니다. (비동기 버전)
        /// </summary>
        /// <param name="fullPath">검사할 전체 경로</param>
        /// <returns>존재하면 true</returns>
        public async Task<bool> ExistDirectoryAsync(string fullPath)
        {
             return await RemoteQueryAsync<bool>(nameof(ExistDirectory), new object[] { fullPath }).ConfigureAwait(false);
        }

        /// <summary>
        /// 서버의 디렉토리를 삭제합니다. (동기 버전)
        /// </summary>
        public bool DeleteDirectory(string fullPath) => DeleteDirectoryAsync(fullPath).Result;

        /// <summary>
        /// 서버의 디렉토리를 삭제합니다. (비동기 버전)
        /// </summary>
        /// <param name="fullPath">삭제할 전체 경로</param>
        /// <returns>삭제 성공 여부</returns>
        public async Task<bool> DeleteDirectoryAsync(string fullPath)
        {
             return await RemoteCommandAsync(nameof(DeleteDirectory), new object[] { fullPath }).ConfigureAwait(false);
        }

        /// <summary>
        /// 디렉토리 이름(경로) 변경을 수행합니다. (동기 버전)
        /// </summary>
        public bool RenameDirectory(string sourcePath, string destPath) => RenameDirectoryAsync(sourcePath, destPath).Result;

        /// <summary>
        /// 디렉토리 이름(경로) 변경을 수행합니다. (비동기 버전)
        /// </summary>
        /// <param name="sourcePath">원본 전체 경로</param>
        /// <param name="destPath">대상 전체 경로</param>
        /// <returns>이동/이름 변경 성공 여부</returns>
        public async Task<bool> RenameDirectoryAsync(string sourcePath, string destPath)
        {
             return await RemoteCommandAsync(nameof(RenameDirectory), new object[] { sourcePath, destPath }).ConfigureAwait(false);
        }

        /// <summary>
        /// 지정 경로의 파일 목록을 반환합니다. (동기 버전)
        /// </summary>
        public List<string> GetFileList(string fullPath) => GetFileListAsync(fullPath).Result;

        /// <summary>
        /// 지정 경로의 파일 목록을 반환합니다. (비동기 버전)
        /// </summary>
        /// <param name="fullPath">조회할 디렉토리의 전체 경로</param>
        /// <returns>서버에서 반환한 파일명 목록</returns>
        public async Task<List<string>> GetFileListAsync(string fullPath)
        {
            return await RemoteQueryAsync<List<string>>(nameof(GetFileList), new object[] { fullPath }).ConfigureAwait(false);
        }

        /// <summary>
        /// 지정 경로의 하위 디렉토리 목록을 반환합니다. (동기 버전)
        /// </summary>
        public List<string> GetSubDirectoryList(string fullPath) => GetSubDirectoryListAsync(fullPath).Result;

        /// <summary>
        /// 지정 경로의 하위 디렉토리 목록을 반환합니다. (비동기 버전)
        /// </summary>
        /// <param name="fullPath">조회할 디렉토리의 전체 경로</param>
        /// <returns>하위 디렉토리명 목록</returns>
        public async Task<List<string>> GetSubDirectoryListAsync(string fullPath)
        {
            return await RemoteQueryAsync<List<string>>(nameof(GetSubDirectoryList), new object[] { fullPath }).ConfigureAwait(false);
        }

        // ----------------------------
        // 파일 전송 관련 API
        // ----------------------------

        /// <summary>
        /// 로컬 파일을 서버로 업로드(동기).
        /// 주의: 동기 호출은 호출 스레드를 차단하므로 UI 스레드에서 호출하지 마십시오.
        /// </summary>
        /// <param name="localPath">로컬 파일 전체 경로</param>
        /// <param name="serverPath">서버 측 저장 경로</param>
        /// <param name="encryptionFunc">업로드 전에 바이트 배열을 변환(암호화)할 델리게이트(선택)</param>
        /// <returns>업로드 성공 여부</returns>
        public bool UploadFile(string localPath, string serverPath, SecurityFunction? encryptionFunc = null) => UploadFileAsync(localPath, serverPath, encryptionFunc).Result;

        /// <summary>
        /// 로컬 파일을 서버로 업로드(비동기).
        /// 
        /// 흐름:
        /// 1. 로컬 파일이 존재하는지 확인
        /// 2. 파일 전체를 바이트 배열로 읽음(메모리 사용 주의)
        /// 3. encryptionFunc가 제공되면 해당 델리게이트로 바이트 배열을 변환
        /// 4. RemoteUploadAsync를 호출하여 실제 전송 수행
        /// </summary>
        /// <param name="localPath">로컬 파일 전체 경로</param>
        /// <param name="serverPath">서버 측 저장 경로</param>
        /// <param name="encryptionFunc">전송 전에 적용할 암호화/변환 함수(선택)</param>
        /// <returns>업로드 성공 여부</returns>
        public async Task<bool> UploadFileAsync(string localPath, string serverPath, SecurityFunction? encryptionFunc = null)
        {
            if (!File.Exists(localPath)) throw new FileNotFoundException("로컬 파일을 찾을 수 없습니다", localPath);

            // 파일 전체를 메모리로 읽음(대용량 파일의 경우 스트리밍 방식 권장)
            byte[] data = await File.ReadAllBytesAsync(localPath).ConfigureAwait(false);
            if (encryptionFunc != null)
            {
                // 암호화/변환이 필요한 경우 호출
                data = encryptionFunc(data);
            }

            // 실제 원격 업로드는 하위 클래스가 구현
            return await RemoteUploadAsync(serverPath, data).ConfigureAwait(false);
        }

        /// <summary>
        /// 암호화된 전송을 위한 항목(동기). 향후 서버 경로 암호화 로직 등을 여기에 추가 가능.
        /// </summary>
        public bool UploadFileWithSecurity(string localPath, string serverPath, SecurityFunction? encryptionFunc = null, string encServerPath = "") => UploadFileWithSecurityAsync(localPath, serverPath, encryptionFunc, encServerPath).Result;

        /// <summary>
        /// 암호화된 전송을 위한 항목(비동기).
        /// 현재는 기본 UploadFileAsync를 호출하여 동일하게 동작합니다.
        /// </summary>
        public async Task<bool> UploadFileWithSecurityAsync(string localPath, string serverPath, SecurityFunction? encryptionFunc = null, string encServerPath = "")
        {
            // 기본 업로드 흐름과 동일하게 동작하며 필요시 서버 경로 암호화 처리 등을 추가할 수 있음
            return await UploadFileAsync(localPath, serverPath, encryptionFunc).ConfigureAwait(false);
        }

        /// <summary>
        /// 서버의 파일을 로컬로 다운로드(동기)
        /// </summary>
        /// <param name="serverPath">서버 파일 경로</param>
        /// <param name="localPath">로컬에 저장할 전체 경로</param>
        /// <param name="decryptionFunc">수신 후 적용할 복호화 함수(선택)</param>
        /// <returns>다운로드 성공 여부</returns>
        public bool DownloadFile(string serverPath, string localPath, SecurityFunction? decryptionFunc = null) => DownloadFileAsync(serverPath, localPath, decryptionFunc).Result;

        /// <summary>
        /// 서버의 파일을 로컬로 다운로드(비동기).
        /// 흐름:
        /// 1. RemoteDownloadAsync로 바이트 배열 수신
        /// 2. 필요 시 decryptionFunc 적용
        /// 3. 로컬 파일로 저장
        /// </summary>
        public async Task<bool> DownloadFileAsync(string serverPath, string localPath, SecurityFunction? decryptionFunc = null)
        {
            byte[] data = await RemoteDownloadAsync(serverPath).ConfigureAwait(false);
            
            if (decryptionFunc != null)
            {
                data = decryptionFunc(data);
            }

            await File.WriteAllBytesAsync(localPath, data).ConfigureAwait(false);
            return true;
        }

        /// <summary>
        /// 보안 다운로드(동기)
        /// </summary>
        public bool DownloadFileWithSecurity(string serverPath, string localPath, SecurityFunction? decryptionFunc = null, string encLocalPath = "") => DownloadFileWithSecurityAsync(serverPath, localPath, decryptionFunc, encLocalPath).Result;

        /// <summary>
        /// 보안 다운로드(비동기)
        /// 현재는 일반 다운로드와 동일하게 동작합니다. 암호화된 로컬 저장 경로 등의 기능을 추가하려면 확장하세요.
        /// </summary>
        public async Task<bool> DownloadFileWithSecurityAsync(string serverPath, string localPath, SecurityFunction? decryptionFunc = null, string encLocalPath = "")
        {
             return await DownloadFileAsync(serverPath, localPath, decryptionFunc).ConfigureAwait(false);
        }

        /// <summary>
        /// 파일 존재 여부 확인(동기)
        /// </summary>
        /// <param name="fullFilePath">서버 상의 전체 파일 경로</param>
        /// <returns>파일이 존재하면 true</returns>
        public bool ExistFile(string fullFilePath) => ExistFileAsync(fullFilePath).Result;

        /// <summary>
        /// 파일 존재 여부 확인(비동기)
        /// </summary>
        /// <param name="fullFilePath">서버 상의 전체 파일 경로</param>
        /// <returns>파일이 존재하면 true</returns>
        public async Task<bool> ExistFileAsync(string fullFilePath)
        {
            return await RemoteQueryAsync<bool>(nameof(ExistFile), new object[] { fullFilePath }).ConfigureAwait(false);
        }

        /// <summary>
        /// 파일 삭제(동기)
        /// </summary>
        /// <param name="fullFilePath">서버 상의 전체 파일 경로</param>
        /// <returns>삭제 성공 여부</returns>
        public bool DeleteFile(string fullFilePath) => DeleteFileAsync(fullFilePath).Result;

        /// <summary>
        /// 파일 삭제(비동기)
        /// </summary>
        public async Task<bool> DeleteFileAsync(string fullFilePath)
        {
            return await RemoteCommandAsync(nameof(DeleteFile), new object[] { fullFilePath }).ConfigureAwait(false);
        }

        /// <summary>
        /// 파일 복사(동기)
        /// </summary>
        public bool CopyFile(string sourceFullPath, string destFullPath, bool overWrite = true) => CopyFileAsync(sourceFullPath, destFullPath, overWrite).Result;

        /// <summary>
        /// 파일 복사(비동기)
        /// </summary>
        /// <param name="sourceFullPath">원본 전체 경로</param>
        /// <param name="destFullPath">대상 전체 경로</param>
        /// <param name="overWrite">대상 파일이 있을 경우 덮어쓸지 여부</param>
        /// <returns>복사 성공 여부</returns>
        public async Task<bool> CopyFileAsync(string sourceFullPath, string destFullPath, bool overWrite = true)
        {
            return await RemoteCommandAsync(nameof(CopyFile), new object[] { sourceFullPath, destFullPath, overWrite }).ConfigureAwait(false);
        }

        /// <summary>
        /// 파일 이동(동기)
        /// </summary>
        public bool MoveFile(string sourceFullPath, string destFullPath, bool overWrite = true) => MoveFileAsync(sourceFullPath, destFullPath, overWrite).Result;

        /// <summary>
        /// 파일 이동(비동기)
        /// </summary>
        /// <param name="sourceFullPath">원본 전체 경로</param>
        /// <param name="destFullPath">대상 전체 경로</param>
        /// <param name="overWrite">대상 파일이 있을 경우 덮어쓸지 여부</param>
        /// <returns>이동 성공 여부</returns>
        public async Task<bool> MoveFileAsync(string sourceFullPath, string destFullPath, bool overWrite = true)
        {
            return await RemoteCommandAsync(nameof(MoveFile), new object[] { sourceFullPath, destFullPath, overWrite }).ConfigureAwait(false);
        }

        /// <summary>
        /// 파일 크기 조회(동기)
        /// </summary>
        public long GetFileSize(string fileFullPath) => GetFileSizeAsync(fileFullPath).Result;

        /// <summary>
        /// 파일 크기 조회(비동기)
        /// </summary>
        /// <param name="fileFullPath">서버 상의 전체 파일 경로</param>
        /// <returns>파일 크기 바이트 단위</returns>
        public async Task<long> GetFileSizeAsync(string fileFullPath)
        {
            return await RemoteQueryAsync<long>(nameof(GetFileSize), new object[] { fileFullPath }).ConfigureAwait(false);
        }
    }
}