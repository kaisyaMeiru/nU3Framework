using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using nU3.Core.Logging;
using nU3.Core.Services;
using nU3.Core.UI.Shell.Services;
using nU3.Models;

namespace nU3.Core.UI.Shell
{
    /// <summary>
    /// �� ����(�α�, ���Ἲ, ���� ������, ũ���� ������)�� �����մϴ�.
    /// ��� �� ���� ���� �߾� ���߽� �ʱ�ȭ �� ���� ����� �����մϴ�.
    /// </summary>
    public class ShellServiceManager : IDisposable
    {
        private readonly Form _ownerForm;
        private readonly string _shellName;

        // Services
        private CrashReportService? _crashReportService;
        private EmailService? _emailService;

        // Settings
        private EmailSettings? _emailSettings;
        private LoggingSettings? _loggingSettings;
        private ServerConnectionSettings? _serverSettings;

        // State
        private bool _loggingEnabled;
        private bool _errorReportingEnabled;
        private bool _serverConnectionEnabled;
        private bool _isInitialized;
        private bool _disposed;

        /// <summary>
        /// �α� ��� ����
        /// </summary>
        public bool LoggingEnabled => _loggingEnabled;

        /// <summary>
        /// ���� ������ ��� ����
        /// </summary>
        public bool ErrorReportingEnabled => _errorReportingEnabled;

        /// <summary>
        /// ���� ���� ��� ����
        /// </summary>
        public bool ServerConnectionEnabled
        {
            get => _serverConnectionEnabled;
            set => _serverConnectionEnabled = value;
        }

        /// <summary>
        /// ���� �߻� �� ���ε� ��� ����
        /// </summary>
        public bool UploadOnError => _loggingSettings?.UploadOnError ?? false;

        /// <summary>
        /// �ʱ�ȭ ����
        /// </summary>
        public bool IsInitialized => _isInitialized;

        /// <summary>
        /// ũ���� ����Ʈ ����
        /// </summary>
        public CrashReportService? CrashReporter => _crashReportService;

        /// <summary>
        /// �̸��� ����
        /// </summary>
        public EmailService? Email => _emailService;

        /// <summary>
        /// ���� ���� ���� ���� �̺�Ʈ
        /// </summary>
        public event EventHandler<ServerConnectionStatusEventArgs>? ServerConnectionStatusChanged;

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="ownerForm">�� �Ŵ����� ������ ��</param>
        /// <param name="shellName">�α뿡 ����� �� �̸�</param>
        public ShellServiceManager(Form ownerForm, string shellName)
        {
            _ownerForm = ownerForm ?? throw new ArgumentNullException(nameof(ownerForm));
            _shellName = shellName ?? "Shell";
        }

        /// <summary>
        /// ��� ���� �ʱ�ȭ
        /// </summary>
        public void Initialize()
        {
            if (_isInitialized)
                return;

            // ���� �ε�
            _loggingSettings = ShellConfiguration.LoadLoggingSettings();
            _emailSettings = ShellConfiguration.LoadEmailSettings();
            _serverSettings = ShellConfiguration.LoadServerConnectionSettings();

            // �ʱ�ȭ ����
            InitializeLogging();
            InitializeServerConnection();
            InitializeErrorReporting();
            InitializeCrashReporting();

            _isInitialized = true;
            LogManager.Info($"{_shellName} services initialized", _shellName);
        }

