using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace nU3.Core.Pipes
{
    public static class NamedPipeClient
    {
        public static async Task<bool> SendAsync(string pipeName, string message, int timeout = 2000)
        {
            try
            {
                using (var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out, PipeOptions.Asynchronous))
                {
                    await pipeClient.ConnectAsync(timeout).ConfigureAwait(false);

                    using (var writer = new StreamWriter(pipeClient, Encoding.UTF8))
                    {
                        await writer.WriteAsync(message).ConfigureAwait(false);
                        await writer.FlushAsync().ConfigureAwait(false);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
