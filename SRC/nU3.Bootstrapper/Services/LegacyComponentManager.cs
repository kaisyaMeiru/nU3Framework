using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using Microsoft.Extensions.Configuration; // 추가

namespace nU3.Bootstrapper.Services
{
    /// <summary>
    /// Legacy OCX/DLL 컴포넌트 등록/해제를 관리하는 서비스
    /// </summary>
    public class LegacyComponentManager
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DllRegisterServerDelegate();

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        private delegate int DllUnregisterServerDelegate();

        private const int S_OK = 0;
        private readonly List<string> _registeredComponents = new();

        /// <summary>
        /// OCX/DLL을 등록합니다.
        /// </summary>
        /// <param name="dllPath">등록할 DLL 경로</param>
        /// <returns>등록 성공 여부</returns>
        public bool RegisterComponent(string dllPath)
        {
            if (string.IsNullOrWhiteSpace(dllPath) || !File.Exists(dllPath))
            {
                FileLogger.Error($"DLL 파일을 찾을 수 없습니다: {dllPath}");
                return false;
            }

            try
            {
                FileLogger.Info($"컴포넌트 등록 시도: {dllPath}");

                // 방법 1: DllRegisterServer 직접 호출 시도
                if (RegisterViaDllRegisterServer(dllPath))
                {
                    _registeredComponents.Add(dllPath);
                    FileLogger.Info($"컴포넌트 등록 성공 (DllRegisterServer): {dllPath}");
                    return true;
                }

                // 방법 2: regsvr32 명령 사용
                if (RegisterViaRegsvr32(dllPath))
                {
                    _registeredComponents.Add(dllPath);
                    FileLogger.Info($"컴포넌트 등록 성공 (regsvr32): {dllPath}");
                    return true;
                }

                FileLogger.Error($"컴포넌트 등록 실패: {dllPath}");
                return false;
            }
            catch (Exception ex)
            {
                FileLogger.Error($"컴포넌트 등록 중 오류: {dllPath}", ex);
                return false;
            }
        }

        /// <summary>
        /// OCX/DLL 등록을 해제합니다.
        /// </summary>
        /// <param name="dllPath">등록 해제할 DLL 경로</param>
        /// <returns>등록 해제 성공 여부</returns>
        public bool UnregisterComponent(string dllPath)
        {
            if (string.IsNullOrWhiteSpace(dllPath) || !File.Exists(dllPath))
            {
                FileLogger.Warning($"DLL 파일을 찾을 수 없습니다: {dllPath}");
                return false;
            }

            try
            {
                FileLogger.Info($"컴포넌트 등록 해제 시도: {dllPath}");

                // 방법 1: DllUnregisterServer 직접 호출 시도
                if (UnregisterViaDllUnregisterServer(dllPath))
                {
                    _registeredComponents.Remove(dllPath);
                    FileLogger.Info($"컴포넌트 등록 해제 성공 (DllUnregisterServer): {dllPath}");
                    return true;
                }

                // 방법 2: regsvr32 명령 사용
                if (UnregisterViaRegsvr32(dllPath))
                {
                    _registeredComponents.Remove(dllPath);
                    FileLogger.Info($"컴포넌트 등록 해제 성공 (regsvr32): {dllPath}");
                    return true;
                }

                FileLogger.Error($"컴포넌트 등록 해제 실패: {dllPath}");
                return false;
            }
            catch (Exception ex)
            {
                FileLogger.Error($"컴포넌트 등록 해제 중 오류: {dllPath}", ex);
                return false;
            }
        }

        /// <summary>
        /// DllRegisterServer를 직접 호출하여 등록
        /// </summary>
        private bool RegisterViaDllRegisterServer(string dllPath)
        {
            IntPtr hModule = IntPtr.Zero;
            try
            {
                hModule = LoadLibrary(dllPath);
                if (hModule == IntPtr.Zero)
                {
                    FileLogger.Warning($"DLL 로드 실패: {dllPath} (Error: {Marshal.GetLastWin32Error()})");
                    return false;
                }

                IntPtr pDllRegisterServer = GetProcAddress(hModule, "DllRegisterServer");
                if (pDllRegisterServer == IntPtr.Zero)
                {
                    FileLogger.Warning($"DllRegisterServer 함수를 찾을 수 없습니다: {dllPath}");
                    return false;
                }

                var dllRegisterServer = Marshal.GetDelegateForFunctionPointer<DllRegisterServerDelegate>(pDllRegisterServer);
                int result = dllRegisterServer();

                return result == S_OK;
            }
            finally
            {
                if (hModule != IntPtr.Zero)
                {
                    FreeLibrary(hModule);
                }
            }
        }