        /// <summary>
        /// �α� �ý��� �ʱ�ȭ
        /// </summary>
        private void InitializeLogging()
        {
            try
            {
                if (_loggingSettings == null || !_loggingSettings.Enabled)
                    return;

                _loggingEnabled = true;

                LogManager.Instance.Initialize(
                    logDirectory: _loggingSettings.LogDirectory,
                    auditDirectory: _loggingSettings.AuditDirectory,
                    fileTransferService: null,
                    enableAutoUpload: _loggingSettings.AutoUpload
                );

                LogManager.Info("Logging system initialized successfully", _shellName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to initialize logging: {ex.Message}");
                _loggingEnabled = false;
            }
        }

        /// <summary>
        /// ���� ���� �ʱ�ȭ
        /// </summary>
        private void InitializeServerConnection()
        {
            try
            {
                if (_serverSettings == null || !_serverSettings.Enabled)
                {
                    LogManager.Info("Server connection is disabled in configuration", _shellName);
                    OnServerConnectionStatusChanged(false, null, "Disabled");
                    _serverConnectionEnabled = false;
                    return;
                }

                LogManager.Info($"Initializing server connection: {_serverSettings.BaseUrl}", _shellName);

                // ConnectivityManager �ʱ�ȭ
                ConnectivityManager.Instance.Initialize(
                    _serverSettings.BaseUrl!,
                    enableLogCompression: true,
                    maxConcurrentConnections: _serverSettings.MaxConcurrentConnections
                );

                // �α� �޽��� �̺�Ʈ ����
                ConnectivityManager.Instance.LogMessage += OnConnectivityLogMessage;

                if (_serverSettings.AutoLogUpload)
                {
                    LogManager.Info("Auto log upload will be enabled", _shellName);
                }

                // ���� �׽�Ʈ (�񵿱�)
                TestServerConnectionAsync();
            }
            catch (Exception ex)
            {
                LogManager.Error($"Failed to initialize server connection: {ex.Message}", _shellName, ex);
                OnServerConnectionStatusChanged(false, null, $"Error: {ex.Message}");
                _serverConnectionEnabled = false;
            }
        }

        private async void TestServerConnectionAsync()
        {
            try
            {
                var connected = await ConnectivityManager.Instance.TestConnectionAsync();

                if (_ownerForm.IsHandleCreated && !_ownerForm.IsDisposed)
                {
                    _ownerForm.Invoke((MethodInvoker)delegate
                    {
                        if (connected)
                        {
                            OnServerConnectionStatusChanged(true, _serverSettings?.BaseUrl, "Connected");
                            LogManager.Info($"Server connection successful: {_serverSettings?.BaseUrl}", _shellName);
                            _serverConnectionEnabled = true;
                        }
                        else
                        {
                            OnServerConnectionStatusChanged(false, _serverSettings?.BaseUrl, "No Response");
                            LogManager.Warning($"Server connection failed: {_serverSettings?.BaseUrl}", _shellName);
                            _serverConnectionEnabled = false;
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                if (_ownerForm.IsHandleCreated && !_ownerForm.IsDisposed)
                {
                    _ownerForm.Invoke((MethodInvoker)delegate
                    {
                        OnServerConnectionStatusChanged(false, _serverSettings?.BaseUrl, $"Error: {ex.Message}");
                        LogManager.Error($"Server connection error: {ex.Message}", _shellName, ex);
                        _serverConnectionEnabled = false;
                    });
                }
            }
        }

        /// <summary>
        /// ���� ������ �ʱ�ȭ (���� ���� ó����)
        /// </summary>
        private void InitializeErrorReporting()
        {
            try
            {
                _errorReportingEnabled = _emailSettings != null;

                if (_errorReportingEnabled)
                {
                    Application.ThreadException += Application_ThreadException;
                    AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
                    TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

                    LogManager.Info("Error reporting initialized", _shellName);
                }
            }
            catch (Exception ex)
            {
                LogManager.Error("Failed to initialize error reporting", _shellName, ex);
                _errorReportingEnabled = false;
            }
        }

        /// <summary>
        /// ũ���� ������ ���� �ʱ�ȭ
        /// </summary>
        private void InitializeCrashReporting()
        {
            try
            {
                _crashReportService = new CrashReportService(
                    _ownerForm,
                    _emailSettings,
                    _shellName
                );

                // Cleanup old logs on startup
                _crashReportService.CleanupOldLogs(_loggingSettings?.RetentionDays ?? 30);

                if (_emailSettings != null)
                {
                    _emailService = new EmailService(_emailSettings);
                }

                LogManager.Info("Crash reporting service initialized", _shellName);
            }
            catch (Exception ex)
            {
                LogManager.Error("Failed to initialize crash reporting", _shellName, ex);
            }
        }

        #region Event Handlers

        private void OnConnectivityLogMessage(object? sender, LogMessageEventArgs e)
        {
            try
            {
                switch (e.Level.ToLower())
                {
                    case "info":
                        LogManager.Info(e.Message, "Connectivity");
                        break;
                    case "warning":
                        LogManager.Warning(e.Message, "Connectivity");
                        break;
                    case "error":
                        LogManager.Error(e.Message, "Connectivity");
                        break;
                    default:
                        LogManager.Debug(e.Message, "Connectivity");
                        break;
                }
            }
            catch
            {
                // Ignore logging errors
            }
        }

        private void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception, "UI Thread Exception");
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception exception)
            {
                HandleUnhandledException(exception, "AppDomain Unhandled Exception");
            }
        }

        private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleUnhandledException(e.Exception, "Task Unobserved Exception");
            e.SetObserved();
        }

        #endregion

        /// <summary>
        /// ó������ ���� ���ܸ� ó���մϴ�.
        /// </summary>
        public void HandleUnhandledException(Exception exception, string source, string? additionalInfo = null)
        {
            try
            {
                var info = additionalInfo ?? $"Source: {source}";

                // �α� ���
                LogManager.Critical($"Unhandled Exception - {source}", "Error", exception);
                LogManager.Critical($"Additional Info: {info}", "Error");

                // ������ �α� ���ε�
                if (_loggingEnabled && UploadOnError && _serverConnectionEnabled && ConnectivityManager.Instance.IsInitialized)
                {
                    Task.Run(async () =>
                    {
                        try
                        {
                            await ConnectivityManager.Instance.Log.UploadCurrentLogImmediatelyAsync();
                            LogManager.Info("Error log uploaded to server", _shellName);
                        }
                        catch (Exception ex)
                        {
                            LogManager.Warning($"Failed to upload error log: {ex.Message}", _shellName);
                        }
                    }).Wait(TimeSpan.FromSeconds(5));
                }

                // ũ���� ����Ʈ ����
                if (_crashReportService != null)
                {
                    _crashReportService.ReportCrash(exception, info);
                }

                var result = MessageBox.Show(
                    $"����ġ ���� ������ �߻��߽��ϴ�.\n\n" +
                    $"����: {exception.Message}\n\n" +
                    $"{(_errorReportingEnabled ? "���� ����Ʈ�� �����ڿ��� �ڵ����� ���۵Ǿ����ϴ�.\n\n" : "")}" +
                    $"���α׷��� �����Ͻðڽ��ϱ�?",
                    "���� �߻�",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error);

                if (result == DialogResult.Yes)
                {
                    LogManager.Info("User chose to exit after error", _shellName);
                    Environment.Exit(1);
                }
            }
            catch
            {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// ��� ���� ���� �׽�Ʈ
        /// </summary>
        public async Task<ConnectivityTestResult?> TestAllConnectionsAsync(CancellationToken cancellationToken = default)
        {
            if (!ConnectivityManager.Instance.IsInitialized)
                return null;

            return await ConnectivityManager.Instance.TestAllConnectionsAsync(cancellationToken);
        }

        /// <summary>
        /// ��� ���� ����
        /// </summary>
        public void Shutdown()
        {
            if (_disposed)
                return;

            try
            {
                // ���� ������ �̺�Ʈ ���� ����
                if (_errorReportingEnabled)
                {
                    Application.ThreadException -= Application_ThreadException;
                    AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
                    TaskScheduler.UnobservedTaskException -= TaskScheduler_UnobservedTaskException;
                }

                // ���� ����� ��� ��� ���� �α� ���ε�
                if (_serverConnectionEnabled && ConnectivityManager.Instance.IsInitialized)
                {
                    try
                    {
                        LogManager.Info("Uploading pending logs to server before shutdown", _shellName);

                        var uploadTask = Task.Run(async () =>
                        {
                            await ConnectivityManager.Instance.Log.UploadAllPendingLogsAsync();
                        });

                        if (!uploadTask.Wait(TimeSpan.FromSeconds(10)))
                        {
                            LogManager.Warning("Log upload timeout during shutdown", _shellName);
                        }
                        else
                        {
                            LogManager.Info("Pending logs uploaded successfully", _shellName);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogManager.Warning($"Failed to upload pending logs: {ex.Message}", _shellName);
                    }

                    // ConnectivityManager �̺�Ʈ ���� ����
                    ConnectivityManager.Instance.LogMessage -= OnConnectivityLogMessage;

                    // ConnectivityManager ����
                    ConnectivityManager.Instance.Dispose();
                    LogManager.Info("ConnectivityManager disposed", _shellName);
                }

                // LogManager ����
                if (_loggingEnabled)
                {
                    LogManager.Instance.Shutdown();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error during shutdown: {ex.Message}");
            }
        }

        private void OnServerConnectionStatusChanged(bool connected, string? serverUrl, string status)
        {
            ServerConnectionStatusChanged?.Invoke(this, new ServerConnectionStatusEventArgs
            {
                Connected = connected,
                ServerUrl = serverUrl,
                Status = status
            });
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            Shutdown();
            
            _crashReportService?.Dispose();
            _emailService?.Dispose();
            
            _disposed = true;
        }
    }

    /// <summary>
    /// ���� ���� ���� ���� �̺�Ʈ �μ�
    /// </summary>
    public class ServerConnectionStatusEventArgs : EventArgs
    {
        public bool Connected { get; set; }
        public string? ServerUrl { get; set; }
        public string? Status { get; set; }
    }
}
