using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using nU3.Core.Logging;

namespace nU3.Core.Pipes
{
    public class NamedPipeServer : IDisposable
    {
        public event EventHandler<string> OnMessageReceived;
        
        private string _pipeName;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _isRunning;
        private Task _listenerTask;

        public bool IsRunning => _isRunning;

        public void Start(string pipeName)
        {
            if (_isRunning)
                return;

            _pipeName = pipeName;
            _cancellationTokenSource = new CancellationTokenSource();
            _isRunning = true;

            _listenerTask = Task.Run(() => ListenLoopAsync(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            
            LogManager.Info($"Named Pipe 서버 시작됨: {_pipeName}", "Core.Pipes");
        }

        public void Stop()
        {
            if (!_isRunning)
                return;

            _isRunning = false;
            _cancellationTokenSource?.Cancel();
            
            // 필요 시 리스너 정리 대기 (오래 차단하지 않음)
            try
            {
                _listenerTask?.Wait(500);
            }
            catch { }

            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;

            LogManager.Info($"Named Pipe 서버 중지됨: {_pipeName}", "Core.Pipes");
        }

        private async Task ListenLoopAsync(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    NamedPipeServerStream pipeServer = null;
                    try
                    {
                        pipeServer = new NamedPipeServerStream(_pipeName, PipeDirection.In, NamedPipeServerStream.MaxAllowedServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

                        // 클라이언트 연결 대기
                        await pipeServer.WaitForConnectionAsync(token).ConfigureAwait(false);

                        if (token.IsCancellationRequested)
                            break;

                        // 메시지 읽기
                        using (var reader = new StreamReader(pipeServer, Encoding.UTF8))
                        {
                            string message = await reader.ReadToEndAsync().ConfigureAwait(false);
                            if (!string.IsNullOrEmpty(message))
                            {
                                OnMessageReceived?.Invoke(this, message);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (IOException ex)
                    {
                        LogManager.Warning($"Named Pipe IO 오류: {ex.Message}", "Core.Pipes");
                    }
                    catch (Exception ex)
                    {
                        LogManager.Error($"Named Pipe 서버 오류: {ex.Message}", "Core.Pipes", ex);
                    }
                    finally
                    {
                        pipeServer?.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                // 이 catch 블록은 UnobservedTaskException 방지에 중요함
                // 루프 자체 충돌 또는 토큰 처리 예외 방지
                System.Diagnostics.Debug.WriteLine($"NamedPipeServer ListenLoop Crashed: {ex.Message}");
                LogManager.Error($"Named Pipe 서버 Listen 루프 치명적 오류: {ex.Message}", "Core.Pipes", ex);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