        /// <summary>
        /// DllUnregisterServer를 직접 호출하여 등록 해제
        /// </summary>
        private bool UnregisterViaDllUnregisterServer(string dllPath)
        {
            IntPtr hModule = IntPtr.Zero;
            try
            {
                hModule = LoadLibrary(dllPath);
                if (hModule == IntPtr.Zero)
                {
                    FileLogger.Warning($"DLL 로드 실패: {dllPath} (Error: {Marshal.GetLastWin32Error()})");
                    return false;
                }

                IntPtr pDllUnregisterServer = GetProcAddress(hModule, "DllUnregisterServer");
                if (pDllUnregisterServer == IntPtr.Zero)
                {
                    FileLogger.Warning($"DllUnregisterServer 함수를 찾을 수 없습니다: {dllPath}");
                    return false;
                }

                var dllUnregisterServer = Marshal.GetDelegateForFunctionPointer<DllUnregisterServerDelegate>(pDllUnregisterServer);
                int result = dllUnregisterServer();

                return result == S_OK;
            }
            finally
            {
                if (hModule != IntPtr.Zero)
                {
                    FreeLibrary(hModule);
                }
            }
        }

        /// <summary>
        /// regsvr32.exe를 사용하여 등록
        /// </summary>
        private bool RegisterViaRegsvr32(string dllPath)
        {
            return ExecuteRegsvr32(dllPath, false);
        }

        /// <summary>
        /// regsvr32.exe를 사용하여 등록 해제
        /// </summary>
        private bool UnregisterViaRegsvr32(string dllPath)
        {
            return ExecuteRegsvr32(dllPath, true);
        }

        /// <summary>
        /// regsvr32.exe 실행
        /// </summary>
        private bool ExecuteRegsvr32(string dllPath, bool unregister)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = "regsvr32.exe",
                    Arguments = $"{(unregister ? "/u " : "")}/s \"{dllPath}\"",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    Verb = "runas" // 관리자 권한 요청
                };

                using var process = Process.Start(startInfo);
                if (process == null)
                {
                    FileLogger.Error("regsvr32.exe 프로세스 시작 실패");
                    return false;
                }

                process.WaitForExit(30000); // 30초 타임아웃

                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                FileLogger.Error($"regsvr32.exe 실행 중 오류: {ex.Message}", ex);
                return false;
            }
        }

        /// <summary>
        /// 등록된 모든 컴포넌트 목록을 가져옵니다.
        /// </summary>
        public IReadOnlyList<string> GetRegisteredComponents()
        {
            return _registeredComponents.AsReadOnly();
        }

        /// <summary>
        /// 설정 파일에서 Legacy 컴포넌트 목록을 읽어 일괄 등록
        /// </summary>
        /// <param name="configuration">Configuration 객체</param>
        /// <returns>등록 결과 (성공한 컴포넌트, 실패한 컴포넌트)</returns>
        public (List<string> Success, List<string> Failed) RegisterFromConfiguration(Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            var success = new List<string>();
            var failed = new List<string>();

            var legacyComponents = configuration.GetSection("LegacyComponents").GetChildren();

            foreach (var component in legacyComponents)
            {
                var path = component.GetValue<string>("Path");
                var autoRegister = component.GetValue<bool>("AutoRegister", false);

                if (string.IsNullOrWhiteSpace(path))
                    continue;

                // 상대 경로를 절대 경로로 변환
                if (!Path.IsPathRooted(path))
                {
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
                }

                if (!autoRegister)
                {
                    FileLogger.Info($"자동 등록이 비활성화된 컴포넌트: {path}");
                    continue;
                }

                if (RegisterComponent(path))
                {
                    success.Add(path);
                }
                else
                {
                    failed.Add(path);
                }
            }

            return (success, failed);
        }

        /// <summary>
        /// 등록된 모든 컴포넌트를 정리합니다 (프로그램 종료 시)
        /// </summary>
        public void CleanupAll()
        {
            FileLogger.SectionStart("Legacy 컴포넌트 정리");

            var componentsToUnregister = _registeredComponents.ToList();

            foreach (var component in componentsToUnregister)
            {
                UnregisterComponent(component);
            }

            FileLogger.SectionEnd("Legacy 컴포넌트 정리");
        }
    }
}
