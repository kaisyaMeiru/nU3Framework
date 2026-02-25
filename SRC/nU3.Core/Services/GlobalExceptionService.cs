using System;
using System.Threading;
using System.Threading.Tasks;
using nU3.Core.Logging;
using nU3.Core.Interfaces;
using nU3.Models;

namespace nU3.Core.Services
{
    /// <summary>
    /// 애플리케이션 전역 예외 처리 및 크래시 리포팅을 관리하는 서비스입니다.
    /// </summary>
    public interface IGlobalExceptionService
    {
        void Initialize(bool enableReporting, bool uploadOnError, EmailSettings? emailSettings, ICrashReporter? crashReporter = null);
        void HandleException(Exception ex, string source);
        void RegisterGlobalHandlers();
    }

    public class GlobalExceptionService : IGlobalExceptionService
    {
        private bool _reportingEnabled;
        private bool _uploadOnError;
        private EmailSettings? _emailSettings;
        private ICrashReporter? _crashReporter;

        public void Initialize(bool enableReporting, bool uploadOnError, EmailSettings? emailSettings, ICrashReporter? crashReporter = null)
        {
            _reportingEnabled = enableReporting;
            _uploadOnError = uploadOnError;
            _emailSettings = emailSettings;
            _crashReporter = crashReporter;

            if (_reportingEnabled)
            {
                LogManager.Info("전역 예외 리포팅 서비스 초기화됨", "System");
            }
        }

        public void RegisterGlobalHandlers()
        {
            // Note: WinForms App에서 호출되어야 함
            AppDomain.CurrentDomain.UnhandledException += (s, e) => {
                if (e.ExceptionObject is Exception ex) HandleException(ex, "AppDomain");
            };
            TaskScheduler.UnobservedTaskException += (s, e) => {
                HandleException(e.Exception, "Task");
                e.SetObserved();
            };
        }

        public void HandleException(Exception ex, string source)
        {
            try
            {
                LogManager.Critical($"[GlobalError] {source}: {ex.Message}", "Error", ex);

                if (_reportingEnabled && _crashReporter != null)
                {
                    _crashReporter.ReportCrashAsync(ex, $"출처: {source}");
                }
            }
            catch
            {
                // 처리기 자체의 오류 방지
            }
        }
    }
}
